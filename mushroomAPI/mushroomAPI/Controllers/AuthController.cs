using Microsoft.AspNetCore.Mvc;
using mushroomAPI.Data;
using mushroomAPI.DTOs;
using mushroomAPI.Entities;
using mushroomAPI.Services.IServices;
using Microsoft.EntityFrameworkCore;

namespace mushroomAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ApplicationDbContext _context;

        public AuthController(IAuthService authService, ApplicationDbContext context)
        {
            _authService = authService;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserProfileDTO>> Register(UserDTO request)
        {
            _authService.CreatePasswordHash(request.Password,
                out byte[] passwordHash,
                out byte[] passwordSalt);

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Register), new
            {
                username = user.Username,
                email = user.Email
            });
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserLoginDTO request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username);

            if (user == null)
                return BadRequest("User not found.");

            if (!_authService.VerifyPasswordHash(request.Password,
                user.PasswordHash,
                user.PasswordSalt))
                return BadRequest("Wrong password.");

            return Ok(_authService.CreateToken(user));
        }
    }
}
