using System;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using Application.InfrastructureInterfaces;
using Application.Validators;
using Application.ViewModels;
using Domain.Model;
using FluentValidation;
using MediatR;

namespace Application.Commands
{
    public class VerifyEmailAddressCommandValidator : AbstractValidator<VerifyEmailAddressCommand>
    {
        public VerifyEmailAddressCommandValidator()
        {
            RuleFor(command => command.Username).NotNull().NotEmpty();
            RuleFor(command => command.Token).SetValidator(new EmailVerificationTokenValidator());
        }
    }

    public class VerifyEmailAddressCommand : IRequest<SuccessViewModel>
    {
        public VerifyEmailAddressCommand(string username, string token)
        {
            Username = username;
            Token = token;
        }

        public string Username { get; }
        public string Token { get; }
    }

    public class VerifyEmailAddressCommandHandler : IRequestHandler<VerifyEmailAddressCommand, SuccessViewModel>
    {
        private readonly IUnitOfWork _unitOfWork;

        public VerifyEmailAddressCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<SuccessViewModel> Handle(VerifyEmailAddressCommand command,
            CancellationToken cancellationToken)
        {
            var user = await GetUserIfExistsAsync(command, cancellationToken);

            await ValidateTokenAsync(command, user, cancellationToken);

            user.VerifyEmail();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return new SuccessViewModel();
        }

        private async Task ValidateTokenAsync(VerifyEmailAddressCommand command, User user, CancellationToken cancellationToken)
        {
            var isTokenValid = user.EmailVerificationTokenHash != default
                               && user.EmailVerificationTokenValidTo != default
                               && BCrypt.Net.BCrypt.Verify(command.Token, user.EmailVerificationTokenHash)
                               && user.EmailVerificationTokenValidTo >= DateTime.Now;
            if (!isTokenValid)
            {
                await Task.Delay(ApplicationConstants.InvalidAuthOperationExtraDelayInMilliseconds, cancellationToken);
                throw new AuthenticationException("The link is not valid, expired or you have generated new one");
            }
        }

        private async Task<User> GetUserIfExistsAsync(VerifyEmailAddressCommand command,
            CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.UserRepository.GetByUsernameAsync(command.Username, cancellationToken);
            if (user == default)
            {
                await Task.Delay(ApplicationConstants.InvalidAuthOperationExtraDelayInMilliseconds, cancellationToken);
                throw new AuthenticationException("The link is not valid, expired or you have generated new one");
            }

            return user;
        }
    }
}