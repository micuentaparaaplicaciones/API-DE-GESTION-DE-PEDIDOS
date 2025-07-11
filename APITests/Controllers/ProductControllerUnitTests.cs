using API.BusinessRules.Interfaces;
using API.Controllers;
using API.DataServices.Interfaces;
using API.Dtos.ProductDtos;
using API.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace APITests.Controllers
{
    /// <summary>
    /// Unit tests for ProductController, covering product management operations such as: 
    /// creation, retrieval, update with concurrency control, and deletion. 
    /// </summary>
    [TestClass()]
    public class ProductControllerUnitTests
    {
        private Mock<IProductDataService> _mockProductDataService = null!;
        private Mock<IProductBusinessRules> _mockProductBusinessRules = null!;
        private Mock<IMapper> _mockMapper = null!;
        private ProductController _productController = null!;

        [TestInitialize]
        public void SetUp()
        {
            _mockProductDataService = new Mock<IProductDataService>();
            _mockProductBusinessRules = new Mock<IProductBusinessRules>();
            _mockMapper = new Mock<IMapper>();
            _productController = new ProductController(
                _mockProductDataService.Object,
                _mockProductBusinessRules.Object,
                _mockMapper.Object);
        }

        #region Helpers

        private static Product GetSampleProduct(int id = 1, string name = "Laptop")
        {
            return new Product
            {
                Key = id,
                Image = new byte[] { 1, 2, 3 },
                Name = name,
                Detail = "Sample Detail",
                Price = 1000.50m,
                AvailableQuantity = 10,
                CreatedBy = 1,
                SuppliedBy = 1,
                CategorizedBy = 1,
                CreationDate = DateTime.UtcNow,
                ModificationDate = DateTime.UtcNow,
                ModifiedBy = 1,
                RowVersion = 0
            };
        }

        private static ProductCreateDto GetSampleProductCreateDto(string name = "Laptop")
        {
            return new ProductCreateDto
            {
                Image = new byte[] { 1, 2, 3 },
                Name = name,
                Detail = "Sample Detail",
                Price = 1000.50m,
                AvailableQuantity = 10,
                CreatedBy = 1,
                SuppliedBy = 1,
                CategorizedBy = 1
            };
        }

        private static ProductUpdateDto GetSampleProductUpdateDto(int id = 1, string name = "Laptop")
        {
            return new ProductUpdateDto
            {
                Key = id,
                Image = new byte[] { 1, 2, 3 },
                Name = name,
                Detail = "Updated Detail",
                Price = 1050.75m,
                AvailableQuantity = 15,
                SuppliedBy = 1,
                CategorizedBy = 1,
                ModifiedBy = 2,
                RowVersion = 0
            };
        }

        private static ProductReadDto GetSampleProductReadDto(int id = 1, string name = "Laptop")
        {
            return new ProductReadDto
            {
                Key = id,
                Image = new byte[] { 1, 2, 3 },
                Name = name,
                Detail = "Sample Detail",
                Price = 1000.50m,
                AvailableQuantity = 10,
                CreatedBy = 1,
                SuppliedBy = 1,
                CategorizedBy = 1,
                CreationDate = DateTime.UtcNow,
                ModificationDate = DateTime.UtcNow,
                ModifiedBy = 1,
                RowVersion = 0
            };
        }

        #endregion

        #region GetProductByKeyAsync

        /// <summary>
        /// The GetProductByKeyAsync method in ProductController returns a Task<ActionResult<ProductReadDto>>,
        /// which is a wrapper (ActionResult<T>). To access the underlying OkObjectResult,
        /// you need to use `.Result` like this: `result.Result as OkObjectResult`.
        /// </summary>
        [TestMethod]
        public async Task GetProductByKeyAsync_ReturnsOkWithProduct()
        {
            // Arrange
            var product = GetSampleProduct();
            var productReadDto = GetSampleProductReadDto();

            _mockProductDataService
                .Setup(s => s.GetProductByKeyAsync(product.Key))
                .ReturnsAsync(product);

            _mockMapper
                .Setup(m => m.Map<ProductReadDto>(product))
                .Returns(productReadDto);

            // Act
            var result = await _productController.GetProductByKeyAsync(product.Key);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(productReadDto, okResult.Value);

            _mockProductDataService.Verify(s => s.GetProductByKeyAsync(product.Key), Times.Once);
        }

        [TestMethod]
        public async Task GetProductByKeyAsync_ReturnsNotFound_WhenProductNotFound()
        {
            // Arrange
            int key = 0; // Non-existent key

            _mockProductDataService
                .Setup(s => s.GetProductByKeyAsync(key))
                .ReturnsAsync((Product?)null);

            // Act
            var result = await _productController.GetProductByKeyAsync(key);

            // Assert
            var notFound = result.Result as NotFoundObjectResult;
            Assert.IsNotNull(notFound);

            _mockProductDataService.Verify(s => s.GetProductByKeyAsync(key), Times.Once);
        }

        #endregion

        #region GetProductByNameAsync

        /// <summary>
        /// The GetProductByNameAsync method in ProductController returns a Task<ActionResult<ProductReadDto>>,
        /// which is a wrapper (ActionResult<T>). To access the underlying OkObjectResult,
        /// you need to use `.Result` like this: `result.Result as OkObjectResult`.
        /// </summary>
        [TestMethod]
        public async Task GetProductByNameAsync_ReturnsOkWithProduct()
        {
            // Arrange
            var product = GetSampleProduct();
            var productReadDto = GetSampleProductReadDto();

            _mockProductDataService
                .Setup(s => s.GetProductByNameAsync(product.Name))
                .ReturnsAsync(product);

            _mockMapper
                .Setup(m => m.Map<ProductReadDto>(product))
                .Returns(productReadDto);

            // Act
            var result = await _productController.GetProductByNameAsync(product.Name);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(productReadDto, okResult.Value);

            _mockProductDataService.Verify(s => s.GetProductByNameAsync(product.Name), Times.Once);
        }

        [TestMethod]
        public async Task GetProductByNameAsync_ReturnsNotFound_WhenProductNotFound()
        {
            // Arrange
            string name = "Unknown"; // Non-existent name

            _mockProductDataService
                .Setup(s => s.GetProductByNameAsync(name))
                .ReturnsAsync((Product?)null);

            // Act
            var result = await _productController.GetProductByNameAsync(name);

            // Assert
            var notFound = result.Result as NotFoundObjectResult;
            Assert.IsNotNull(notFound);

            _mockProductDataService.Verify(s => s.GetProductByNameAsync(name), Times.Once);
        }

        #endregion

        #region GetAllProductsAsync

        [TestMethod]
        public async Task GetAllProductsAsync_ReturnsOkWithProducts()
        {
            // Arrange
            var products = new List<Product> { GetSampleProduct() };
            var productReadDtos = new List<ProductReadDto> { GetSampleProductReadDto() };

            _mockProductDataService
                .Setup(s => s.GetAllProductsAsync())
                .ReturnsAsync(products);

            _mockMapper
                .Setup(m => m.Map<List<ProductReadDto>>(products))
                .Returns(productReadDtos);

            // Act
            var result = await _productController.GetAllProductsAsync();

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            CollectionAssert.AreEqual(productReadDtos, (List<ProductReadDto>)okResult.Value!);

            _mockProductDataService.Verify(s => s.GetAllProductsAsync(), Times.Once);
        }

        [TestMethod]
        public async Task GetAllProductsAsync_ReturnsNotFound_WhenProductsNotFound()
        {
            // Arrange
            var products = new List<Product>();
            var productReadDtos = new List<ProductReadDto>();

            _mockProductDataService
                .Setup(s => s.GetAllProductsAsync())
                .ReturnsAsync(products);

            _mockMapper
                .Setup(m => m.Map<List<ProductReadDto>>(products))
                .Returns(productReadDtos);

            // Act
            var result = await _productController.GetAllProductsAsync();

            // Assert
            var notFound = result.Result as NotFoundObjectResult;
            Assert.IsNotNull(notFound);

            _mockProductDataService.Verify(s => s.GetAllProductsAsync(), Times.Once);
        }

        #endregion

        #region AddProductAsync

        [TestMethod]
        public async Task AddProductAsync_ReturnsCreatedAtRoute_WhenProductAdded()
        {
            // Arrange
            var productCreateDto = GetSampleProductCreateDto();
            var product = GetSampleProduct();
            var productReadDto = GetSampleProductReadDto();

            _mockProductBusinessRules
                .Setup(r => r.ValidateNewProductAsync(productCreateDto))
                .ReturnsAsync((true, null));

            _mockMapper
                .Setup(m => m.Map<Product>(productCreateDto))
                .Returns(product);

            _mockProductDataService
                .Setup(s => s.AddProductAsync(product))
                .Returns(Task.CompletedTask);

            _mockMapper
                .Setup(m => m.Map<ProductReadDto>(product))
                .Returns(productReadDto);

            // Act
            var result = await _productController.AddProductAsync(productCreateDto);

            // Assert
            var createdAtRoute = result.Result as CreatedAtRouteResult;
            Assert.IsNotNull(createdAtRoute);
            Assert.AreEqual("GetProductByKeyAsync", createdAtRoute.RouteName);
            Assert.AreEqual(productReadDto, createdAtRoute.Value);

            _mockProductBusinessRules.Verify(r => r.ValidateNewProductAsync(productCreateDto), Times.Once);
            _mockProductDataService.Verify(s => s.AddProductAsync(product), Times.Once);
        }

        [TestMethod]
        public async Task AddProductAsync_ReturnsBadRequest_WhenModelStateInvalid()
        {
            // Arrange
            _productController.ModelState.AddModelError("Name", "Required");
            var productCreateDto = GetSampleProductCreateDto();

            // Act
            var result = await _productController.AddProductAsync(productCreateDto);

            // Assert
            var badRequest = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);

            // It's fine not to call .Verify() because you don't expect the service to be called if the ModelState is invalid.
        }

        [TestMethod]
        public async Task AddProductAsync_ReturnsBadRequest_WhenBusinessRulesFailed()
        {
            // Arrange
            var productCreateDto = GetSampleProductCreateDto();

            _mockProductBusinessRules
                .Setup(r => r.ValidateNewProductAsync(productCreateDto))
                .ReturnsAsync((false, "Name is already in use."));

            // Act
            var result = await _productController.AddProductAsync(productCreateDto);

            // Assert
            var badRequest = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
            Assert.IsTrue(badRequest.Value?.ToString()?.Contains("Name is already in use.") ?? false); // Verifies that the error message returned in the BadRequest contains the expected text.

            _mockProductBusinessRules.Verify(r => r.ValidateNewProductAsync(productCreateDto), Times.Once);
            _mockProductDataService.VerifyNoOtherCalls(); // Verifies that no other methods on _mockProductDataService were called, not even AddProductAsync.
        }

        #endregion

        #region UpdateProductAsync

        [TestMethod]
        public async Task UpdateProductAsync_ReturnsNoContent_WhenProductUpdated()
        {
            // Arrange
            var productUpdateDto = GetSampleProductUpdateDto();
            var product = GetSampleProduct();

            _mockProductDataService
                .Setup(s => s.GetProductByKeyAsync(productUpdateDto.Key))
                .ReturnsAsync(product);

            _mockProductBusinessRules
                .Setup(r => r.ValidateUpdatedProductAsync(productUpdateDto))
                .ReturnsAsync((true, null));

            _mockMapper.Setup(m => m.Map(productUpdateDto, product));

            _mockProductDataService
                .Setup(s => s.UpdateProductAsync(product))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _productController.UpdateProductAsync(productUpdateDto.Key, productUpdateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task UpdateProductAsync_ReturnsBadRequest_WhenModelStateInvalid()
        {
            // Arrange
            var productUpdateDto = GetSampleProductUpdateDto();
            _productController.ModelState.AddModelError("Name", "Required");

            // Act
            var result = await _productController.UpdateProductAsync(productUpdateDto.Key, productUpdateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task UpdateProductAsync_ReturnsBadRequest_WhenKeyMismatch()
        {
            // Arrange
            var productUpdateDto = GetSampleProductUpdateDto();
            int differentKey = productUpdateDto.Key + 1;

            // Act
            var result = await _productController.UpdateProductAsync(differentKey, productUpdateDto);

            // Assert
            var badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
            StringAssert.Contains(badRequest.Value?.ToString(), "Product key mismatch");
        }

        [TestMethod]
        public async Task UpdateProductAsync_ReturnsNotFound_WhenProductNotFound()
        {
            // Arrange
            var productUpdateDto = GetSampleProductUpdateDto();

            _mockProductDataService
                .Setup(s => s.GetProductByKeyAsync(productUpdateDto.Key))
                .ReturnsAsync((Product?)null);

            // Act
            var result = await _productController.UpdateProductAsync(productUpdateDto.Key, productUpdateDto);

            // Assert
            var notFound = result as NotFoundObjectResult;
            Assert.IsNotNull(notFound);
        }

        [TestMethod]
        public async Task UpdateProductAsync_ReturnsBadRequest_WhenBusinessRulesFailed()
        {
            // Arrange
            var productUpdateDto = GetSampleProductUpdateDto();
            var product = GetSampleProduct();

            _mockProductDataService
                .Setup(s => s.GetProductByKeyAsync(productUpdateDto.Key))
                .ReturnsAsync(product);

            _mockProductBusinessRules
                .Setup(r => r.ValidateUpdatedProductAsync(productUpdateDto))
                .ReturnsAsync((false, "Name is already in use."));

            // Act
            var result = await _productController.UpdateProductAsync(productUpdateDto.Key, productUpdateDto);

            // Assert
            var badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
            StringAssert.Contains(badRequest.Value?.ToString(), "Name is already in use.");
        }

        [TestMethod]
        public async Task UpdateProductAsync_ReturnsConflict_WhenConcurrencyExceptionThrown()
        {
            // Arrange
            var productUpdateDto = GetSampleProductUpdateDto();
            var product = GetSampleProduct();

            _mockProductDataService
                .Setup(s => s.GetProductByKeyAsync(productUpdateDto.Key))
                .ReturnsAsync(product);

            _mockProductBusinessRules
                .Setup(r => r.ValidateUpdatedProductAsync(productUpdateDto))
                .ReturnsAsync((true, null));

            _mockMapper.Setup(m => m.Map(productUpdateDto, product));

            _mockProductDataService
                .Setup(s => s.UpdateProductAsync(product))
                .ThrowsAsync(new DbUpdateConcurrencyException());

            // Act
            var result = await _productController.UpdateProductAsync(productUpdateDto.Key, productUpdateDto);

            // Assert
            var conflict = result as ConflictObjectResult;
            Assert.IsNotNull(conflict);
            StringAssert.Contains(conflict.Value?.ToString(), "The product was modified by another user.");
        }

        #endregion

        #region DeleteProductAsync

        [TestMethod]
        public async Task DeleteProductAsync_ReturnsNotFound_WhenProductNotFound()
        {
            // Arrange
            int key = 123;

            _mockProductDataService
                .Setup(s => s.GetProductByKeyAsync(key))
                .ReturnsAsync((Product?)null);

            // Act
            var result = await _productController.DeleteProductAsync(key);

            // Assert
            var notFound = result as NotFoundObjectResult;
            Assert.IsNotNull(notFound);
        }

        [TestMethod]
        public async Task DeleteProductAsync_ReturnsNoContent_WhenProductDeleted()
        {
            // Arrange
            var product = GetSampleProduct();

            _mockProductDataService
                .Setup(s => s.GetProductByKeyAsync(product.Key))
                .ReturnsAsync(product);

            _mockProductDataService
                .Setup(s => s.DeleteProductAsync(product.Key))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _productController.DeleteProductAsync(product.Key);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));

            _mockProductDataService.Verify(s => s.DeleteProductAsync(product.Key), Times.Once);
        }

        #endregion
    }
}
