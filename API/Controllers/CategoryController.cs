using API.BusinessRules.Interfaces;
using API.DataServices.Interfaces;
using API.Dtos.CategoryDtos;
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
    /// Manages category operations including creation, retrieval, update with concurrency control, and deletion.
    /// </summary>
    [ApiController]
    [Route("api/category")]
    public class CategoryController(
    ICategoryDataService categoryDataService,
    ICategoryBusinessRules categoryBusinessRules,
    IMapper mapper) : ControllerBase
    {
        private readonly ICategoryDataService _categoryDataService = categoryDataService ?? throw new ArgumentNullException(nameof(categoryDataService));
        private readonly ICategoryBusinessRules _categoryBusinessRules = categoryBusinessRules ?? throw new ArgumentNullException(nameof(categoryBusinessRules));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        [HttpGet("{key:int}", Name = "GetCategoryByKeyAsync")]
        public async Task<ActionResult<CategoryReadDto>> GetCategoryByKeyAsync(int key)
        {
            var category = await _categoryDataService.GetCategoryByKeyAsync(key);

            if (category == null)
                return NotFound(new { message = $"Category with key {key} not found." });

            return Ok(_mapper.Map<CategoryReadDto>(category));
        }

        [HttpGet("name/{name}")]
        public async Task<ActionResult<CategoryReadDto>> GetCategoryByNameAsync(string name)
        {
            var category = await _categoryDataService.GetCategoryByNameAsync(name);

            if (category == null)
                return NotFound(new { message = $"Category with name {name} not found." });

            return Ok(_mapper.Map<CategoryReadDto>(category));
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<CategoryReadDto>>> GetAllCategoriesAsync()
        {
            var categories = await _categoryDataService.GetAllCategoriesAsync();

            if (categories == null || categories.Count == 0)
                return NotFound(new { message = "Categories not found." });

            return Ok(_mapper.Map<List<CategoryReadDto>>(categories));
        }

        /// <summary>
        /// Real conflict: Simplified constructor in C# 12 and endpoints in ASP.NET Core 8/7.
        /// Your controller uses the new C# simplified constructor syntax, which is perfectly valid.
        /// However, in some versions of ASP.NET Core (especially 7 and 8), when combined with CreatedAtAction(...),
        /// the system may fail to correctly assign ActionDescriptor.RouteValues["action"],
        /// which breaks response routing.
        /// To resolve this, you have two options:
        /// 1. Use the full classic constructor instead of the simplified one.
        /// 2. Manually specify the action name in the [HttpGet] attribute using Name = "...", like: [HttpGet("{key:int}", Name = "GetCategoryByKeyAsync")],
        ///     and return with CreatedAtRoute like: return CreatedAtRoute("GetCategoryByKeyAsync", new { key = user.Key }, categoryReadDto);
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<CategoryReadDto>> AddCategoryAsync(CategoryCreateDto categoryCreateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (Success, ErrorMessage) = await _categoryBusinessRules.ValidateNewCategoryAsync(categoryCreateDto);
            if (!Success)
                return BadRequest(new { message = ErrorMessage });

            var category = _mapper.Map<Category>(categoryCreateDto);

            await _categoryDataService.AddCategoryAsync(category);

            var categoryReadDto = _mapper.Map<CategoryReadDto>(category);

            return CreatedAtRoute("GetCategoryByKeyAsync", new { key = category.Key }, categoryReadDto);

        }

        [HttpPut("{key:int}")]
        public async Task<ActionResult> UpdateCategoryAsync(int key, CategoryUpdateDto categoryUpdateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (key != categoryUpdateDto.Key)
                return BadRequest(new { message = "Category key mismatch." });

            var existingCategory = await _categoryDataService.GetCategoryByKeyAsync(key);
            if (existingCategory == null)
                return NotFound(new { message = $"Category with key {key} not found." });

            var (Success, ErrorMessage) = await _categoryBusinessRules.ValidateUpdatedCategoryAsync(categoryUpdateDto);
            if (!Success)
                return BadRequest(new { message = ErrorMessage });

            // This is a mapping over an existing instance.
            _mapper.Map(categoryUpdateDto, existingCategory);

            // This is necessary to handle an expected situation.
            try
            {
                await _categoryDataService.UpdateCategoryAsync(existingCategory);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict(new { message = "The category was modified by another user. Please reload and try again." });
            }

            return NoContent();
        }

        [HttpDelete("{key:int}")]
        public async Task<ActionResult> DeleteCategoryAsync(int key)
        {
            if (await _categoryDataService.GetCategoryByKeyAsync(key) is null)
                return NotFound(new { message = $"Category with key {key} not found." });

            await _categoryDataService.DeleteCategoryAsync(key);

            return NoContent();
        }
    }
}
