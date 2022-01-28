using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Exceptions;
using Application.InfrastructureInterfaces;
using Application.Validators;
using Application.ViewModels;
using FluentValidation;
using MediatR;

namespace Application.Commands
{
    public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
    {
        public ResetPasswordCommandValidator()
        {
            RuleFor(command => command.Username).NotNull().NotEmpty();
            RuleFor(command => command.ResetPasswordToken).NotNull().NotEmpty();
            RuleFor(command => command.NewPassword).SetValidator(new PasswordValidator());
        }
    }

    public class ResetPasswordCommand : IRequest<SuccessViewModel>
    {
        public ResetPasswordCommand(string username, string resetPasswordToken, string newPassword)
        {
            Username = username;
            ResetPasswordToken = resetPasswordToken;
            NewPassword = newPassword;
        }

        public string Username { get; }
        public string ResetPasswordToken { get; }
        public string NewPassword { get; }
    }

    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, SuccessViewModel>
    {
        private readonly ApplicationSettings _settings;
        private readonly IUnitOfWork _unitOfWork;

        public ResetPasswordCommandHandler(IUnitOfWork unitOfWork, ApplicationSettings settings)
        {
            _unitOfWork = unitOfWork;
            _settings = settings;
        }

        public async Task<SuccessViewModel> Handle(ResetPasswordCommand command, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.UserRepository.GetByUsernameAsync(command.Username, cancellationToken);
            if (user == default)
            {
                throw new NotFoundException("User not found");
            }

            var isTokenValid = BCrypt.Net.BCrypt.Verify(command.ResetPasswordToken, user.PasswordResetTokenHash)
                               && DateTime.Now < user.PasswordResetTokenValidTo;

            if (!isTokenValid)
            {
                await Task.Delay(ApplicationConstants.InvalidAuthOperationExtraDelayInMilliseconds, cancellationToken);
                throw new BadRequestException("The link is not valid, expired or you have generated new one");
            }

            var newPasswordHash =
                BCrypt.Net.BCrypt.HashPassword(command.NewPassword + _settings.PasswordHashPepper, 14);
            user.ChangePassword(newPasswordHash);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return new SuccessViewModel();
        }
    }
}