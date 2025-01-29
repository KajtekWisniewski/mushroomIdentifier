using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mushroomAPI.Entities;
using mushroomAPI.Repository.Contracts;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using mushroomAPI.DTOs.User;
using mushroomAPI.DTOs.Mushroom.Predictions;

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
        [HttpGet("recognitions")]
        public async Task<ActionResult<UserRecognitionsDTO>> GetUserRecognitions()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            var userId = int.Parse(userIdClaim);
            var user = await _userRepository.GetById(userId);
            if (user == null) return NotFound();

            var recognitionsDto = new UserRecognitionsDTO
            {
                UserId = userId,
                SavedRecognitions = user.SavedRecognitions.Select(r => new RecognitionDTO
                {
                    Category = r.Category,
                    Confidence = r.Confidence,
                    SavedAt = r.SavedAt
                }).ToList()
            };

            return Ok(recognitionsDto);
        }

        [Authorize]
        [HttpPost("recognitions")]
        public async Task<ActionResult> SaveRecognitions(SaveRecognitionDTO recognitionDto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            var userId = int.Parse(userIdClaim);
            var user = await _userRepository.GetById(userId);
            if (user == null) return NotFound();

            var newRecognitions = recognitionDto.Predictions
                .OrderByDescending(p => double.Parse(p.Confidence.TrimEnd('%')))
                .Select(p => new Recognition
                {
                    Category = p.Category,
                    Confidence = p.Confidence,
                    SavedAt = DateTime.UtcNow
                })
                .ToList();

            user.SavedRecognitions.AddRange(newRecognitions);
            _userRepository.Update(user);

            return await _userRepository.SafeChangesAsync() ? NoContent() : BadRequest();
        }
    }
}
