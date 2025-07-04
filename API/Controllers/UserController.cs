using API.BusinessRules.Interfaces;
using API.DataServices.Interfaces;
using API.Dtos.UserDtos;
using API.Entities;
using API.Security.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    /// <summary>
    /// Manages user operations including creation, retrieval, update with concurrency control, and deletion.
    /// </summary>
    [ApiController]
    [Route("api/user")]
    public class UserController(
        IUserDataService userDataService,
        IUserBusinessRules userBusinessRules,
        IMapper mapper,
        IUserSecurityService userSecurityService) : ControllerBase
    {
        private readonly IUserDataService _userDataService = userDataService ?? throw new ArgumentNullException(nameof(userDataService));
        private readonly IUserBusinessRules _userBusinessRules = userBusinessRules ?? throw new ArgumentNullException(nameof(userBusinessRules));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        private readonly IUserSecurityService _userSecurityService = userSecurityService ?? throw new ArgumentNullException(nameof(userSecurityService));

        [HttpGet("{key:int}", Name = "GetUserByKey")]
        public async Task<ActionResult<UserReadDto>> GetUserByKeyAsync(int key)
        {
            var user = await _userDataService.GetUserByKeyAsync(key);

            if (user == null)
                return NotFound(new { message = $"User with key {key} not found." });

            return Ok(_mapper.Map<UserReadDto>(user));
        }

        [HttpGet("identification/{identification}")]
        public async Task<ActionResult<UserReadDto>> GetUserByIdentificationAsync(string identification)
        {
            var user = await _userDataService.GetUserByIdentificationAsync(identification);

            if (user == null)
                return NotFound(new { message = $"User with identification {identification} not found." });

            return Ok(_mapper.Map<UserReadDto>(user));
        }

        [HttpGet("email/{email}")]
        public async Task<ActionResult<UserReadDto>> GetUserByEmailAsync(string email)
        {
            var user = await _userDataService.GetUserByEmailAsync(email);

            if (user == null)
                return NotFound(new { message = $"User with email {email} not found." });

            return Ok(_mapper.Map<UserReadDto>(user));
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<UserReadDto>>> GetAllUsersAsync()
        {
            var users = await _userDataService.GetAllUsersAsync();

            if (users == null || users.Count == 0)
                return NotFound(new { message = "Users not found." });

            return Ok(_mapper.Map<List<UserReadDto>>(users));
        }

        /// <summary>
        /// Real conflict: Simplified constructor in C# 12 and endpoints in ASP.NET Core 8/7.
        /// Your controller uses the new C# simplified constructor syntax, which is perfectly valid.
        /// However, in some versions of ASP.NET Core (especially 7 and 8), when combined with CreatedAtAction(...),
        /// the system may fail to correctly assign ActionDescriptor.RouteValues["action"],
        /// which breaks response routing.
        /// To resolve this, you have two options:
        /// 1. Use the full classic constructor instead of the simplified one.
        /// 2. Manually specify the action name in the [HttpGet] attribute using Name = "...", like: [HttpGet("{key:int}", Name = "GetUserByKey")],
        ///     and return with CreatedAtRoute like: return CreatedAtRoute("GetUserByKey", new { key = user.Key }, userReadDto);
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<UserReadDto>> AddUserAsync(UserCreateDto userCreateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (Success, ErrorMessage) = await _userBusinessRules.ValidateNewUserAsync(userCreateDto);
            if (!Success)
                return BadRequest(new { message = ErrorMessage });

            var user = _mapper.Map<User>(userCreateDto);

            user.Password = _userSecurityService.HashPassword(user, userCreateDto.Password);

            await _userDataService.AddUserAsync(user);

            var userReadDto = _mapper.Map<UserReadDto>(user);

            return CreatedAtRoute("GetUserByKey", new { key = user.Key }, userReadDto);

        }

        [HttpPut("{key:int}")]
        public async Task<ActionResult> UpdateUserAsync(int key, UserUpdateDto userUpdateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (key != userUpdateDto.Key)
                return BadRequest(new { message = "User key mismatch." });

            var existingUser = await _userDataService.GetUserByKeyAsync(key);
            if (existingUser == null)
                return NotFound(new { message = $"User with key {key} not found." });

            var (Success, ErrorMessage) = await _userBusinessRules.ValidateUpdatedUserAsync(userUpdateDto);
            if (!Success)
                return BadRequest(new { message = ErrorMessage });

            // This is a mapping over an existing instance.
            _mapper.Map(userUpdateDto, existingUser);

            if (_userSecurityService.NeedsRehash(existingUser, userUpdateDto.Password))
                existingUser.Password = _userSecurityService.HashPassword(existingUser, userUpdateDto.Password);

            // This is necessary to handle an expected situation.
            try
            {
                await _userDataService.UpdateUserAsync(existingUser);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict(new { message = "The user was modified by another user. Please reload and try again." });
            }

            return NoContent();
        }

        [HttpDelete("{key:int}")]
        public async Task<ActionResult> DeleteUserAsync(int key)
        {
            if (await _userDataService.GetUserByKeyAsync(key) is null)
                return NotFound(new { message = $"User with key {key} not found." });

            await _userDataService.DeleteUserAsync(key);

            return NoContent();
        }
    }
}