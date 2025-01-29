using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using mushroomAPI.Data;
using mushroomAPI.Entities;
using mushroomAPI.Repository.Contracts;

namespace mushroomAPI.Repository
{
    public class ForumRepository : IForumRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ForumRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<T>> GetAllByMushroomId<T>(int mushroomId)
        {
            return await _context.ForumPosts
                .Where(p => p.MushroomId == mushroomId)
                .OrderByDescending(p => p.CreatedAt)
                .ProjectTo<T>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllByUserId<T>(int userId)
        {
            return await _context.ForumPosts
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .ProjectTo<T>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<ForumPost?> GetById(int id)
        {
            return await _context.ForumPosts.FindAsync(id);
        }

        public async Task<T?> GetById<T>(int id)
        {
            return await _context.ForumPosts
                .Where(p => p.Id == id)
                .ProjectTo<T>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }

        public void Add(ForumPost post)
        {
            _context.ForumPosts.Add(post);
        }

        public void Update(ForumPost post)
        {
            _context.ForumPosts.Update(post);
        }

        public void Delete(ForumPost post)
        {
            _context.ForumPosts.Remove(post);
        }

        public async Task<bool> SafeChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}