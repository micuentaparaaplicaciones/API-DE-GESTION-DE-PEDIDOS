using API.BusinessRules.Interfaces;
using API.DataServices.Interfaces;
using API.Dtos.SupplierDtos;
using API.Dtos.UserDtos;
using API.Entities;
using API.Security;
using API.Security.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    /// <summary>
    /// Manages supplier operations including creation, retrieval, update with concurrency control, and deletion.
    /// </summary>
    [ApiController]
    [Route("api/supplier")]
    public class SupplierController(
        ISupplierDataService supplierDataService,
        ISupplierBusinessRules supplierBusinessRules,
        IMapper mapper) : ControllerBase
    {
        private readonly ISupplierDataService _supplierDataService = supplierDataService ?? throw new ArgumentNullException(nameof(supplierDataService));
        private readonly ISupplierBusinessRules _supplierBusinessRules = supplierBusinessRules ?? throw new ArgumentNullException(nameof(supplierBusinessRules));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        [HttpGet("{key:int}", Name = "GetSupplierByKeyAsync")]
        public async Task<ActionResult<SupplierReadDto>> GetSupplierByKeyAsync(int key)
        {
            var supplier = await _supplierDataService.GetSupplierByKeyAsync(key);

            if (supplier == null)
                return NotFound(new { message = $"Supplier with key {key} not found." });

            return Ok(_mapper.Map<SupplierReadDto>(supplier));
        }

        [HttpGet("name/{name}")]
        public async Task<ActionResult<SupplierReadDto>> GetSupplierByNameAsync(string name)
        {
            var supplier = await _supplierDataService.GetSupplierByNameAsync(name);

            if (supplier == null)
                return NotFound(new { message = $"Supplier with name {name} not found." });

            return Ok(_mapper.Map<SupplierReadDto>(supplier));
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<SupplierReadDto>>> GetAllSuppliersAsync()
        {
            var suppliers = await _supplierDataService.GetAllSuppliersAsync();

            if (suppliers == null || suppliers.Count == 0)
                return NotFound(new { message = "Suppliers not found." });

            return Ok(_mapper.Map<List<SupplierReadDto>>(suppliers));
        }

        /// <summary>
        /// Real conflict: Simplified constructor in C# 12 and endpoints in ASP.NET Core 8/7.
        /// Your controller uses the new C# simplified constructor syntax, which is perfectly valid.
        /// However, in some versions of ASP.NET Core (especially 7 and 8), when combined with CreatedAtAction(...),
        /// the system may fail to correctly assign ActionDescriptor.RouteValues["action"],
        /// which breaks response routing.
        /// To resolve this, you have two options:
        /// 1. Use the full classic constructor instead of the simplified one.
        /// 2. Manually specify the action name in the [HttpGet] attribute using Name = "...", like: [HttpGet("{key:int}", Name = "GetSupplierByKeyAsync")],
        ///     and return with CreatedAtRoute like: return CreatedAtRoute("GetSupplierByKeyAsync", new { key = supplier.Key }, supplierReadDto);
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<SupplierReadDto>> AddSupplierAsync(SupplierCreateDto supplierCreateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (Success, ErrorMessage) = await _supplierBusinessRules.ValidateNewSupplierAsync(supplierCreateDto);
            if (!Success)
                return BadRequest(new { message = ErrorMessage });

            var supplier = _mapper.Map<Supplier>(supplierCreateDto);

            await _supplierDataService.AddSupplierAsync(supplier);

            var supplierReadDto = _mapper.Map<SupplierReadDto>(supplier);

            return CreatedAtRoute("GetSupplierByKeyAsync", new { key = supplier.Key }, supplierReadDto);
        }

        [HttpPut("{key:int}")]
        public async Task<ActionResult> UpdateSupplierAsync(int key, SupplierUpdateDto supplierUpdateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (key != supplierUpdateDto.Key)
                return BadRequest(new { message = "Supplier key mismatch." });

            var existingSupplier = await _supplierDataService.GetSupplierByKeyAsync(key);
            if (existingSupplier == null)
                return NotFound(new { message = $"Supplier with key {key} not found." });

            var (Success, ErrorMessage) = await _supplierBusinessRules.ValidateUpdatedSupplierAsync(supplierUpdateDto);
            if (!Success)
                return BadRequest(new { message = ErrorMessage });

            // This is a mapping over an existing instance.
            _mapper.Map(supplierUpdateDto, existingSupplier);

            // This is necessary to handle an expected situation.
            try
            {
                await _supplierDataService.UpdateSupplierAsync(existingSupplier);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict(new { message = "The supplier was modified by another user. Please reload and try again." });
            }

            return NoContent();
        }

        [HttpDelete("{key:int}")]
        public async Task<ActionResult> DeleteSupplierAsync(int key)
        {
            if (await _supplierDataService.GetSupplierByKeyAsync(key) is null)
                return NotFound(new { message = $"Supplier with key {key} not found." });

            await _supplierDataService.DeleteSupplierAsync(key);

            return NoContent();
        }
    }
}
