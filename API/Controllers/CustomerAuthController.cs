using API.BusinessRules.Interfaces;
using API.DataServices.Interfaces;
using API.Dtos.CustomerDtos;
using API.Dtos.UserDtos;
using API.Entities;
using API.Security;
using API.Security.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/customer-auth")]
    public class CustomerAuthController(
        ICustomerDataService customerDataService,
        ICustomerBusinessRules customerBusinessRules,
        IMapper mapper,
        ICustomerSecurityService customerSecurityService,
        IJwtService jwtService) : ControllerBase
    {
        private readonly ICustomerDataService _customerDataService = customerDataService ?? throw new ArgumentNullException(nameof(customerDataService));
        private readonly ICustomerBusinessRules _customerBusinessRules = customerBusinessRules ?? throw new ArgumentNullException(nameof(customerBusinessRules));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        private readonly ICustomerSecurityService _customerSecurityService = customerSecurityService ?? throw new ArgumentNullException(nameof(customerSecurityService));
        private readonly IJwtService _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));

        [HttpPost("login")]
        public async Task<ActionResult<string>> LoginAsync([FromBody] CustomerLoginDto customerLoginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var customer = await _customerDataService.GetCustomerByEmailAsync(customerLoginDto.Email);

            if (customer is null || !_customerSecurityService.VerifyPassword(customer, customerLoginDto.Password))
                return Unauthorized(new { message = "Credenciales inválidas." });

            var token = _jwtService.GenerateCustomerToken(customer);
            return Ok(new { token });
        }

        [HttpPost("register")]
        public async Task<ActionResult<string>> RegisterAsync([FromBody] CustomerCreateDto customerCreateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (Success, ErrorMessage) = await _customerBusinessRules.ValidateNewCustomerAsync(customerCreateDto);
            if (!Success)
                return BadRequest(new { message = ErrorMessage });

            var customer = _mapper.Map<Customer>(customerCreateDto);

            customer.Password = _customerSecurityService.HashPassword(customer, customerCreateDto.Password);

            await _customerDataService.AddCustomerAsync(customer);

            var token = _jwtService.GenerateCustomerToken(customer);
            return CreatedAtRoute("GetCustomerByKey", new { key = customer.Key }, new { token });
        }
    }
}