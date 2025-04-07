using Microsoft.Extensions.Configuration;
using Moq;
using mushroomAPI.Entities;
using mushroomAPI.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Xunit;

namespace mushroomAPI.UnitTests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            var configSection = new Mock<IConfigurationSection>();
            configSection.Setup(x => x.Value).Returns("my-secret-key-here-minimum-16-characters-long");

            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(x => x.GetSection("AppSettings:Token")).Returns(configSection.Object);

            _authService = new AuthService(_mockConfiguration.Object);
        }

        [Fact]
        public void CreatePasswordHash_ShouldCreateValidHashAndSalt()
        {
            string password = "TestPassword123!";

            _authService.CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

            Assert.NotNull(passwordHash);
            Assert.NotNull(passwordSalt);
            Assert.NotEmpty(passwordHash);
            Assert.NotEmpty(passwordSalt);
        }

        [Fact]
        public void VerifyPasswordHash_WithCorrectPassword_ReturnsTrue()
        {
            
            string password = "TestPassword123!";
            _authService.CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

            bool result = _authService.VerifyPasswordHash(password, passwordHash, passwordSalt);

            Assert.True(result);
        }

        [Fact]
        public void VerifyPasswordHash_WithIncorrectPassword_ReturnsFalse()
        {
            
            string correctPassword = "TestPassword123!";
            string wrongPassword = "WrongPassword123!";
            _authService.CreatePasswordHash(correctPassword, out byte[] passwordHash, out byte[] passwordSalt);

            bool result = _authService.VerifyPasswordHash(wrongPassword, passwordHash, passwordSalt);
           
            Assert.False(result);
        }

        [Fact]
        public void CreateToken_ShouldCreateValidJwtToken()
        {
            
            var user = new User
            {
                Id = 1,
                Username = "testuser",
                Email = "test@example.com",
                IsAdmin = false
            };

            
            var token = _authService.CreateToken(user);
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            
            Assert.NotNull(token);
            Assert.Contains(jwtToken.Claims, claim => claim.Type == ClaimTypes.NameIdentifier && claim.Value == user.Id.ToString());
            Assert.Contains(jwtToken.Claims, claim => claim.Type == ClaimTypes.Name && claim.Value == user.Username);
            Assert.Contains(jwtToken.Claims, claim => claim.Type == ClaimTypes.Email && claim.Value == user.Email);
            Assert.Contains(jwtToken.Claims, claim => claim.Type == ClaimTypes.Role && claim.Value == "User");
        }

        [Fact]
        public void CreateToken_WithAdminRole_ShouldIncludeAdminRoleClaim()
        {
            
            var user = new User
            {
                Id = 1,
                Username = "admin",
                Email = "admin@example.com",
                IsAdmin = true
            };
          
            var token = _authService.CreateToken(user);
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
          
            Assert.Contains(jwtToken.Claims, claim => claim.Type == ClaimTypes.Role && claim.Value == "Admin");
        }
    }
}