using API.BusinessRules.Interfaces;
using API.DataServices.Interfaces;
using API.Dtos.UserDtos;
using API.Entities;
using API.Security.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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

        //[HttpGet("{key:int}")]
        [HttpGet("{key:int}", Name = "GetUserByKey")]
        public async Task<ActionResult<UserReadDto>> GetUserByKeyAsync(int key)
        {
            var user = await _userDataService.GetUserByKeyAsync(key);

            if (user == null)
                return NotFound(new { message = $"User with key {key} not found." });

            // Este es un mapeo hacia una nueva instancia UserReadDto.
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
        /// Conflicto real: Constructor simplificado en C# 12 y endpoints en ASP.NET Core 8/7.
        /// Tu controlador usa la nueva sintaxis de constructor simplificado de C#: Este patrón es perfectamente válido, 
        /// pero en algunas versiones de ASP.NET Core (especialmente 7 y 8),
        /// combinado con CreatedAtAction(...), puede provocar que el sistema no asigne correctamente el ActionDescriptor.RouteValues["action"], 
        /// lo que rompe el enrutamiento de respuesta. 
        /// Para resolver tenemos dos opciones: 
        /// 1. Usa el constructor clásico completo de C# en lugar del simplificado.
        /// 2. Agrega manualmente el nombre de la acción al atributo [HttpGet] usando Name = "..."], así: [HttpGet("{key:int}", Name = "GetUserByKey")] y 
        /// retornando CreatedAtRoute así: return CreatedAtRoute("GetUserByKey", new { key = user.Key }, userReadDto);
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

            Console.WriteLine($"User.Key después de reload: {userReadDto.Key}");

            //return CreatedAtAction(nameof(GetUserByKeyAsync), new { key = user.Key }, userReadDto);
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

            // Este es un mapeo sobre una instancia ya existente.
            _mapper.Map(userUpdateDto, existingUser);

            if (_userSecurityService.NeedsRehash(existingUser, userUpdateDto.Password))
                existingUser.Password = _userSecurityService.HashPassword(existingUser, userUpdateDto.Password);

            // Es necesario para controlar una situación esperada.
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