using System.Threading;
using System.Threading.Tasks;
using Application.Exceptions;
using Application.InfrastructureInterfaces;
using Application.Utils.Email;
using Application.Utils.Email.Templates;
using Application.Validators;
using Application.ViewModels;
using FluentValidation;
using MediatR;

namespace Application.Commands
{
    public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
    {
        public ChangePasswordCommandValidator()
        {
            RuleFor(command => command.NewPassword).SetValidator(new PasswordValidator());
            RuleFor(command => command.OldPassword).NotNull().NotEmpty();
        }
    }

    public class ChangePasswordCommand : IRequest<SuccessViewModel>
    {
        public ChangePasswordCommand(long userId, string oldPassword, string newPassword)
        {
            UserId = userId;
            OldPassword = oldPassword;
            NewPassword = newPassword;
        }

        public long UserId { get; set; }
        public string OldPassword { get; }
        public string NewPassword { get; }
    }

    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, SuccessViewModel>
    {
        private readonly IEmailService _emailService;
        private readonly ApplicationSettings _settings;
        private readonly IUnitOfWork _unitOfWork;

        public ChangePasswordCommandHandler(IUnitOfWork unitOfWork, IEmailService emailService,
            ApplicationSettings settings)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _settings = settings;
        }

        public async Task<SuccessViewModel> Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(command.UserId, cancellationToken);
            if (user == default)
            {
                throw new NotFoundException("User not found");
            }

            var isPasswordVerified =
                BCrypt.Net.BCrypt.Verify(command.OldPassword + _settings.PasswordHashPepper, user.PasswordHash);

            if (!isPasswordVerified)
            {
                await Task.Delay(ApplicationConstants.InvalidAuthOperationExtraDelayInMilliseconds, cancellationToken);
                throw new BadRequestException("Invalid credentials");
            }

            var newPasswordHash =
                BCrypt.Net.BCrypt.HashPassword(command.NewPassword + _settings.PasswordHashPepper, 14);
            user.ChangePassword(newPasswordHash);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _emailService.SendEmailAsync(user.Email, new ChangedPasswordTemplateData(user.Username));
            return new SuccessViewModel();
        }
    }
}