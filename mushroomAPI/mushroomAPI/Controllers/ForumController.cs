using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mushroomAPI.DTOs;
using mushroomAPI.DTOs.Forum;
using mushroomAPI.Entities;
using mushroomAPI.Repository.Contracts;
using System.Security.Claims;

namespace mushroomAPI.Controllers
{
  [ApiController]
[Route("api/forum")]
public class ForumController : ControllerBase
{
    private readonly IForumRepository _repository;
    private readonly IMapper _mapper;

    public ForumController(IForumRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

        [HttpGet("mushroom/{mushroomId}")]
        public async Task<ActionResult<PagedList<ForumPostDTO>>> GetPostsByMushroom(
               int mushroomId,
               [FromQuery] int page = 1,
               [FromQuery] int pageSize = 10)
        {
            return Ok(await _repository.GetAllByMushroomIdPaginated<ForumPostDTO>(mushroomId, page, pageSize));
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<PagedList<ForumPostDTO>>> GetPostsByUser(
            int userId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            return Ok(await _repository.GetAllByUserIdPaginated<ForumPostDTO>(userId, page, pageSize));
        }

        [Authorize]
    [HttpPost]
    public async Task<ActionResult<ForumPostDTO>> CreatePost(CreateForumPostDTO postDto)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();

        var userId = int.Parse(userIdClaim);

        var post = new ForumPost
        {
            Content = postDto.Content,
            MushroomId = postDto.MushroomId,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        _repository.Add(post);

        return await _repository.SafeChangesAsync()
            ? CreatedAtAction(nameof(GetPostsByMushroom), new { mushroomId = post.MushroomId },
                await _repository.GetById<ForumPostDTO>(post.Id))
            : BadRequest();
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdatePost(int id, UpdateForumPostDTO postDto)
    {
        var post = await _repository.GetById(id);
        if (post == null) return NotFound();

        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();

        var userId = int.Parse(userIdClaim);
        if (post.UserId != userId) return Forbid();

        post.Content = postDto.Content;
        post.UpdatedAt = DateTime.UtcNow;
        _repository.Update(post);

        return await _repository.SafeChangesAsync() ? NoContent() : BadRequest();
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeletePost(int id)
    {
        var post = await _repository.GetById(id);
        if (post == null) return NotFound();

        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();

        var userId = int.Parse(userIdClaim);
        var isAdmin = User.IsInRole("Admin");

        if (post.UserId != userId && !isAdmin) return Forbid();

        _repository.Delete(post);
        return await _repository.SafeChangesAsync() ? NoContent() : BadRequest();
    }
}
}