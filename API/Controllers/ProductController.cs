using API.BusinessRules.Interfaces;
using API.DataServices.Interfaces;
using API.Dtos.ProductDtos;
using API.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    /// <summary>
    /// Manages product operations including creation, retrieval, update with concurrency control, and deletion.
    /// </summary>
    [ApiController]
    [Route("api/product")]
    public class ProductController(
        IProductDataService productDataService,
        IProductBusinessRules productBusinessRules,
        IMapper mapper) : ControllerBase
    {
        private readonly IProductDataService _productDataService = productDataService ?? throw new ArgumentNullException(nameof(productDataService));
        private readonly IProductBusinessRules _productBusinessRules = productBusinessRules ?? throw new ArgumentNullException(nameof(productBusinessRules));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        [HttpGet("{key:int}", Name = "GetProductByKeyAsync")]
        public async Task<ActionResult<ProductReadDto>> GetProductByKeyAsync(int key)
        {
            var product = await _productDataService.GetProductByKeyAsync(key);

            if (product == null)
                return NotFound(new { message = $"Product with key {key} not found." });

            return Ok(_mapper.Map<ProductReadDto>(product));
        }

        [HttpGet("name/{name}")]
        public async Task<ActionResult<ProductReadDto>> GetProductByNameAsync(string name)
        {
            var product = await _productDataService.GetProductByNameAsync(name);

            if (product == null)
                return NotFound(new { message = $"Product with name {name} not found." });

            return Ok(_mapper.Map<ProductReadDto>(product));
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<ProductReadDto>>> GetAllProductsAsync()
        {
            var products = await _productDataService.GetAllProductsAsync();

            if (products == null || products.Count == 0)
                return NotFound(new { message = "Products not found." });

            return Ok(_mapper.Map<List<ProductReadDto>>(products));
        }

        /// <summary>
        /// Real conflict: Simplified constructor in C# 12 and endpoints in ASP.NET Core 8/7.
        /// Your controller uses the new C# simplified constructor syntax, which is perfectly valid.
        /// However, in some versions of ASP.NET Core (especially 7 and 8), when combined with CreatedAtAction(...),
        /// the system may fail to correctly assign ActionDescriptor.RouteValues["action"],
        /// which breaks response routing.
        /// To resolve this, you have two options:
        /// 1. Use the full classic constructor instead of the simplified one.
        /// 2. Manually specify the action name in the [HttpGet] attribute using Name = "...", like: [HttpGet("{key:int}", Name = "GetProductByKeyAsync")],
        ///     and return with CreatedAtRoute like: return CreatedAtRoute("GetProductByKeyAsync", new { key = user.Key }, productReadDto);
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ProductReadDto>> AddProductAsync(ProductCreateDto productCreateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (Success, ErrorMessage) = await _productBusinessRules.ValidateNewProductAsync(productCreateDto);
            if (!Success)
                return BadRequest(new { message = ErrorMessage });

            var product = _mapper.Map<Product>(productCreateDto);

            await _productDataService.AddProductAsync(product);

            var productReadDto = _mapper.Map<ProductReadDto>(product);

            return CreatedAtRoute("GetProductByKeyAsync", new { key = product.Key }, productReadDto);
        }

        [HttpPut("{key:int}")]
        public async Task<ActionResult> UpdateProductAsync(int key, ProductUpdateDto productUpdateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (key != productUpdateDto.Key)
                return BadRequest(new { message = "Product key mismatch." });

            var existingProduct = await _productDataService.GetProductByKeyAsync(key);
            if (existingProduct == null)
                return NotFound(new { message = $"Product with key {key} not found." });

            var (Success, ErrorMessage) = await _productBusinessRules.ValidateUpdatedProductAsync(productUpdateDto);
            if (!Success)
                return BadRequest(new { message = ErrorMessage });

            // This is a mapping over an existing instance.
            _mapper.Map(productUpdateDto, existingProduct);

            // This is necessary to handle an expected situation.
            try
            {
                await _productDataService.UpdateProductAsync(existingProduct);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict(new { message = "The product was modified by another user. Please reload and try again." });
            }

            return NoContent();
        }

        [HttpDelete("{key:int}")]
        public async Task<ActionResult> DeleteProductAsync(int key)
        {
            if (await _productDataService.GetProductByKeyAsync(key) is null)
                return NotFound(new { message = $"Product with key {key} not found." });

            await _productDataService.DeleteProductAsync(key);

            return NoContent();
        }
    }
}
