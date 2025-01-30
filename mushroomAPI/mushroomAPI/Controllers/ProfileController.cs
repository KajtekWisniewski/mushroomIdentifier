using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mushroomAPI.DTOs;
using mushroomAPI.DTOs.User;
using mushroomAPI.DTOs.Mushroom.Predictions;
using mushroomAPI.Entities;
using mushroomAPI.Repository.Contracts;
using System.Security.Claims;

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
                    user.IsAdmin
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
        public async Task<ActionResult<PagedList<RecognitionBatchDTO>>> GetUserRecognitions(
           [FromQuery] int page = 1,
           [FromQuery] int pageSize = 5)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            var userId = int.Parse(userIdClaim);
            var user = await _userRepository.GetById(userId);
            if (user == null) return NotFound();

            var groupedRecognitions = user.SavedRecognitions
                .GroupBy(r => r.BatchId)
                .OrderByDescending(g => g.First().SavedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(g => new RecognitionBatchDTO
                {
                    BatchId = g.Key,
                    SavedAt = g.First().SavedAt,
                    Predictions = g.OrderByDescending(r => double.Parse(r.Confidence.TrimEnd('%')))
                        .Select(r => _mapper.Map<RecognitionDTO>(r))
                        .ToList()
                })
                .ToList();

            var totalBatches = user.SavedRecognitions
                .GroupBy(r => r.BatchId)
                .Count();

            return Ok(new PagedList<RecognitionBatchDTO>
            {
                Items = groupedRecognitions,
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalBatches,
                TotalPages = (int)Math.Ceiling(totalBatches / (double)pageSize)
            });
        }

        [Authorize]
        [HttpDelete("recognitions/{batchId}")]
        public async Task<ActionResult> DeleteRecognitionBatch(string batchId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            var userId = int.Parse(userIdClaim);
            var user = await _userRepository.GetById(userId);
            if (user == null) return NotFound();

            user.SavedRecognitions = user.SavedRecognitions
                .Where(r => r.BatchId != batchId)
                .ToList();

            _userRepository.Update(user);

            return await _userRepository.SafeChangesAsync() ? NoContent() : BadRequest();
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

            var batchId = Guid.NewGuid().ToString();
            var savedAt = DateTime.UtcNow;

            var newRecognitions = recognitionDto.Predictions
                .OrderByDescending(p => double.Parse(p.Confidence.TrimEnd('%')))
                .Select(p => new Recognition
                {
                    Category = p.Category,
                    Confidence = p.Confidence,
                    SavedAt = savedAt,
                    BatchId = batchId
                })
                .ToList();

            user.SavedRecognitions.AddRange(newRecognitions);
            _userRepository.Update(user);

            return await _userRepository.SafeChangesAsync() ? NoContent() : BadRequest();
        }

        [HttpGet("locations")]
        [Authorize]
        public async Task<ActionResult<List<UserLocationDTO>>> GetUserLocations()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            var userId = int.Parse(userIdClaim);
            var locations = await _userRepository.GetUserLocations(userId);

            return Ok(locations);
        }
    }
}