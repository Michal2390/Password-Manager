using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Exceptions;
using Application.InfrastructureInterfaces;
using Application.Utils.Email;
using Application.Utils.Email.Templates;
using Application.Utils.RandomStringGenerator;
using Application.ViewModels;
using FluentValidation;
using MediatR;

namespace Application.Commands
{
    public class RequestResetPasswordCommandValidator : AbstractValidator<RequestResetPasswordCommand>
    {
        public RequestResetPasswordCommandValidator()
        {
            RuleFor(command => command.Identifier).NotNull().NotEmpty();
        }
    }

    public class RequestResetPasswordCommand : IRequest<SuccessViewModel>
    {
        public RequestResetPasswordCommand(string identifier)
        {
            Identifier = identifier;
        }

        public string Identifier { get; set; }
    }

    public class RequestResetPasswordCommandHandler : IRequestHandler<RequestResetPasswordCommand, SuccessViewModel>
    {
        private readonly IEmailService _emailService;
        private readonly ApplicationSettings _settings;
        private readonly IUnitOfWork _unitOfWork;

        public RequestResetPasswordCommandHandler(IUnitOfWork unitOfWork, IEmailService emailService,
            ApplicationSettings settings)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _settings = settings;
        }

        public async Task<SuccessViewModel> Handle(RequestResetPasswordCommand command,
            CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.UserRepository.GetByEmailOrUsernameAsync(command.Identifier,
                command.Identifier, cancellationToken);
            if (user == default)
            {
                throw new NotFoundException("User not found");
            }

            var passwordResetToken =
                RandomStringGenerator.GeneratePasswordResetToken(ApplicationConstants.PasswordResetTokenLength);
            var passwordResetTokenHash = BCrypt.Net.BCrypt.HashPassword(passwordResetToken);
            var passwordResetTokenValidTo =
                DateTime.Now.AddMinutes(ApplicationConstants.PasswordResetTokenDurationInMinutes);
            var url = _settings.FrontendUrl + "/reset-password/" + user.Username + "/" + passwordResetToken;

            user.AddPasswordResetToken(passwordResetTokenHash, passwordResetTokenValidTo);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _emailService.SendEmailAsync(user.Email, new RequestResetPasswordTemplateData(user.Username, url));
            return new SuccessViewModel();
        }
    }
}