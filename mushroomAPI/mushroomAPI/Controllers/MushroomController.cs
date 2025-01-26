using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mushroomAPI.DTOs;
using mushroomAPI.Entities;
using mushroomAPI.Repository.Contracts;

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
        public async Task<ActionResult<IEnumerable<MushroomDTO>>> GetAllMushrooms()
        {
            return Ok(await _repository.GetAll<MushroomDTO>());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MushroomDTO>> GetMushroomById(int id)
        {
            var mushroom = await _repository.GetById<MushroomDTO>(id);
            return mushroom == null ? NotFound() : Ok(mushroom);
        }

        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<MushroomDTO>>> GetMushroomsByCategory(MushroomCategory category)
        {
            var mushrooms = await _repository.GetAll<MushroomDTO>();
            return Ok(mushrooms.Where(m => m.Category == category));
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
        [HttpPost("{id}/coordinates")]
        public async Task<ActionResult> AddCoordinates(int id, Coordinates coordinates)
        {
            var mushroom = await _repository.GetById(id);
            if (mushroom == null) return NotFound();
            mushroom.Locations.Add(coordinates);
            return await _repository.SafeChangesAsync() ? NoContent() : BadRequest();
        }

        [HttpGet("mock")]
        public ActionResult GetMockPredictions()
        {
            var random = new Random();
            var categories = Enum.GetValues<MushroomCategory>()
                .OrderBy(x => random.Next())
                .Take(5)
                .Select(cat => new
                {
                    category = cat.ToString(),
                    confidence = $"{random.NextDouble() * 100:0.00}%"
                })
                .ToList();

            return Ok(new { predictions = categories });
        }
    }
}
