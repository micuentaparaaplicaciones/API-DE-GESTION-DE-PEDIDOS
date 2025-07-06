using API.DataServices.Interfaces;
using API.Dtos.UserDtos;
using API.Security.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/user-auth")]
    public class UserAuthController(
        IUserDataService userDataService,
        IUserSecurityService userSecurityService,
        IJwtService jwtService) : ControllerBase
    {
        private readonly IUserDataService _userDataService = userDataService ?? throw new ArgumentNullException(nameof(userDataService));
        private readonly IUserSecurityService _userSecurityService = userSecurityService ?? throw new ArgumentNullException(nameof(userSecurityService));
        private readonly IJwtService _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));

        [HttpPost("login")]
        public async Task<ActionResult<string>> LoginAsync([FromBody] UserLoginDto userLoginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userDataService.GetUserByEmailAsync(userLoginDto.Email);

            if (user is null || !_userSecurityService.VerifyPassword(user, userLoginDto.Password))
                return Unauthorized(new { message = "Credenciales inválidas." });

            var token = _jwtService.GenerateUserToken(user);
            return Ok(new { token });
        }
    }
}