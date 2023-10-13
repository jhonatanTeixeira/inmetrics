using Application.Service;
using Domain.Document;
using Infrastructure.Dto;
using Infrastructure.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace Users.Controllers
{
    [ApiController]
    [Route("")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly UserContext UserContext;

        private readonly IUserService UserService;

        private readonly IAuthentication Authentication;

        public UserController(UserContext userContext, IUserService userService, IAuthentication authentication)
        {
            UserContext = userContext;
            UserService = userService;
            Authentication = authentication;
        }

        [HttpGet("profile")]
        public async Task<ActionResult<User>> Profile()
        {
            return await UserService.GetById(UserContext.UserId);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult> Login([FromBody] Login login)
        {
            if (await Authentication.Login(login)) {
                return Ok(new { login.Token });
            }

            return Unauthorized();
        }
    }
}