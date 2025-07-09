using API.BusinessRules.Interfaces;
using API.Controllers;
using API.DataServices.Interfaces;
using API.Dtos.CategoryDtos;
using API.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace APITests.Controllers
{
    /// <summary>
    /// Unit tests for CategoryController, covering category management operations such as: 
    /// creation, retrieval, update with concurrency control, and deletion. 
    /// </summary>
    [TestClass()]
    public class CategoryControllerUnitTests
    {
        private Mock<ICategoryDataService> _mockCategoryDataService = null!;
        private Mock<ICategoryBusinessRules> _mockCategoryBusinessRules = null!;
        private Mock<IMapper> _mockMapper = null!;
        private CategoryController _categoryController = null!;

        [TestInitialize]
        public void SetUp()
        {
            _mockCategoryDataService = new Mock<ICategoryDataService>();
            _mockCategoryBusinessRules = new Mock<ICategoryBusinessRules>();
            _mockMapper = new Mock<IMapper>();
            _categoryController = new CategoryController(
                _mockCategoryDataService.Object,
                _mockCategoryBusinessRules.Object,
                _mockMapper.Object);
        }

        #region Helpers

        private static Category GetSampleCategory(int id = 1, string name = "Tech")
        {
            return new Category
            {
                Key = id,
                Name = name,
                CreationDate = DateTime.UtcNow,
                ModificationDate = DateTime.UtcNow,
                CreatedBy = 0,
                ModifiedBy = 0,
                RowVersion = 0
            };
        }

        private static CategoryCreateDto GetSampleCategoryCreateDto(string name = "Tech")
        {
            return new CategoryCreateDto
            {
                Name = name,
                CreatedBy = 0
            };
        }

        private static CategoryUpdateDto GetSampleCategoryUpdateDto(int id = 1, string name = "Tech")
        {
            return new CategoryUpdateDto
            {
                Key = id,
                Name = name,
                ModifiedBy = 0,
                RowVersion = 0
            };
        }

        private static CategoryReadDto GetSampleCategoryReadDto(int id = 1, string name = "Tech")
        {
            return new CategoryReadDto
            {
                Key = id,
                Name = name,
                CreationDate = DateTime.UtcNow,
                ModificationDate = DateTime.UtcNow,
                CreatedBy = 0,
                ModifiedBy = 0,
                RowVersion = 0
            };
        }

        #endregion

        #region GetCategoryByKeyAsync

        /// <summary>
        /// The GetCategoryByKeyAsync method in CategoryController returns a Task<ActionResult<CategoryReadDto>>,
        /// which is a wrapper (ActionResult<T>). To access the underlying OkObjectResult,
        /// you need to use `.Result` like this: `result.Result as OkObjectResult`.
        /// </summary>
        [TestMethod]
        public async Task GetCategoryByKeyAsync_ReturnsOkWithCategory()
        {
            // Arrange
            var category = GetSampleCategory();
            var categoryReadDto = GetSampleCategoryReadDto();

            _mockCategoryDataService
                .Setup(s => s.GetCategoryByKeyAsync(category.Key))
                .ReturnsAsync(category);

            _mockMapper.
                Setup(m => m.Map<CategoryReadDto>(category))
                .Returns(categoryReadDto);

            // Act
            var result = await _categoryController.GetCategoryByKeyAsync(category.Key);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(categoryReadDto, okResult.Value);

            _mockCategoryDataService.Verify(s => s.GetCategoryByKeyAsync(category.Key), Times.Once);
        }

        [TestMethod]
        public async Task GetCategoryByKeyAsync_ReturnsNotFound_WhenCategoryNotFound()
        {
            // Arrange
            int key = 0; // Non-existent key

            _mockCategoryDataService
                .Setup(s => s.GetCategoryByKeyAsync(key))
                .ReturnsAsync((Category?)null);

            // Act
            var result = await _categoryController.GetCategoryByKeyAsync(key);

            // Assert
            var notFound = result.Result as NotFoundObjectResult;
            Assert.IsNotNull(notFound);

            _mockCategoryDataService.Verify(s => s.GetCategoryByKeyAsync(key), Times.Once);
        }

        #endregion

        #region GetCategoryByNameAsync

        /// <summary>
        /// The GetCategoryByNameAsync method in CategoryController returns a Task<ActionResult<CategoryReadDto>>,
        /// which is a wrapper (ActionResult<T>). To access the underlying OkObjectResult,
        /// you need to use `.Result` like this: `result.Result as OkObjectResult`.
        /// </summary>
        [TestMethod]
        public async Task GetCategoryByNameAsync_ReturnsOkWithCategory()
        {
            // Arrange
            var category = GetSampleCategory();
            var categoryReadDto = GetSampleCategoryReadDto();

            _mockCategoryDataService
                .Setup(s => s.GetCategoryByNameAsync(category.Name))
                .ReturnsAsync(category);

            _mockMapper
                .Setup(m => m.Map<CategoryReadDto>(category))
                .Returns(categoryReadDto);

            // Act
            var result = await _categoryController.GetCategoryByNameAsync(category.Name);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(categoryReadDto, okResult.Value);
            
            _mockCategoryDataService.Verify(s => s.GetCategoryByNameAsync(category.Name), Times.Once);
        }

        [TestMethod]
        public async Task GetCategoryByNameAsync_ReturnsNotFound_WhenCategoryNotFound()
        {
            // Arrange
            string name = "Unknown"; // Non-existent name

            _mockCategoryDataService
                .Setup(s => s.GetCategoryByNameAsync(name))
                .ReturnsAsync((Category?)null);

            // Act
            var result = await _categoryController.GetCategoryByNameAsync(name);

            // Assert
            var notFound = result.Result as NotFoundObjectResult;
            Assert.IsNotNull(notFound);
            
            _mockCategoryDataService.Verify(s => s.GetCategoryByNameAsync(name), Times.Once);
        }

        #endregion

        #region GetAllCategoriesAsync

        [TestMethod]
        public async Task GetAllCategoriesAsync_ReturnsOkWithCategories()
        {
            // Arrange
            var categories = new List<Category> { GetSampleCategory() };
            var categoryReadDtos = new List<CategoryReadDto> { GetSampleCategoryReadDto() };

            _mockCategoryDataService
                .Setup(s => s.GetAllCategoriesAsync())
                .ReturnsAsync(categories);

            _mockMapper
                .Setup(m => m.Map<List<CategoryReadDto>>(categories))
                .Returns(categoryReadDtos);

            // Act
            var result = await _categoryController.GetAllCategoriesAsync();

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            CollectionAssert.AreEqual(categoryReadDtos, (List<CategoryReadDto>)okResult.Value!);

            _mockCategoryDataService.Verify(s => s.GetAllCategoriesAsync(), Times.Once);
        }

        [TestMethod]
        public async Task GetAllCategoriesAsync_ReturnsNotFound_WhenCategoriesNotFound()
        {
            // Arrange
            var categories = new List<Category>();
            var categoryReadDtos = new List<CategoryReadDto>();

            _mockCategoryDataService
                .Setup(s => s.GetAllCategoriesAsync())
                .ReturnsAsync(categories);

            _mockMapper
                .Setup(m => m.Map<List<CategoryReadDto>>(categories))
                .Returns(categoryReadDtos);

            // Act
            var result = await _categoryController.GetAllCategoriesAsync();

            // Assert
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);

            _mockCategoryDataService.Verify(s => s.GetAllCategoriesAsync(), Times.Once);
        }

        #endregion

        #region AddCategoryAsync

        [TestMethod]
        public async Task AddCategoryAsync_ReturnsCreatedAtRoute_WhenCategoryAdded()
        {
            // Arrange
            var categoryCreateDto = GetSampleCategoryCreateDto();
            var category = GetSampleCategory();
            var categoryReadDto = GetSampleCategoryReadDto();

            _mockCategoryBusinessRules
                .Setup(r => r.ValidateNewCategoryAsync(categoryCreateDto))
                .ReturnsAsync((true, null));

            _mockMapper
                .Setup(m => m.Map<Category>(categoryCreateDto))
                .Returns(category);

            _mockCategoryDataService
                .Setup(s => s.AddCategoryAsync(category))
                .Returns(Task.CompletedTask);

            _mockMapper
                .Setup(m => m.Map<CategoryReadDto>(category))
                .Returns(categoryReadDto);

            // Act
            var result = await _categoryController.AddCategoryAsync(categoryCreateDto);

            // Assert
            var createdAtRoute = result.Result as CreatedAtRouteResult;
            Assert.IsNotNull(createdAtRoute);
            Assert.AreEqual("GetCategoryByKeyAsync", createdAtRoute.RouteName);
            Assert.AreEqual(categoryReadDto, createdAtRoute.Value);

            _mockCategoryBusinessRules.Verify(r => r.ValidateNewCategoryAsync(categoryCreateDto), Times.Once);
            _mockCategoryDataService.Verify(s => s.AddCategoryAsync(category), Times.Once);

        }

        [TestMethod]
        public async Task AddCategoryAsync_ReturnsBadRequest_WhenModelStateInvalid()
        {
            // Arrange
            _categoryController.ModelState.AddModelError("Name", "Required");
            var categoryCreateDto = GetSampleCategoryCreateDto();

            // Act
            var result = await _categoryController.AddCategoryAsync(categoryCreateDto);

            // Assert
            var badRequest = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);

            // It's fine not to call .Verify() because you don't expect the service to be called if the ModelState is invalid.
        }

        [TestMethod]
        public async Task AddCategoryAsync_ReturnsBadRequest_WhenBusinessRulesFailed()
        {
            // Arrange
            var categoryCreateDto = GetSampleCategoryCreateDto();

            _mockCategoryBusinessRules
                .Setup(r => r.ValidateNewCategoryAsync(categoryCreateDto))
                .ReturnsAsync((false, "Name is already in use."));

            // Act
            var result = await _categoryController.AddCategoryAsync(categoryCreateDto);

            // Assert
            var badRequest = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
            Assert.IsTrue(badRequest.Value?.ToString()?.Contains("Name is already in use.") ?? false); // Verifies that the error message returned in the BadRequest contains the expected text.

            _mockCategoryBusinessRules.Verify(r => r.ValidateNewCategoryAsync(categoryCreateDto), Times.Once);
            _mockCategoryDataService.VerifyNoOtherCalls(); // Verifies that no other methods on _mockCategoryDataService were called, not even AddCategoryAsync.
        }

        #endregion

        #region UpdateCategoryAsync

        [TestMethod]
        public async Task UpdateCategoryAsync_ReturnsNoContent_WhenCategoryUpdated()
        {
            // Arrange
            var categoryUpdateDto = GetSampleCategoryUpdateDto();
            var category = GetSampleCategory();

            _mockCategoryDataService
                .Setup(s => s.GetCategoryByKeyAsync(categoryUpdateDto.Key))
                .ReturnsAsync(category);

            _mockCategoryBusinessRules
                .Setup(r => r.ValidateUpdatedCategoryAsync(categoryUpdateDto))
                .ReturnsAsync((true, null));

            _mockMapper
                .Setup(m => m.Map(categoryUpdateDto, category));

            _mockCategoryDataService
                .Setup(s => s.UpdateCategoryAsync(category))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _categoryController.UpdateCategoryAsync(categoryUpdateDto.Key, categoryUpdateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task UpdateCategoryAsync_ReturnsBadRequest_WhenModelStateInvalid()
        {
            // Arrange
            var categoryUpdateDto = GetSampleCategoryUpdateDto();
            _categoryController.ModelState.AddModelError("Name", "Required");

            // Act
            var result = await _categoryController.UpdateCategoryAsync(categoryUpdateDto.Key, categoryUpdateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task UpdateCategoryAsync_ReturnsBadRequest_WhenKeyMismatch()
        {
            // Arrange
            var categoryUpdateDto = GetSampleCategoryUpdateDto();
            int differentKey = categoryUpdateDto.Key + 1;

            // Act
            var result = await _categoryController.UpdateCategoryAsync(differentKey, categoryUpdateDto);

            // Assert
            var badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
            StringAssert.Contains(badRequest.Value?.ToString(), "Category key mismatch");
        }

        [TestMethod]
        public async Task UpdateCategoryAsync_ReturnsNotFound_WhenCategoryNotFound()
        {
            // Arrange
            var categoryUpdateDto = GetSampleCategoryUpdateDto();

            _mockCategoryDataService
                .Setup(s => s.GetCategoryByKeyAsync(categoryUpdateDto.Key))
                .ReturnsAsync((Category?)null);

            // Act
            var result = await _categoryController.UpdateCategoryAsync(categoryUpdateDto.Key, categoryUpdateDto);

            // Assert
            var notFound = result as NotFoundObjectResult;
            Assert.IsNotNull(notFound);
        }

        [TestMethod]
        public async Task UpdateCategoryAsync_ReturnsBadRequest_WhenBusinessRulesFailed()
        {
            // Arrange
            var categoryUpdateDto = GetSampleCategoryUpdateDto();
            var category = GetSampleCategory();

            _mockCategoryDataService
                .Setup(s => s.GetCategoryByKeyAsync(categoryUpdateDto.Key))
                .ReturnsAsync(category);

            _mockCategoryBusinessRules
                .Setup(r => r.ValidateUpdatedCategoryAsync(categoryUpdateDto))
                .ReturnsAsync((false, "Name is already in use."));

            // Act
            var result = await _categoryController.UpdateCategoryAsync(categoryUpdateDto.Key, categoryUpdateDto);

            // Assert
            var badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
            StringAssert.Contains(badRequest.Value?.ToString(), "Name is already in use.");
        }

        [TestMethod]
        public async Task UpdateCategoryAsync_ReturnsConflict_WhenConcurrencyExceptionThrown()
        {
            // Arrange
            var categoryUpdateDto = GetSampleCategoryUpdateDto();
            var category = GetSampleCategory();

            _mockCategoryDataService
                .Setup(s => s.GetCategoryByKeyAsync(categoryUpdateDto.Key))
                .ReturnsAsync(category);

            _mockCategoryBusinessRules
                .Setup(r => r.ValidateUpdatedCategoryAsync(categoryUpdateDto))
                .ReturnsAsync((true, null));

            _mockMapper
                .Setup(m => m.Map(categoryUpdateDto, category));

            _mockCategoryDataService
                .Setup(s => s.UpdateCategoryAsync(category))
                .ThrowsAsync(new DbUpdateConcurrencyException());

            // Act
            var result = await _categoryController.UpdateCategoryAsync(categoryUpdateDto.Key, categoryUpdateDto);

            // Assert
            var conflict = result as ConflictObjectResult;
            Assert.IsNotNull(conflict);
            StringAssert.Contains(conflict.Value?.ToString(), "The category was modified by another user.");
        }

        #endregion

        #region DeleteCategoryAsync

        [TestMethod]
        public async Task DeleteCategoryAsync_ReturnsNotFound_WhenCategoryNotFound()
        {
            // Arrange
            int key = 123;

            _mockCategoryDataService
                .Setup(s => s.GetCategoryByKeyAsync(key))
                .ReturnsAsync((Category?)null);

            // Act
            var result = await _categoryController.DeleteCategoryAsync(key);

            // Assert
            var notFound = result as NotFoundObjectResult;
            Assert.IsNotNull(notFound);
        }

        [TestMethod]
        public async Task DeleteCategoryAsync_ReturnsNoContent_WhenCategoryDeleted()
        {
            // Arrange
            var category = GetSampleCategory();

            _mockCategoryDataService
                .Setup(s => s.GetCategoryByKeyAsync(category.Key))
                .ReturnsAsync(category);

            _mockCategoryDataService
                .Setup(s => s.DeleteCategoryAsync(category.Key))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _categoryController.DeleteCategoryAsync(category.Key);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));

            _mockCategoryDataService.Verify(s => s.DeleteCategoryAsync(category.Key), Times.Once);
        }

        #endregion
    }
}
