using CleanArchitecture.Application.Users.Commands.CreateUser;
using CleanArchitecture.Application.Users.Commands.Login;
using CleanArchitecture.WebAPI.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.WebAPI.Controllers
{
    public class UserController : BaseApiController
    {
        #region Actions
        [HttpPost("user-login")]
        public async Task<IActionResult> UserLogin([FromBody] LoginCommand loginCommand)
        {
            return Result(await Mediator.Send(loginCommand));
        }

        [Authorize]
        [HttpPost("Create-user")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand loginCommand)
        {
            return Result(await Mediator.Send(loginCommand));
        }
        #endregion
    }
}
