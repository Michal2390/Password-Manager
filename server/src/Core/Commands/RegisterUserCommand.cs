using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Exceptions;
using Application.InfrastructureInterfaces;
using Application.Utils.Email;
using Application.Utils.Email.Templates;
using Application.Utils.RandomStringGenerator;
using Application.Validators;
using Application.ViewModels;
using Domain.Model;
using FluentValidation;
using MediatR;
using Serilog;

namespace Application.Commands
{
    public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserCommandValidator()
        {
            RuleFor(command => command.Email).SetValidator(new EmailValidator());
            RuleFor(command => command.Username).SetValidator(new UsernameValidator());
            RuleFor(command => command.Password).SetValidator(new PasswordValidator());
            RuleFor(command => command.EncryptionKeyHash).NotNull().NotEmpty().WithMessage("Must not be empty");
        }
    }

    public class RegisterUserCommand : IRequest<SuccessViewModel>
    {
        public RegisterUserCommand(string username, string email, string password, string encryptionKeyHash)
        {
            Username = username;
            Email = email;
            Password = password;
            EncryptionKeyHash = encryptionKeyHash;
        }

        public string Username { get; }
        public string Email { get; }
        public string Password { get; }
        public string EncryptionKeyHash { get; }
    }

    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, SuccessViewModel>
    {
        private readonly IEmailService _emailService;
        private readonly ApplicationSettings _settings;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterUserCommandHandler(IEmailService emailService, ApplicationSettings settings,
            IUnitOfWork unitOfWork)
        {
            _emailService = emailService;
            _settings = settings;
            _unitOfWork = unitOfWork;
        }

        public async Task<SuccessViewModel> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
        {
            await CheckIfUserAlreadyExists(command.Email, command.Username);
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(command.Password + _settings.PasswordHashPepper, 12)!;
            var masterPasswordHash =
                BCrypt.Net.BCrypt.HashPassword(command.EncryptionKeyHash + _settings.EncryptionKeyHashPepper, 12)!;
            var (verificationToken, verificationTokenHash, verificationTokenValidTo, universalToken) =
                GenerateEmailVerificationToken();

            await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var registeredUser = User.Register(command.Username.ToLower(), command.Email.ToLower(), passwordHash,
                masterPasswordHash, verificationTokenHash, verificationTokenValidTo, universalToken);
            await _unitOfWork.UserRepository.AddAsync(registeredUser, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await TryToSendEmailAsync(command.Email.ToLower(), command.Username.ToLower(), verificationToken);
            await transaction.CommitAsync(cancellationToken);
            return new SuccessViewModel();
        }

        private static (string, string, DateTime, string) GenerateEmailVerificationToken()
        {
            var emailVerificationToken = Guid.NewGuid().ToString();
            var emailVerificationTokenHash = BCrypt.Net.BCrypt.HashPassword(emailVerificationToken);
            var emailVerificationTokenValidTo =
                DateTime.Now.AddMinutes(ApplicationConstants.EmailVerificationTokenDurationInMinutes);
            var universalToken =
                RandomStringGenerator.GeneratePasswordResetToken(ApplicationConstants.UniversalTokenLength);
            return (emailVerificationToken, emailVerificationTokenHash, emailVerificationTokenValidTo, universalToken);
        }

        private async Task CheckIfUserAlreadyExists(string email, string username)
        {
            var user = await _unitOfWork.UserRepository.GetByEmailOrUsernameAsync(email, username);

            if (user != default)
            {
                var propertyName = user.Email == email.ToLower()
                    ? nameof(email)
                    : nameof(username);
                throw new BadRequestException($"User with given {propertyName} already exists");
            }
        }

        private async Task TryToSendEmailAsync(string email, string username, string verificationToken)
        {
            try
            {
                var url = _settings.FrontendUrl + "/verify-email-address/" + username + "/" + verificationToken;
                await _emailService.SendEmailAsync(email, new VerifyEmailAddressEmailTemplateData(username, url));
            }
            catch (Exception exception)
            {
                Log.Error(exception, "Exception occured during sending verification email");
            }
        }
    }
}