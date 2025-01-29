using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using mushroomAPI.Data;
using mushroomAPI.DTOs;
using mushroomAPI.Entities;
using mushroomAPI.Repository.Contracts;


namespace mushroomAPI.Repository
{
    public class MushroomRepository(ApplicationDbContext context, IMapper mapper) : IMushroomRepository
    {
        public async Task<PagedList<T>> GetPaginated<T>(int page, int pageSize)
        {
            var query = context.Mushrooms.AsQueryable();
            var total = await query.CountAsync();

            var items = await query
                .OrderBy(m => m.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<T>(mapper.ConfigurationProvider)
                .ToListAsync();

            return new PagedList<T>
            {
                Items = items,
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = total,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            };
        }

        public async Task<PagedList<T>> GetPaginatedByCategory<T>(MushroomCategory category, int page, int pageSize)
        {
            var query = context.Mushrooms.Where(m => m.Category == category);
            var total = await query.CountAsync();

            var items = await query
                .OrderBy(m => m.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<T>(mapper.ConfigurationProvider)
                .ToListAsync();

            return new PagedList<T>
            {
                Items = items,
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = total,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            };
        }

        public void Add(Mushroom mushroom)
        {
            context.Mushrooms.Add(mushroom);
        }

        public void Update(Mushroom mushroom)
        {
            context.Mushrooms.Update(mushroom);
        }

        public void Delete(Mushroom mushroom)
        {
            context.Mushrooms.Remove(mushroom);
        }

        public async Task<Mushroom?> GetById(int id)
        {
            return await context.Mushrooms.FindAsync(id);
        }

        public async Task<T?> GetById<T>(int id)
        {
            return await context
                .Mushrooms
                .Where(p => p.Id == id)
                .ProjectTo<T>(mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }

        public async Task<bool> SafeChangesAsync()
        {
            return await context.SaveChangesAsync() > 0;
        }
    }
}
