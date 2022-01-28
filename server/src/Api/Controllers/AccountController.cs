using System.Threading.Tasks;
using Application.Commands;
using Application.Queries;
using Application.Utils.Authorization;
using Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/accounts")]
    [ApiController]
    public class AccountController : BaseController
    {
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult> Register([FromBody] RegisterUserCommand command)
        {
            await Mediator.Send(command);
            return NoContent();
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthViewModel>> Login([FromBody] LoginUserCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPost("verify-email-address")]
        [AllowAnonymous]
        public async Task<ActionResult> VerifyEmailAddress([FromBody] VerifyEmailAddressCommand command)
        {
            await Mediator.Send(command);
            return NoContent();
        }

        [HttpPost("request-password-reset")]
        [AllowAnonymous]
        public async Task<ActionResult> RequestPasswordReset([FromBody] RequestResetPasswordCommand command)
        {
            await Mediator.Send(command);
            return NoContent();
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
        {
            await Mediator.Send(command);
            return NoContent();
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
        {
            command.UserId = AuthorizationService.RequireUserId(HttpContext.User);
            await Mediator.Send(command);
            return NoContent();
        }

        [HttpPost("verify-encryption-key")]
        [Authorize]
        public async Task<ActionResult> VerifyEncryptionKeyHash([FromBody] VerifyEncryptionKeyHashCommand command)
        {
            command.UserId = AuthorizationService.RequireUserId(HttpContext.User);
            await Mediator.Send(command);
            return NoContent();
        }

        [HttpPost("request-email-verification")]
        [AllowAnonymous]
        public async Task<ActionResult> RequestEmailVerification([FromBody] RequestEmailVerificationCommand command)
        {
            await Mediator.Send(command);
            return NoContent();
        }
    }
}