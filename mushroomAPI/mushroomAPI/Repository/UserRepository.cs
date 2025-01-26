using AutoMapper;
using mushroomAPI.Data;
using mushroomAPI.Entities;
using mushroomAPI.Repository.Contracts;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace mushroomAPI.Repository
{
    public class UserRepository(ApplicationDbContext context, IMapper mapper) : IUserRepository
    {
        public void Add(User user)
        {
            context.Users.Add(user);
        }

        public void Delete(User user)
        {
            context.Users.Remove(user);
        }

        public void DeleteById(int id)
        {
            var user = context.Users.Find(id);
            if (user != null)
                context.Users.Remove(user);
        }

        public async Task<IEnumerable<T>> GetAll<T>()
        {
            return await context
                .Users
                .ProjectTo<T>(mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<User?> GetById(int id)
        {
            return await context.Users.FindAsync(id);
        }

        public async Task<T?> GetById<T>(int id)
        {
            return await context
                .Users
                .Where(u => u.Id == id)
                .ProjectTo<T>(mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }

        public void Update(User user)
        {
            context.Users.Update(user);
        }

        public void UpdateById(int id, User user)
        {
            var existingUser = context.Users.Find(id);
            if (existingUser != null)
            {
                user.Id = id;
                context.Entry(existingUser).CurrentValues.SetValues(user);
            }
        }

        public async Task<bool> UpdateAdminStatus(int userId, bool isAdmin)
        {
            var user = await context.Users.FindAsync(userId);
            if (user == null) return false;

            user.IsAdmin = isAdmin;
            return await SafeChangesAsync();
        }

        public async Task<bool> SafeChangesAsync()
        {
            return await context.SaveChangesAsync() > 0;
        }
    }
}
