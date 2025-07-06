using API.BusinessRules.Interfaces;
using API.Controllers;
using API.DataServices.Interfaces;
using API.Dtos.UserDtos;
using API.Entities;
using API.Security.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace APITests.Controllers
{
    /// <summary>
    /// Unit tests for UserController, covering user management operations such as: 
    /// creation, retrieval, update with concurrency control, and deletion. 
    /// </summary>
    [TestClass()]
    public class UserControllerUnitTests
    {
        private Mock<IUserDataService> _mockUserDataService = null!;
        private Mock<IUserBusinessRules> _mockUserBusinessRules = null!;
        private Mock<IMapper> _mockMapper = null!;
        private Mock<IUserSecurityService> _mockUserSecurityService = null!; 
        private UserController _userController = null!;

        [TestInitialize]
        public void SetUp()
        {
            _mockUserDataService = new Mock<IUserDataService>();
            _mockUserBusinessRules = new Mock<IUserBusinessRules>();
            _mockMapper = new Mock<IMapper>();
            _mockUserSecurityService = new Mock<IUserSecurityService>();
            _userController = new UserController(
                _mockUserDataService.Object, _mockUserBusinessRules.Object, 
                _mockMapper.Object, 
                _mockUserSecurityService.Object);
        }

        #region Helpers

        private static User GetSampleUser(int id = 1, string identification = "102340567")
        {
            return new User
            {
                Key = id,
                Identification = identification,
                Name = "User",
                Email = $"{identification.ToLower()}@example.com",
                Phone = "55551111",
                Address = "User Address",
                Password = "password",
                Role = "Administrator",
                RegistrationDate = DateTime.UtcNow,
                ModificationDate = DateTime.UtcNow,
                CreatedBy = null,
                ModifiedBy = null,
                RowVersion = 0
            };
        }

        private static UserCreateDto GetSampleUserCreateDto(string identification = "102340567")
        {
            return new UserCreateDto
            {
                Identification = identification,
                Name = "User",
                Email = $"{identification.ToLower()}@example.com",
                Phone = "55551111",
                Address = "User Address",
                Password = "password",
                Role = "Administrator",
                CreatedBy = null
            };
        }

        private static UserUpdateDto GetSampleUserUpdateDto(int id = 1, string identification = "102340567")
        {
            return new UserUpdateDto
            {
                Key = id,
                Identification = identification,
                Name = "User",
                Email = $"{identification.ToLower()}@example.com",
                Phone = "55551111",
                Address = "User Address",
                Password = "password",
                Role = "Administrator",
                ModifiedBy = null,
                RowVersion = 0
            };
        }

        private static UserReadDto GetSampleUserReadDto(int id = 1, string identification = "102340567")
        {
            return new UserReadDto
            {
                Key = id,
                Identification = identification,
                Name = "User",
                Email = $"{identification.ToLower()}@example.com",
                Phone = "55551111",
                Address = "User Address",
                RegistrationDate = DateTime.UtcNow,
                ModificationDate = DateTime.UtcNow,
                Role = "Administrator",
                CreatedBy = null,
                ModifiedBy = null,
                RowVersion = 0
            };
        }

        #endregion

        #region GetUserByKeyAsync

        /// <summary>
        /// The GetUserByKeyAsync method in UserController returns a Task<ActionResult<UserReadDto>>,
        /// which is a wrapper (ActionResult<T>). To access the underlying OkObjectResult,
        /// you need to use `.Result` like this: `result.Result as OkObjectResult`.
        /// </summary>
        [TestMethod]
        public async Task GetUserByKeyAsync_ReturnsOkWithUser()
        {
            // Arrange
            var user = GetSampleUser();
            var userReadDto = GetSampleUserReadDto();

            _mockUserDataService.Setup(s => s.GetUserByKeyAsync(user.Key)).ReturnsAsync(user);
            _mockMapper.Setup(m => m.Map<UserReadDto>(user)).Returns(userReadDto);

            // Act
            var result = await _userController.GetUserByKeyAsync(user.Key);

            // Assert
            var okResult = result.Result as OkObjectResult; 
            Assert.IsNotNull(okResult);
            Assert.AreEqual(userReadDto, okResult.Value);
            _mockUserDataService.Verify(s => s.GetUserByKeyAsync(user.Key), Times.Once);
        }

        [TestMethod]
        public async Task GetUserByKeyAsync_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            int key = 0; // Non-existent key

            _mockUserDataService.Setup(s => s.GetUserByKeyAsync(key)).ReturnsAsync((User?)null);

            // Act
            var result = await _userController.GetUserByKeyAsync(key);

            // Assert
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            _mockUserDataService.Verify(s => s.GetUserByKeyAsync(key), Times.Once);
        }

        #endregion

        #region GetAllUsersAsync

        [TestMethod] 
        public async Task GetAllUsersAsync_ReturnsOkWithUsers()
        {
            // Arrange
            var users = new List<User> { GetSampleUser() };
            var userReadDtos = new List<UserReadDto> { GetSampleUserReadDto() };

            _mockUserDataService.Setup(s => s.GetAllUsersAsync()).ReturnsAsync(users);
            _mockMapper.Setup(m => m.Map<List<UserReadDto>>(users)).Returns(userReadDtos);

            // Act
            var result = await _userController.GetAllUsersAsync();

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            CollectionAssert.AreEqual(userReadDtos, (List<UserReadDto>)okResult.Value!);
            _mockUserDataService.Verify(s => s.GetAllUsersAsync(), Times.Once);
        }

        [TestMethod] 
        public async Task GetAllUsersAsync_ReturnsNotFound_WhenNoUsersExist()
        {
            // Arrange
            var users = new List<User>();
            var userReadDtos = new List<UserReadDto>();

            _mockUserDataService.Setup(s => s.GetAllUsersAsync()).ReturnsAsync(users);
            _mockMapper.Setup(m => m.Map<List<UserReadDto>>(users)).Returns(userReadDtos);

            // Act
            var result = await _userController.GetAllUsersAsync();

            // Assert
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            _mockUserDataService.Verify(s => s.GetAllUsersAsync(), Times.Once);
        }

        #endregion

        #region AddUserAsync

        [TestMethod]
        public async Task AddUserAsync_ReturnsCreatedAtRoute_WhenUserIsAdded()
        {
            // Arrange
            var userCreateDto = GetSampleUserCreateDto();
            var user = GetSampleUser();
            var userReadDto = GetSampleUserReadDto();

            _mockUserBusinessRules
                .Setup(r => r.ValidateNewUserAsync(userCreateDto))
                .ReturnsAsync((true, null));

            _mockMapper
                .Setup(m => m.Map<User>(userCreateDto))
                .Returns(user);

            _mockUserSecurityService
                .Setup(s => s.HashPassword(user, userCreateDto.Password))
                .Returns("hashedPassword");

            _mockUserDataService
                .Setup(s => s.AddUserAsync(user))
                .Returns(Task.CompletedTask);

            _mockMapper
                .Setup(m => m.Map<UserReadDto>(user))
                .Returns(userReadDto);

            // Act
            var result = await _userController.AddUserAsync(userCreateDto);

            // Assert
            var createdAtRoute = result.Result as CreatedAtRouteResult;
            Assert.IsNotNull(createdAtRoute);
            Assert.AreEqual("GetUserByKey", createdAtRoute.RouteName);
            Assert.AreEqual(userReadDto, createdAtRoute.Value);

            _mockUserBusinessRules.Verify(r => r.ValidateNewUserAsync(userCreateDto), Times.Once);
            _mockUserDataService.Verify(s => s.AddUserAsync(user), Times.Once);
        }

        [TestMethod]
        public async Task AddUserAsync_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            _userController.ModelState.AddModelError("Name", "Required");
            var userCreateDto = GetSampleUserCreateDto();

            // Act
            var result = await _userController.AddUserAsync(userCreateDto);

            // Assert
            var badRequest = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
            // It's fine not to call .Verify() because you don't expect the service to be called if the ModelState is invalid.
        }

        [TestMethod]
        public async Task AddUserAsync_ReturnsBadRequest_WhenBusinessRulesValidationFails()
        {
            // Arrange
            var userCreateDto = GetSampleUserCreateDto();

            _mockUserBusinessRules
                .Setup(r => r.ValidateNewUserAsync(userCreateDto))
                .ReturnsAsync((false, "Identification is already in use."));

            // Act
            var result = await _userController.AddUserAsync(userCreateDto);

            // Assert
            var badRequest = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
            Assert.IsTrue(badRequest.Value?.ToString()?.Contains("Identification is already in use.") ?? false); // Verifies that the error message returned in the BadRequest contains the expected text.

            _mockUserBusinessRules.Verify(r => r.ValidateNewUserAsync(userCreateDto), Times.Once);
            _mockUserDataService.VerifyNoOtherCalls(); // Verifies that no other methods on _mockUserDataService were called, not even AddUserAsync.
        }

        #endregion

        #region UpdateUserAsync

        /// <summary>
        /// The UpdateUserAsync method in UserController returns a Task<ActionResult> (without <T>),
        /// so the result object is already a NoContentResult if the update is successful.
        /// </summary>
        [TestMethod]
        public async Task UpdateUserAsync_ReturnsNoContent_WhenSuccessful()
        {
            // Arrange
            var userUpdateDto = GetSampleUserUpdateDto();
            var user = GetSampleUser();

            _mockUserDataService
                .Setup(s => s.GetUserByKeyAsync(userUpdateDto.Key))
                .ReturnsAsync(user);
            
            _mockUserBusinessRules
                .Setup(r => r.ValidateUpdatedUserAsync(userUpdateDto))
                .ReturnsAsync((true, null));
            
            _mockMapper
                .Setup(m => m.Map(userUpdateDto, user));
           
            _mockUserSecurityService
                .Setup(s => s.NeedsRehash(user, userUpdateDto.Password))
                .Returns(true);
           
            _mockUserSecurityService
                .Setup(s => s.HashPassword(user, userUpdateDto.Password))
                .Returns("new-hash");
            
            _mockUserDataService
                .Setup(s => s.UpdateUserAsync(user))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _userController.UpdateUserAsync(userUpdateDto.Key, userUpdateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task UpdateUserAsync_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var userUpdateDto = GetSampleUserUpdateDto();
            _userController.ModelState.AddModelError("Name", "Required");

            // Act
            var result = await _userController.UpdateUserAsync(userUpdateDto.Key, userUpdateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task UpdateUserAsync_ReturnsBadRequest_WhenKeyMismatch()
        {
            // Arrange
            var userUpdateDto = GetSampleUserUpdateDto();
            int differentKey = userUpdateDto.Key + 1;

            // Act
            var result = await _userController.UpdateUserAsync(differentKey, userUpdateDto);

            // Assert
            var badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
            StringAssert.Contains(badRequest.Value?.ToString(), "User key mismatch");
        }

        [TestMethod]
        public async Task UpdateUserAsync_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userUpdateDto = GetSampleUserUpdateDto();

            _mockUserDataService
                .Setup(s => s.GetUserByKeyAsync(userUpdateDto.Key))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _userController.UpdateUserAsync(userUpdateDto.Key, userUpdateDto);

            // Assert
            var notFound = result as NotFoundObjectResult;
            Assert.IsNotNull(notFound);
        }

        [TestMethod]
        public async Task UpdateUserAsync_ReturnsBadRequest_WhenBusinessRulesValidationFails()
        {
            // Arrange
            var userUpdateDto = GetSampleUserUpdateDto();
            var user = GetSampleUser();

            _mockUserDataService
                .Setup(s => s.GetUserByKeyAsync(userUpdateDto.Key))
                .ReturnsAsync(user);

            _mockUserBusinessRules
                .Setup(r => r.ValidateUpdatedUserAsync(userUpdateDto))
                .ReturnsAsync((false, "Email is already in use."));

            // Act
            var result = await _userController.UpdateUserAsync(userUpdateDto.Key, userUpdateDto);

            // Assert
            var badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
            StringAssert.Contains(badRequest.Value?.ToString(), "Email is already in use");
        }

        [TestMethod]
        public async Task UpdateUserAsync_ReturnsConflict_WhenConcurrencyExceptionIsThrown()
        {
            // Arrange
            var userUpdateDto = GetSampleUserUpdateDto();
            var user = GetSampleUser();

            _mockUserDataService
                .Setup(s => s.GetUserByKeyAsync(userUpdateDto.Key))
                .ReturnsAsync(user);

            _mockUserBusinessRules
                .Setup(r => r.ValidateUpdatedUserAsync(userUpdateDto))
                .ReturnsAsync((true, null));

            _mockMapper
                .Setup(m => m.Map(userUpdateDto, user));

            _mockUserSecurityService
                .Setup(s => s.NeedsRehash(user, userUpdateDto.Password))
                .Returns(false);

            _mockUserDataService
                .Setup(s => s.UpdateUserAsync(user))
                .ThrowsAsync(new DbUpdateConcurrencyException());

            // Act
            var result = await _userController.UpdateUserAsync(userUpdateDto.Key, userUpdateDto);

            // Assert
            var conflict = result as ConflictObjectResult;
            Assert.IsNotNull(conflict);
            StringAssert.Contains(conflict.Value?.ToString(), "The user was modified by another user");
        }

        #endregion

        #region DeleteUserAsync

        [TestMethod]
        public async Task DeleteUserAsync_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            int key = 123;

            _mockUserDataService
                .Setup(s => s.GetUserByKeyAsync(key))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _userController.DeleteUserAsync(key);

            // Assert
            var notFound = result as NotFoundObjectResult;
            Assert.IsNotNull(notFound);
        }

        [TestMethod]
        public async Task DeleteUserAsync_ReturnsNoContent_WhenUserExists()
        {
            // Arrange
            var user = GetSampleUser();

            _mockUserDataService
                .Setup(s => s.GetUserByKeyAsync(user.Key))
                .ReturnsAsync(user);

            _mockUserDataService
                .Setup(s => s.DeleteUserAsync(user.Key))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _userController.DeleteUserAsync(user.Key);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            _mockUserDataService.Verify(s => s.DeleteUserAsync(user.Key), Times.Once);
        }

        #endregion

    }
}