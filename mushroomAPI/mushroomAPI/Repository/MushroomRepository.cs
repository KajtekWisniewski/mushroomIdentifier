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
        public async Task<IEnumerable<T>> GetAll<T>()
        {
            return await context
                .Mushrooms
                .ProjectTo<T>(mapper.ConfigurationProvider)
                .ToListAsync();
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
