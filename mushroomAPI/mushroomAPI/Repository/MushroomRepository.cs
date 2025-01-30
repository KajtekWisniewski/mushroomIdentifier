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
            return await context.Mushrooms
                .Include(m => m.Locations)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<T?> GetById<T>(int id)
        {
            return await context
                .Mushrooms
                .Where(p => p.Id == id)
                .ProjectTo<T>(mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }

        public async Task<PagedList<T>> SearchMushrooms<T>(
         string? searchTerm,
         MushroomCategory? category = null,
         string? season = null,
         bool? isEdible = null,
         int page = 1,
         int pageSize = 10)
        {
            var query = context.Mushrooms.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(m =>
                    EF.Functions.ILike(m.Name, $"%{searchTerm}%") ||
                    EF.Functions.ILike(m.ScientificName, $"%{searchTerm}%")
                );
            }

            if (category.HasValue)
            {
                query = query.Where(m => m.Category == category.Value);
            }

            if (!string.IsNullOrWhiteSpace(season))
            {
                season = season.ToLower();
                query = query.Where(m => EF.Functions.ILike(m.Season, $"%{season}%"));
            }

            if (isEdible.HasValue)
            {
                query = query.Where(m => m.IsEdible == isEdible.Value);
            }

            var total = await query.CountAsync();

            var mushrooms = await query
                .OrderBy(m => m.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var commonNamesResults = (await context.Mushrooms
                 .ToListAsync())
                 .Where(m => m.CommonNames.Any(cn => cn.ToLower().Contains(searchTerm)))
                 .Take(pageSize)
                 .ToList();


                mushrooms = mushrooms.Union(commonNamesResults)
                    .Take(pageSize)
                    .ToList();
            }

            var items = mushrooms.Select(m => mapper.Map<T>(m)).ToList();

            return new PagedList<T>
            {
                Items = items,
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = total,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            };
        }

        public async Task<bool> SafeChangesAsync()
        {
            return await context.SaveChangesAsync() > 0;
        }
    }
}
