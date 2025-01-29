using Microsoft.AspNetCore.Mvc;
using mushroomAPI.Data;
using mushroomAPI.Entities;
using mushroomAPI.Services.IServices;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using mushroomAPI.DTOs.Auth;
using mushroomAPI.DTOs.User;

namespace mushroomAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public AuthController(IAuthService authService, ApplicationDbContext context, IMapper mapper)
        {
            _authService = authService;
            _context = context;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserProfileDTO>> Register(UserDTO request)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username || u.Email == request.Email);

            if (existingUser != null)
            {
                return Conflict(new { message = "A user with the same username or email already exists." });
            }

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

            var userProfileDTO = _mapper.Map<UserProfileDTO>(user);

            return CreatedAtAction(nameof(Register), userProfileDTO);
        }


        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDTO>> Login(UserLoginDTO request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username);

            if (user == null)
                return BadRequest("User not found.");

            if (!_authService.VerifyPasswordHash(request.Password,
                user.PasswordHash,
                user.PasswordSalt))
                return BadRequest("Wrong password.");

            var token = _authService.CreateToken(user);

            var userProfileDTO = _mapper.Map<UserProfileDTO>(user);

            var loginResponse = new LoginResponseDTO
            {
                Token = token,
                User = userProfileDTO
            };

            return Ok(loginResponse);
        }
    }
}