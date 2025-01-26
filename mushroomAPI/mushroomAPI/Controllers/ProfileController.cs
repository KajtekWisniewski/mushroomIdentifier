using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mushroomAPI.DTOs;
using mushroomAPI.Entities;
using mushroomAPI.Repository.Contracts;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace mushroomAPI.Controllers
{
    [ApiController]
    [Route("api/profile")]
    public class ProfileController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public ProfileController(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserProfileDTO>> GetProfile(int id)
        {
            var user = await _userRepository.GetById<UserProfileDTO>(id);
            if (user == null) return NotFound();

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            bool isOwnProfile = currentUserId != null && currentUserId == id.ToString();

            if (isOwnProfile)
            {
                return Ok(new
                {
                    user.Username,
                    user.Email,
                    user.IsAdmin,
                    user.SavedRecognitions
                });
            }

            return Ok(new
            {
                user.Username,
                user.IsAdmin
            });
        }

        [Authorize]
        [HttpPut("recognitions")]
        public async Task<ActionResult> UpdateRecognitions(UserRecognitionsDTO recognitionsDto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            var userId = int.Parse(userIdClaim);
            if (userId != recognitionsDto.UserId) return Forbid();

            var user = await _userRepository.GetById(userId);
            if (user == null) return NotFound();

            user.SavedRecognitions = recognitionsDto.SavedRecognitions;
            _userRepository.Update(user);
            return await _userRepository.SafeChangesAsync() ? NoContent() : BadRequest();
        }
    }
}
