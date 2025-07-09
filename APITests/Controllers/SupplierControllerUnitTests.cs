using API.BusinessRules.Interfaces;
using API.Controllers;
using API.DataServices.Interfaces;
using API.Dtos.CategoryDtos;
using API.Dtos.SupplierDtos;
using API.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace APITests.Controllers
{
    /// <summary>
    /// Unit tests for SupplierController, covering supplier management operations such as: 
    /// creation, retrieval, update with concurrency control, and deletion. 
    /// </summary>
    [TestClass()]
    public class SupplierControllerUnitTests
    {
        private Mock<ISupplierDataService> _mockSupplierDataService = null!;
        private Mock<ISupplierBusinessRules> _mockSupplierBusinessRules = null!;
        private Mock<IMapper> _mockMapper = null!;
        private SupplierController _supplierController = null!;

        [TestInitialize]
        public void SetUp()
        {
            _mockSupplierDataService = new Mock<ISupplierDataService>();
            _mockSupplierBusinessRules = new Mock<ISupplierBusinessRules>();
            _mockMapper = new Mock<IMapper>();
            _supplierController = new SupplierController(
                _mockSupplierDataService.Object,
                _mockSupplierBusinessRules.Object,
                _mockMapper.Object);
        }

        #region Helpers

        private static Supplier GetSampleSupplier(int id = 1, string name = "Acme")
        {
            return new Supplier
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

        private static SupplierCreateDto GetSampleSupplierCreateDto(string name = "Acme")
        {
            return new SupplierCreateDto
            {
                Name = name,
                CreatedBy = 0
            };
        }

        private static SupplierUpdateDto GetSampleSupplierUpdateDto(int id = 1, string name = "Acme")
        {
            return new SupplierUpdateDto
            {
                Key = id,
                Name = name,
                ModifiedBy = 0,
                RowVersion = 0
            };
        }

        private static SupplierReadDto GetSampleSupplierReadDto(int id = 1, string name = "Acme")
        {
            return new SupplierReadDto
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

        #region GetSupplierByKeyAsync

        [TestMethod]
        public async Task GetSupplierByKeyAsync_ReturnsOkWithSupplier()
        {
            // Arrange
            var supplier = GetSampleSupplier();
            var supplierReadDto = GetSampleSupplierReadDto();

            _mockSupplierDataService.Setup(s => s.GetSupplierByKeyAsync(supplier.Key)).ReturnsAsync(supplier);
            _mockMapper.Setup(m => m.Map<SupplierReadDto>(supplier)).Returns(supplierReadDto);

            // Act
            var result = await _supplierController.GetSupplierByKeyAsync(supplier.Key);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(supplierReadDto, okResult.Value);

            _mockSupplierDataService.Verify(s => s.GetSupplierByKeyAsync(supplier.Key), Times.Once);
        }

        [TestMethod]
        public async Task GetSupplierByKeyAsync_ReturnsNotFound_WhenSupplierNotFound()
        {
            // Arrange
            int key = 0; // Non-existent key

            _mockSupplierDataService
                .Setup(s => s.GetSupplierByKeyAsync(key))
                .ReturnsAsync((Supplier?)null);

            var result = await _supplierController.GetSupplierByKeyAsync(key);

            var notFound = result.Result as NotFoundObjectResult;
            Assert.IsNotNull(notFound);

            _mockSupplierDataService.Verify(s => s.GetSupplierByKeyAsync(key), Times.Once);
        }

        #endregion

        #region GetSupplierByNameAsync

        /// <summary>
        /// The GetSupplierByNameAsync method in SupplierController returns a Task<ActionResult<SupplierReadDto>>,
        /// which is a wrapper (ActionResult<T>). To access the underlying OkObjectResult,
        /// you need to use `.Result` like this: `result.Result as OkObjectResult`.
        /// </summary>
        [TestMethod]
        public async Task GetSupplierByNameAsync_ReturnsOkWithSupplier()
        {
            // Arrange
            var supplier = GetSampleSupplier();
            var supplierReadDto = GetSampleSupplierReadDto();

            _mockSupplierDataService
                .Setup(s => s.GetSupplierByNameAsync(supplier.Name))
                .ReturnsAsync(supplier);

            _mockMapper
                .Setup(m => m.Map<SupplierReadDto>(supplier))
                .Returns(supplierReadDto);

            // Act
            var result = await _supplierController.GetSupplierByNameAsync(supplier.Name);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(supplierReadDto, okResult.Value);

            _mockSupplierDataService.Verify(s => s.GetSupplierByNameAsync(supplier.Name), Times.Once);
        }

        [TestMethod]
        public async Task GetSupplierByNameAsync_ReturnsNotFound_WhenSupplierNotFound()
        {
            // Arrange
            string name = "Unknown"; // Non-existent name

            _mockSupplierDataService
                .Setup(s => s.GetSupplierByNameAsync(name))
                .ReturnsAsync((Supplier?)null);

            // Act
            var result = await _supplierController.GetSupplierByNameAsync(name);

            // Assert
            var notFound = result.Result as NotFoundObjectResult;
            Assert.IsNotNull(notFound);

            _mockSupplierDataService.Verify(s => s.GetSupplierByNameAsync(name), Times.Once);
        }

        #endregion

        #region GetAllSuppliersAsync

        [TestMethod]
        public async Task GetAllSuppliersAsync_ReturnsOkWithSuppliers()
        {
            // Arrange
            var suppliers = new List<Supplier> { GetSampleSupplier() };
            var supplierReadDtos = new List<SupplierReadDto> { GetSampleSupplierReadDto() };

            _mockSupplierDataService
                .Setup(s => s.GetAllSuppliersAsync())
                .ReturnsAsync(suppliers);

            _mockMapper
                .Setup(m => m.Map<List<SupplierReadDto>>(suppliers))
                .Returns(supplierReadDtos);

            // Act
            var result = await _supplierController.GetAllSuppliersAsync();

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            CollectionAssert.AreEqual(supplierReadDtos, (List<SupplierReadDto>)okResult.Value!);

            _mockSupplierDataService.Verify(s => s.GetAllSuppliersAsync(), Times.Once);
        }

        [TestMethod]
        public async Task GetAllSuppliersAsync_ReturnsNotFound_WhenSuppliersNotFound()
        {
            // Arrange
            var suppliers = new List<Supplier>();
            var supplierDtos = new List<SupplierReadDto>();

            _mockSupplierDataService
                .Setup(s => s.GetAllSuppliersAsync())
                .ReturnsAsync(suppliers);

            _mockMapper
                .Setup(m => m.Map<List<SupplierReadDto>>(suppliers))
                .Returns(supplierDtos);

            // Act
            var result = await _supplierController.GetAllSuppliersAsync();

            // Assert
            var notFound = result.Result as NotFoundObjectResult;
            Assert.IsNotNull(notFound);

            _mockSupplierDataService.Verify(s => s.GetAllSuppliersAsync(), Times.Once);
        }

        #endregion

        #region AddSupplierAsync

        [TestMethod]
        public async Task AddSupplierAsync_ReturnsCreatedAtRoute_WhenSupplierAdded()
        {
            // Arrange
            var supplierCreateDto = GetSampleSupplierCreateDto();
            var supplier = GetSampleSupplier();
            var supplierReadDto = GetSampleSupplierReadDto();

            _mockSupplierBusinessRules
                .Setup(r => r.ValidateNewSupplierAsync(supplierCreateDto))
                .ReturnsAsync((true, null));

            _mockMapper
                .Setup(m => m.Map<Supplier>(supplierCreateDto))
                .Returns(supplier);

            _mockSupplierDataService
                .Setup(s => s.AddSupplierAsync(supplier))
                .Returns(Task.CompletedTask);

            _mockMapper
                .Setup(m => m.Map<SupplierReadDto>(supplier))
                .Returns(supplierReadDto);

            // Act
            var result = await _supplierController.AddSupplierAsync(supplierCreateDto);

            // Assert
            var created = result.Result as CreatedAtRouteResult;
            Assert.IsNotNull(created);
            Assert.AreEqual("GetSupplierByKeyAsync", created.RouteName);
            Assert.AreEqual(supplierReadDto, created.Value);

            _mockSupplierBusinessRules.Verify(r => r.ValidateNewSupplierAsync(supplierCreateDto), Times.Once);
            _mockSupplierDataService.Verify(s => s.AddSupplierAsync(supplier), Times.Once);
        }

        [TestMethod]
        public async Task AddSupplierAsync_ReturnsBadRequest_WhenModelStateInvalid()
        {
            // Arrange
            _supplierController.ModelState.AddModelError("Name", "Required");
            var supplierCreateDto = GetSampleSupplierCreateDto();

            // Act
            var result = await _supplierController.AddSupplierAsync(supplierCreateDto);

            // Assert
            var badRequest = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);

            // It's fine not to call .Verify() because you don't expect the service to be called if the ModelState is invalid.
        }

        [TestMethod]
        public async Task AddSupplierAsync_ReturnsBadRequest_WhenBusinessRulesFailed()
        {
            // Arrange
            var supplierCreateDto = GetSampleSupplierCreateDto();

            _mockSupplierBusinessRules
                .Setup(r => r.ValidateNewSupplierAsync(supplierCreateDto))
                .ReturnsAsync((false, "Name is already in use."));

            // Act
            var result = await _supplierController.AddSupplierAsync(supplierCreateDto);

            // Assert
            var badRequest = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
            Assert.IsTrue(badRequest.Value?.ToString()?.Contains("Name is already in use.") ?? false); // Verifies that the error message returned in the BadRequest contains the expected text.

            _mockSupplierBusinessRules.Verify(r => r.ValidateNewSupplierAsync(supplierCreateDto), Times.Once);
            _mockSupplierDataService.VerifyNoOtherCalls(); // Verifies that no other methods on _mockSupplierDataService were called, not even AddSupplierAsync.
        }

        #endregion

        #region UpdateSupplierAsync

        [TestMethod]
        public async Task UpdateSupplierAsync_ReturnsNoContent_WhenSupplierUpdated()
        {
            // Arrange
            var supplierUpdateDto = GetSampleSupplierUpdateDto();
            var supplier = GetSampleSupplier();

            _mockSupplierDataService
                .Setup(s => s.GetSupplierByKeyAsync(supplierUpdateDto.Key))
                .ReturnsAsync(supplier);

            _mockSupplierBusinessRules
                .Setup(r => r.ValidateUpdatedSupplierAsync(supplierUpdateDto))
                .ReturnsAsync((true, null));

            _mockMapper
                .Setup(m => m.Map(supplierUpdateDto, supplier));

            _mockSupplierDataService
                .Setup(s => s.UpdateSupplierAsync(supplier))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _supplierController.UpdateSupplierAsync(supplierUpdateDto.Key, supplierUpdateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task UpdateSupplierAsync_ReturnsBadRequest_WhenModelStateInvalid()
        {
            // Arrange
            var supplierUpdateDto = GetSampleSupplierUpdateDto();
            _supplierController.ModelState.AddModelError("Name", "Required");

            // Act
            var result = await _supplierController.UpdateSupplierAsync(supplierUpdateDto.Key, supplierUpdateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task UpdateSupplierAsync_ReturnsBadRequest_WhenKeyMismatch()
        {
            // Arrange
            var dto = GetSampleSupplierUpdateDto();
            int differentKey = dto.Key + 1;

            // Act
            var result = await _supplierController.UpdateSupplierAsync(differentKey, dto);

            // Assert
            var badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
            StringAssert.Contains(badRequest.Value?.ToString(), "Supplier key mismatch");
        }

        [TestMethod]
        public async Task UpdateSupplierAsync_ReturnsNotFound_WhenSupplierNotFound()
        {
            // Arrange
            var supplierUpdateDto = GetSampleSupplierUpdateDto();

            _mockSupplierDataService
                .Setup(s => s.GetSupplierByKeyAsync(supplierUpdateDto.Key))
                .ReturnsAsync((Supplier?)null);

            // Act
            var result = await _supplierController.UpdateSupplierAsync(supplierUpdateDto.Key, supplierUpdateDto);

            // Assert
            var notFound = result as NotFoundObjectResult;
            Assert.IsNotNull(notFound);
        }

        [TestMethod]
        public async Task UpdateSupplierAsync_ReturnsBadRequest_WhenBusinessRulesFailed()
        {
            // Arrange
            var supplierUpdateDto = GetSampleSupplierUpdateDto();
            var supplier = GetSampleSupplier();

            _mockSupplierDataService
                .Setup(s => s.GetSupplierByKeyAsync(supplierUpdateDto.Key))
                .ReturnsAsync(supplier);

            _mockSupplierBusinessRules
                .Setup(r => r.ValidateUpdatedSupplierAsync(supplierUpdateDto))
                .ReturnsAsync((false, "Name is already in use."));

            // Act
            var result = await _supplierController.UpdateSupplierAsync(supplierUpdateDto.Key, supplierUpdateDto);

            // Assert
            var badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
            StringAssert.Contains(badRequest.Value?.ToString(), "Name is already in use.");
        }

        [TestMethod]
        public async Task UpdateSupplierAsync_ReturnsConflict_WhenConcurrencyExceptionThrown()
        {
            // Arrange
            var supplierUpdateDto = GetSampleSupplierUpdateDto();
            var supplier = GetSampleSupplier();

            _mockSupplierDataService
                .Setup(s => s.GetSupplierByKeyAsync(supplierUpdateDto.Key))
                .ReturnsAsync(supplier);

            _mockSupplierBusinessRules
                .Setup(r => r.ValidateUpdatedSupplierAsync(supplierUpdateDto))
                .ReturnsAsync((true, null));

            _mockMapper
                .Setup(m => m.Map(supplierUpdateDto, supplier));

            _mockSupplierDataService
                .Setup(s => s.UpdateSupplierAsync(supplier))
                .ThrowsAsync(new DbUpdateConcurrencyException());

            var result = await _supplierController.UpdateSupplierAsync(supplierUpdateDto.Key, supplierUpdateDto);

            var conflict = result as ConflictObjectResult;
            Assert.IsNotNull(conflict);
            StringAssert.Contains(conflict.Value?.ToString(), "The supplier was modified by another user.");
        }

        #endregion

        #region DeleteSupplierAsync

        [TestMethod]
        public async Task DeleteSupplierAsync_ReturnsNotFound_WhenSupplierNotFound()
        {
            // Arrange
            int key = 123;

            _mockSupplierDataService
                .Setup(s => s.GetSupplierByKeyAsync(key))
                .ReturnsAsync((Supplier?)null);

            // Act
            var result = await _supplierController.DeleteSupplierAsync(key);

            // Assert
            var notFound = result as NotFoundObjectResult;
            Assert.IsNotNull(notFound);
        }

        [TestMethod]
        public async Task DeleteSupplierAsync_ReturnsNoContent_WhenSupplierDeleted()
        {
            // Arrange
            var supplier = GetSampleSupplier();

            _mockSupplierDataService
                .Setup(s => s.GetSupplierByKeyAsync(supplier.Key))
                .ReturnsAsync(supplier);

            _mockSupplierDataService
                .Setup(s => s.DeleteSupplierAsync(supplier.Key))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _supplierController.DeleteSupplierAsync(supplier.Key);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));

            _mockSupplierDataService.Verify(s => s.DeleteSupplierAsync(supplier.Key), Times.Once);
        }

        #endregion
    }
}
