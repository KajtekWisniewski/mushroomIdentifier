using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using mushroomAPI.Controllers;
using mushroomAPI.Data;
using mushroomAPI.DTOs.Auth;
using mushroomAPI.DTOs.User;
using mushroomAPI.Entities;
using mushroomAPI.Services.IServices;
using Xunit;

namespace mushroomAPI.UnitTests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly DbContextOptions<ApplicationDbContext> _options;
        private readonly ApplicationDbContext _context;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mockAuthService = new Mock<IAuthService>();
            _mockMapper = new Mock<IMapper>();
            
            
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            
            _context = new ApplicationDbContext(_options);
            
            
            var testUser = new User
            {
                Id = 1,
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = new byte[] { 1, 2, 3 },
                PasswordSalt = new byte[] { 4, 5, 6 }
            };
            
            _context.Users.Add(testUser);
            _context.SaveChanges();
            
            _controller = new AuthController(_mockAuthService.Object, _context, _mockMapper.Object);
        }

        [Fact]
        public async Task Register_WithNewUser_ReturnsCreatedUser()
        {
            
            var userDto = new UserDTO
            {
                Username = "newuser",
                Email = "new@example.com",
                Password = "password123"
            };

            byte[] passwordHash = { 1, 2, 3 };
            byte[] passwordSalt = { 4, 5, 6 };

            _mockAuthService.Setup(x => x.CreatePasswordHash(userDto.Password, out passwordHash, out passwordSalt));
            
            var userProfileDto = new UserProfileDTO
            {
                Username = userDto.Username,
                Email = userDto.Email
            };
            
            _mockMapper.Setup(m => m.Map<UserProfileDTO>(It.IsAny<User>())).Returns(userProfileDto);

            
            var result = await _controller.Register(userDto);

            
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(_controller.Register), createdAtActionResult.ActionName);
            
            var returnValue = Assert.IsType<UserProfileDTO>(createdAtActionResult.Value);
            Assert.Equal("newuser", returnValue.Username);
            Assert.Equal("new@example.com", returnValue.Email);
        }

        [Fact]
        public async Task Register_WithExistingUsername_ReturnsConflict()
        {
            
            var userDto = new UserDTO
            {
                Username = "testuser", 
                Email = "new@example.com",
                Password = "password123"
            };

            
            var result = await _controller.Register(userDto);

            
            var conflictResult = Assert.IsType<ConflictObjectResult>(result.Result);
            Assert.NotNull(conflictResult.Value);
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsTokenAndUser()
        {
            
            var loginDto = new UserLoginDTO
            {
                Username = "testuser",
                Password = "password123"
            };
            
            _mockAuthService.Setup(x => x.VerifyPasswordHash(loginDto.Password, It.IsAny<byte[]>(), It.IsAny<byte[]>()))
                .Returns(true);
                
            _mockAuthService.Setup(x => x.CreateToken(It.IsAny<User>()))
                .Returns("test-token");
                
            var userProfileDto = new UserProfileDTO
            {
                Username = "testuser",
                Email = "test@example.com"
            };
            
            _mockMapper.Setup(m => m.Map<UserProfileDTO>(It.IsAny<User>())).Returns(userProfileDto);

            
            var result = await _controller.Login(loginDto);

            
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<LoginResponseDTO>(okResult.Value);
            Assert.Equal("test-token", returnValue.Token);
            Assert.Equal("testuser", returnValue.User.Username);
        }

        [Fact]
        public async Task Login_WithInvalidUsername_ReturnsBadRequest()
        {
            
            var loginDto = new UserLoginDTO
            {
                Username = "nonexistentuser",
                Password = "password123"
            };

            
            var result = await _controller.Login(loginDto);

            
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("User not found.", badRequestResult.Value);
        }

        [Fact]
        public async Task Login_WithInvalidPassword_ReturnsBadRequest()
        {
            
            var loginDto = new UserLoginDTO
            {
                Username = "testuser",
                Password = "wrongpassword"
            };

            _mockAuthService.Setup(x => x.VerifyPasswordHash(loginDto.Password, It.IsAny<byte[]>(), It.IsAny<byte[]>()))
                .Returns(false);

            
            var result = await _controller.Login(loginDto);

            
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Wrong password.", badRequestResult.Value);
        }
    }
}