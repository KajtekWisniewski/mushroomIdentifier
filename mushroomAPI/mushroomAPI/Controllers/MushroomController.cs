using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mushroomAPI.DTOs;
using mushroomAPI.DTOs.Mushroom.Coordinates;
using mushroomAPI.DTOs.Mushroom.Entries;
using mushroomAPI.DTOs.Mushroom.Predictions;
using mushroomAPI.Entities;
using mushroomAPI.Repository.Contracts;
using System.Security.Claims;

namespace mushroomAPI.Controllers
{
    [ApiController]
    [Route("api/mushrooms")]
    public class MushroomController : ControllerBase
    {
        private readonly IMushroomRepository _repository;
        private readonly IMapper _mapper;
        public MushroomController(IMushroomRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<MushroomDTO>>> GetAllMushrooms(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            return Ok(await _repository.GetPaginated<MushroomDTO>(page, pageSize));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MushroomDTO>> GetMushroomById(int id)
        {
            var mushroom = await _repository.GetById<MushroomDTO>(id);
            return mushroom == null ? NotFound() : Ok(mushroom);
        }

        [HttpGet("category/{category}")]
        public async Task<ActionResult<PagedList<MushroomDTO>>> GetMushroomsByCategory(
            MushroomCategory category,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            return Ok(await _repository.GetPaginatedByCategory<MushroomDTO>(category, page, pageSize));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<MushroomDTO>> CreateMushroom(UpsertMushroomDTO mushroomDto)
        {
            var mushroom = _mapper.Map<Mushroom>(mushroomDto);
            _repository.Add(mushroom);

            return await _repository.SafeChangesAsync()
                ? CreatedAtAction(nameof(GetMushroomById), new { id = mushroom.Id }, mushroom)
                : BadRequest();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateMushroom(int id, UpsertMushroomDTO mushroomDto)
        {
            var mushroom = await _repository.GetById(id);
            if (mushroom == null) return NotFound();
            _mapper.Map(mushroomDto, mushroom);
            _repository.Update(mushroom);

            return await _repository.SafeChangesAsync() ? NoContent() : BadRequest();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMushroom(int id)
        {
            var mushroom = await _repository.GetById(id);
            if (mushroom == null) return NotFound();
            _repository.Delete(mushroom);
            return await _repository.SafeChangesAsync() ? NoContent() : BadRequest();
        }

        [Authorize]
        [HttpDelete("{mushroomId}/coordinates/{locationId}")]
        public async Task<ActionResult> DeleteCoordinates(int mushroomId, int locationId)
        {
            var mushroom = await _repository.GetById(mushroomId);
            if (mushroom == null) return NotFound();

            var location = mushroom.Locations.FirstOrDefault(l => l.Id == locationId);
            if (location == null) return NotFound();

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            if (location.UserId != userId && !User.IsInRole("Admin"))
                return Forbid();

            mushroom.Locations.Remove(location);
            return await _repository.SafeChangesAsync() ? NoContent() : BadRequest();
        }

        [Authorize]
        [HttpPost("{id}/coordinates")]
        public async Task<ActionResult> AddCoordinates(int id, SaveCoordinatesDTO coordinates)
        {
            var mushroom = await _repository.GetById(id);
            if (mushroom == null) return NotFound();

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var username = User.FindFirst(ClaimTypes.Name)?.Value!;

            var location = new Coordinates
            {
                Latitude = coordinates.Latitude,
                Longitude = coordinates.Longitude,
                UserId = userId,
                Username = username
            };

            mushroom.Locations.Add(location);
            return await _repository.SafeChangesAsync() ? NoContent() : BadRequest();
        }

        [HttpGet("mock")]
        public ActionResult GetMockPredictions()
        {
            var random = new Random();
            var categories = Enum.GetValues<MushroomCategory>()
                .OrderBy(x => random.Next())
                .Take(5)
                .Select(cat => new RecognitionDTO
                {
                    Category = cat,
                    Confidence = $"{random.NextDouble() * 100:0.00}%"
                })
                .ToList();

            return Ok(new { predictions = categories });
        }
    }
}
