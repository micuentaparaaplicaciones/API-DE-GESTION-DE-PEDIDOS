using API.BusinessRules.Interfaces;
using API.DataServices.Interfaces;
using API.Dtos.CustomerDtos;
using API.Dtos.UserDtos;
using API.Entities;
using API.Security.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    /// <summary>
    /// Manages customer operations including creation, retrieval, update with concurrency control, and deletion.
    /// </summary>
    [ApiController]
    [Route("api/customer")]
    public class CustomerController(
        ICustomerDataService customerDataService,
        ICustomerBusinessRules customerBusinessRules,
        IMapper mapper,
        ICustomerSecurityService customerSecurityService) : ControllerBase
    {
        private readonly ICustomerDataService _customerDataService = customerDataService ?? throw new ArgumentNullException(nameof(customerDataService));
        private readonly ICustomerBusinessRules _customerBusinessRules = customerBusinessRules ?? throw new ArgumentNullException(nameof(customerBusinessRules));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        private readonly ICustomerSecurityService _customerSecurityService = customerSecurityService ?? throw new ArgumentNullException(nameof(customerSecurityService));

        [HttpGet("{key:int}", Name = "GetCustomerByKey")]
        public async Task<ActionResult<CustomerReadDto>> GetCustomerByKeyAsync(int key)
        {
            var customer = await _customerDataService.GetCustomerByKeyAsync(key);

            if (customer == null)
                return NotFound(new { message = $"Customer with key {key} not found." });

            return Ok(_mapper.Map<CustomerReadDto>(customer));
        }

        [HttpGet("identification/{identification}")]
        public async Task<ActionResult<CustomerReadDto>> GetCustomerByIdentificationAsync(string identification)
        {
            var customer = await _customerDataService.GetCustomerByIdentificationAsync(identification);

            if (customer == null)
                return NotFound(new { message = $"Customer with identification {identification} not found." });

            return Ok(_mapper.Map<CustomerReadDto>(customer));
        }

        [HttpGet("email/{email}")]
        public async Task<ActionResult<CustomerReadDto>> GetCustomerByEmailAsync(string email)
        {
            var customer = await _customerDataService.GetCustomerByEmailAsync(email);

            if (customer == null)
                return NotFound(new { message = $"Customer with email {email} not found." });

            return Ok(_mapper.Map<CustomerReadDto>(customer));
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<CustomerReadDto>>> GetAllCustomersAsync()
        {
            var customers = await _customerDataService.GetAllCustomersAsync();

            if (customers == null || customers.Count == 0)
                return NotFound(new { message = "Customers not found." });

            return Ok(_mapper.Map<List<CustomerReadDto>>(customers));
        }

        /// <summary>
        /// Real conflict: Simplified constructor in C# 12 and endpoints in ASP.NET Core 8/7.
        /// Your controller uses the new C# simplified constructor syntax, which is perfectly valid.
        /// However, in some versions of ASP.NET Core (especially 7 and 8), when combined with CreatedAtAction(...),
        /// the system may fail to correctly assign ActionDescriptor.RouteValues["action"],
        /// which breaks response routing.
        /// To resolve this, you have two options:
        /// 1. Use the full classic constructor instead of the simplified one.
        /// 2. Manually specify the action name in the [HttpGet] attribute using Name = "...", like: [HttpGet("{key:int}", Name = "GetCustomerByKey")],
        ///     and return with CreatedAtRoute like: return CreatedAtRoute("GetCustomerByKey", new { key = customer.Key }, readDto);
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<CustomerReadDto>> AddCustomerAsync(CustomerCreateDto customerCreateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (Success, ErrorMessage) = await _customerBusinessRules.ValidateNewCustomerAsync(customerCreateDto);
            if (!Success)
                return BadRequest(new { message = ErrorMessage });

            var customer = _mapper.Map<Customer>(customerCreateDto);

            customer.Password = _customerSecurityService.HashPassword(customer, customerCreateDto.Password);

            await _customerDataService.AddCustomerAsync(customer);

            var customerReadDto = _mapper.Map<CustomerReadDto>(customer);

            return CreatedAtRoute("GetCustomerByKey", new { key = customer.Key }, customerReadDto);

        }

        [HttpPut("{key:int}")]
        public async Task<ActionResult> UpdateCustomerAsync(int key, CustomerUpdateDto customerUpdateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (key != customerUpdateDto.Key)
                return BadRequest(new { message = "Customer key mismatch." });

            var existingCustomer = await _customerDataService.GetCustomerByKeyAsync(key);
            if (existingCustomer == null)
                return NotFound(new { message = $"Customer with key {key} not found." });

            var (Success, ErrorMessage) = await _customerBusinessRules.ValidateUpdatedCustomerAsync(customerUpdateDto);
            if (!Success)
                return BadRequest(new { message = ErrorMessage });

            // This is a mapping over an existing instance.
            _mapper.Map(customerUpdateDto, existingCustomer);

            if (_customerSecurityService.NeedsRehash(existingCustomer, customerUpdateDto.Password))
                existingCustomer.Password = _customerSecurityService.HashPassword(existingCustomer, customerUpdateDto.Password);

            // This is necessary to handle an expected situation.
            try
            {
                await _customerDataService.UpdateCustomerAsync(existingCustomer);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict(new { message = "The customer was modified by another user. Please reload and try again." });
            }

            return NoContent();
        }

        [HttpDelete("{key:int}")]
        public async Task<ActionResult> DeleteCustomerAsync(int key)
        {
            if (await _customerDataService.GetCustomerByKeyAsync(key) is null)
                return NotFound(new { message = $"Customer with key {key} not found." });

            await _customerDataService.DeleteCustomerAsync(key);

            return NoContent();
        }
    }
}