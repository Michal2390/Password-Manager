using System.Threading.Tasks;
using Application.Commands;
using Application.Queries;
using Application.Utils.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api")]
    [Authorize]
    [ApiController]
    public class VaultController : BaseController
    {
        [HttpGet("vault")]
        public async Task<ActionResult> GetVault()
        {
            var query = new GetVaultQuery
            {
                UserId = AuthorizationService.RequireUserId(HttpContext.User),
            };
            return Ok(await Mediator.Send(query));
        }

        [HttpPost("vault")]
        public async Task<ActionResult> AddCipherLogin([FromBody] AddCipherLoginCommand command)
        {
            command.UserId = AuthorizationService.RequireUserId(HttpContext.User);
            await Mediator.Send(command);
            return NoContent();
        }
    }
}