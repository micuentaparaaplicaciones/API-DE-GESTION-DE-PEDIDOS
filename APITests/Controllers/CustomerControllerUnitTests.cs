using API.BusinessRules.Interfaces;
using API.Controllers;
using API.DataServices.Interfaces;
using API.Dtos.CustomerDtos;
using API.Entities;
using API.Security.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace APITests.Controllers
{
    [TestClass()]
    public class CustomerControllerUnitTests
    {
        private Mock<ICustomerDataService> _mockCustomerDataService = null!;
        private Mock<ICustomerBusinessRules> _mockCustomerBusinessRules = null!;
        private Mock<IMapper> _mockMapper = null!;
        private Mock<ICustomerSecurityService> _mockCustomerSecurityService = null!;
        private CustomerController _customerController = null!;

        [TestInitialize]
        public void SetUp()
        {
            _mockCustomerDataService = new Mock<ICustomerDataService>();
            _mockCustomerBusinessRules = new Mock<ICustomerBusinessRules>();
            _mockMapper = new Mock<IMapper>();
            _mockCustomerSecurityService = new Mock<ICustomerSecurityService>();
            _customerController = new CustomerController(
                _mockCustomerDataService.Object,
                _mockCustomerBusinessRules.Object,
                _mockMapper.Object,
                _mockCustomerSecurityService.Object);
        }

        #region Helpers

        private static Customer GetSampleCustomer(int key = 1, string identification = "102340567")
        {
            return new Customer
            {
                Key = key,
                Identification = identification,
                Name = "Customer",
                Email = $"{identification.ToLower()}@example.com",
                Phone = "55551111",
                Address = "Customer Address",
                Password = "password",
                RegistrationDate = DateTime.UtcNow,
                ModificationDate = DateTime.UtcNow,
                CreatedBy = null,
                ModifiedBy = null,
                RowVersion = 0
            };
        }

        private static CustomerCreateDto GetSampleCustomerCreateDto(string identification = "102340567")
        {
            return new CustomerCreateDto
            {
                Identification = identification,
                Name = "Customer",
                Email = $"{identification.ToLower()}@example.com",
                Phone = "55551111",
                Address = "Customer Address",
                Password = "password",
                CreatedBy = null
            };
        }

        private static CustomerUpdateDto GetSampleCustomerUpdateDto(int key = 1, string identification = "102340567")
        {
            return new CustomerUpdateDto
            {
                Key = key,
                Identification = identification,
                Name = "Customer",
                Email = $"{identification.ToLower()}@example.com",
                Phone = "55551111",
                Address = "Customer Address",
                Password = "password",
                ModifiedBy = null,
                RowVersion = 0
            };
        }

        private static CustomerReadDto GetSampleCustomerReadDto(int key = 1, string identification = "102340567")
        {
            return new CustomerReadDto
            {
                Key = key,
                Identification = identification,
                Name = "Customer",
                Email = $"{identification.ToLower()}@example.com",
                Phone = "55551111",
                Address = "Customer Address",
                RegistrationDate = DateTime.UtcNow,
                ModificationDate = DateTime.UtcNow,
                CreatedBy = null,
                ModifiedBy = null,
                RowVersion = 0
            };
        }

        #endregion

        #region GetCustomerByKeyAsync

        /// <summary>
        /// The GetCustomerByKeyAsync method in CustomerController returns a Task<ActionResult<CustomerReadDto>>,
        /// which is a wrapper (ActionResult<T>). To access the underlying OkObjectResult,
        /// you need to use `.Result` like this: `result.Result as OkObjectResult`.
        /// </summary>
        [TestMethod]
        public async Task GetCustomerByKeyAsync_ReturnsOkWithCustomer()
        {
            // Arrange
            var customer = GetSampleCustomer();
            var customerReadDto = GetSampleCustomerReadDto();

            _mockCustomerDataService.Setup(s => s.GetCustomerByKeyAsync(customer.Key)).ReturnsAsync(customer);
            _mockMapper.Setup(m => m.Map<CustomerReadDto>(customer)).Returns(customerReadDto);

            // Act
            var result = await _customerController.GetCustomerByKeyAsync(customer.Key);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(customerReadDto, okResult.Value);
            _mockCustomerDataService.Verify(s => s.GetCustomerByKeyAsync(customer.Key), Times.Once);
        }

        [TestMethod]
        public async Task GetCustomerByKeyAsync_ReturnsNotFound_WhenCustomerDoesNotExist()
        {
            // Arrange
            int key = 0; // Non-existent key

            _mockCustomerDataService.Setup(s => s.GetCustomerByKeyAsync(key)).ReturnsAsync((Customer?)null);

            // Act
            var result = await _customerController.GetCustomerByKeyAsync(key);

            // Assert
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            _mockCustomerDataService.Verify(s => s.GetCustomerByKeyAsync(key), Times.Once);
        }

        #endregion

        #region GetAllCustomersAsync

        [TestMethod]
        public async Task GetAllCustomersAsync_ReturnsOkWithCustomers()
        {
            // Arrange
            var customers = new List<Customer> { GetSampleCustomer() };
            var customerReadDtos = new List<CustomerReadDto> { GetSampleCustomerReadDto() };

            _mockCustomerDataService.Setup(s => s.GetAllCustomersAsync()).ReturnsAsync(customers);
            _mockMapper.Setup(m => m.Map<List<CustomerReadDto>>(customers)).Returns(customerReadDtos);

            // Act
            var result = await _customerController.GetAllCustomersAsync();

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            CollectionAssert.AreEqual(customerReadDtos, (List<CustomerReadDto>)okResult.Value!);
            _mockCustomerDataService.Verify(s => s.GetAllCustomersAsync(), Times.Once);
        }

        [TestMethod]
        public async Task GetAllCustomersAsync_ReturnsNotFound_WhenNoCustomersExist()
        {
            // Arrange
            var customers = new List<Customer>();
            var customerReadDtos = new List<CustomerReadDto>();

            _mockCustomerDataService.Setup(s => s.GetAllCustomersAsync()).ReturnsAsync(customers);
            _mockMapper.Setup(m => m.Map<List<CustomerReadDto>>(customers)).Returns(customerReadDtos);

            // Act
            var result = await _customerController.GetAllCustomersAsync();

            // Assert
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            _mockCustomerDataService.Verify(s => s.GetAllCustomersAsync(), Times.Once);
        }

        #endregion

        #region AddCustomerAsync

        [TestMethod]
        public async Task AddCustomerAsync_ReturnsCreatedAtRoute_WhenCustomerIsAdded()
        {
            // Arrange
            var customerCreateDto = GetSampleCustomerCreateDto();
            var customer = GetSampleCustomer();
            var customerReadDto = GetSampleCustomerReadDto();

            _mockCustomerBusinessRules
                .Setup(r => r.ValidateNewCustomerAsync(customerCreateDto))
                .ReturnsAsync((true, null));

            _mockMapper
                .Setup(m => m.Map<Customer>(customerCreateDto))
                .Returns(customer);

            _mockCustomerSecurityService
                .Setup(s => s.HashPassword(customer, customerCreateDto.Password))
                .Returns("hashedPassword");

            _mockCustomerDataService
                .Setup(s => s.AddCustomerAsync(customer))
                .Returns(Task.CompletedTask);

            _mockMapper
                .Setup(m => m.Map<CustomerReadDto>(customer))
                .Returns(customerReadDto);

            // Act
            var result = await _customerController.AddCustomerAsync(customerCreateDto);

            // Assert
            var createdAtRoute = result.Result as CreatedAtRouteResult;
            Assert.IsNotNull(createdAtRoute);
            Assert.AreEqual("GetCustomerByKey", createdAtRoute.RouteName);
            Assert.AreEqual(customerReadDto, createdAtRoute.Value);

            _mockCustomerBusinessRules.Verify(r => r.ValidateNewCustomerAsync(customerCreateDto), Times.Once);
            _mockCustomerDataService.Verify(s => s.AddCustomerAsync(customer), Times.Once);
        }

        [TestMethod]
        public async Task AddCustomerAsync_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            _customerController.ModelState.AddModelError("Name", "Required");
            var customerCreateDto = GetSampleCustomerCreateDto();

            // Act
            var result = await _customerController.AddCustomerAsync(customerCreateDto);

            // Assert
            var badRequest = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
            // It's fine not to call .Verify() because you don't expect the service to be called if the ModelState is invalid.
        }

        [TestMethod]
        public async Task AddCustomerAsync_ReturnsBadRequest_WhenBusinessRulesValidationFails()
        {
            // Arrange
            var customerCreateDto = GetSampleCustomerCreateDto();

            _mockCustomerBusinessRules
                .Setup(r => r.ValidateNewCustomerAsync(customerCreateDto))
                .ReturnsAsync((false, "Identification is already in use."));

            // Act
            var result = await _customerController.AddCustomerAsync(customerCreateDto);

            // Assert
            var badRequest = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
            Assert.IsTrue(badRequest.Value?.ToString()?.Contains("Identification is already in use.") ?? false); // Verifies that the error message returned in the BadRequest contains the expected text.

            _mockCustomerBusinessRules.Verify(r => r.ValidateNewCustomerAsync(customerCreateDto), Times.Once); 
            _mockCustomerDataService.VerifyNoOtherCalls(); // Verifies that no other methods on _mockCustomerDataService were called, not even AddCustomerAsync.
        }

        #endregion

        #region UpdateCustomerAsync

        /// <summary>
        /// The UpdateCustomerAsync method in CustomerController returns a Task<ActionResult> (without <T>),
        /// so the result object is already a NoContentResult if the update is successful.
        /// </summary>
        [TestMethod]
        public async Task UpdateCustomerAsync_ReturnsNoContent_WhenSuccessful()
        {
            // Arrange
            var customerUpdateDto = GetSampleCustomerUpdateDto();
            var customer = GetSampleCustomer();

            _mockCustomerDataService
                .Setup(s => s.GetCustomerByKeyAsync(customerUpdateDto.Key))
                .ReturnsAsync(customer);

            _mockCustomerBusinessRules
                .Setup(r => r.ValidateUpdatedCustomerAsync(customerUpdateDto))
                .ReturnsAsync((true, null));

            _mockMapper
                .Setup(m => m.Map(customerUpdateDto, customer));

            _mockCustomerSecurityService
                .Setup(s => s.NeedsRehash(customer, customerUpdateDto.Password))
                .Returns(true);

            _mockCustomerSecurityService
                .Setup(s => s.HashPassword(customer, customerUpdateDto.Password))
                .Returns("new-hash");

            _mockCustomerDataService
                .Setup(s => s.UpdateCustomerAsync(customer))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _customerController.UpdateCustomerAsync(customerUpdateDto.Key, customerUpdateDto);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task UpdateCustomerAsync_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var customerUpdateDto = GetSampleCustomerUpdateDto();
            _customerController.ModelState.AddModelError("Name", "Required");

            // Act
            var result = await _customerController.UpdateCustomerAsync(customerUpdateDto.Key, customerUpdateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task UpdateCustomerAsync_ReturnsBadRequest_WhenKeyMismatch()
        {
            // Arrange
            var customerUpdateDto = GetSampleCustomerUpdateDto();
            int wrongKey = customerUpdateDto.Key + 1;

            // Act
            var result = await _customerController.UpdateCustomerAsync(wrongKey, customerUpdateDto);

            // Assert
            var badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
            StringAssert.Contains(badRequest.Value?.ToString(), "Customer key mismatch");
        }

        [TestMethod]
        public async Task UpdateCustomerAsync_ReturnsNotFound_WhenCustomerDoesNotExist()
        {
            // Arrange
            var customerUpdateDto = GetSampleCustomerUpdateDto();

            _mockCustomerDataService
                .Setup(s => s.GetCustomerByKeyAsync(customerUpdateDto.Key))
                .ReturnsAsync((Customer?)null);

            // Act
            var result = await _customerController.UpdateCustomerAsync(customerUpdateDto.Key, customerUpdateDto);

            // Assert
            var notFound = result as NotFoundObjectResult;
            Assert.IsNotNull(notFound);
        }

        [TestMethod]
        public async Task UpdateCustomerAsync_ReturnsBadRequest_WhenBusinessRulesValidationFails()
        {
            // Arrange
            var customerUpdateDto = GetSampleCustomerUpdateDto();
            var customer = GetSampleCustomer();

            _mockCustomerDataService
                .Setup(s => s.GetCustomerByKeyAsync(customerUpdateDto.Key))
                .ReturnsAsync(customer);

            _mockCustomerBusinessRules
                .Setup(r => r.ValidateUpdatedCustomerAsync(customerUpdateDto))
                .ReturnsAsync((false, "Email already in use."));

            // Act
            var result = await _customerController.UpdateCustomerAsync(customerUpdateDto.Key, customerUpdateDto);

            // Assert
            var badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
            StringAssert.Contains(badRequest.Value?.ToString(), "Email already in use");
        }

        [TestMethod]
        public async Task UpdateCustomerAsync_ReturnsConflict_WhenConcurrencyExceptionIsThrown()
        {
            // Arrange
            var customerUpdateDto = GetSampleCustomerUpdateDto();
            var customer = GetSampleCustomer();

            _mockCustomerDataService
                .Setup(s => s.GetCustomerByKeyAsync(customerUpdateDto.Key))
                .ReturnsAsync(customer);

            _mockCustomerBusinessRules
                .Setup(r => r.ValidateUpdatedCustomerAsync(customerUpdateDto))
                .ReturnsAsync((true, null));

            _mockMapper
                .Setup(m => m.Map(customerUpdateDto, customer));
            _mockCustomerSecurityService
                .Setup(s => s.NeedsRehash(customer, customerUpdateDto.Password))
                .Returns(false);

            _mockCustomerDataService
                .Setup(s => s.UpdateCustomerAsync(customer))
                .ThrowsAsync(new DbUpdateConcurrencyException());

            // Act
            var result = await _customerController.UpdateCustomerAsync(customerUpdateDto.Key, customerUpdateDto);

            // Assert
            var conflict = result as ConflictObjectResult;
            Assert.IsNotNull(conflict);
            StringAssert.Contains(conflict.Value?.ToString(), "The customer was modified by another user");
        }

        #endregion

        #region DeleteCustomerAsync

        [TestMethod]
        public async Task DeleteCustomerAsync_ReturnsNotFound_WhenCustomerDoesNotExist()
        {
            // Arrange
            int key = 123;

            _mockCustomerDataService
                .Setup(s => s.GetCustomerByKeyAsync(key))
                .ReturnsAsync((Customer?)null);

            // Act
            var result = await _customerController.DeleteCustomerAsync(key);

            // Assert
            var notFound = result as NotFoundObjectResult;
            Assert.IsNotNull(notFound);
        }

        [TestMethod]
        public async Task DeleteCustomerAsync_ReturnsNoContent_WhenCustomerExists()
        {
            // Arrange
            var customer = GetSampleCustomer();

            _mockCustomerDataService
                .Setup(s => s.GetCustomerByKeyAsync(customer.Key))
                .ReturnsAsync(customer);

            _mockCustomerDataService
                .Setup(s => s.DeleteCustomerAsync(customer.Key))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _customerController.DeleteCustomerAsync(customer.Key);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            _mockCustomerDataService.Verify(s => s.DeleteCustomerAsync(customer.Key), Times.Once);
        }

        #endregion

    }
}
