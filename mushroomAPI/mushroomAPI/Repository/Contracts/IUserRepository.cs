using mushroomAPI.Entities;
using mushroomAPI.DTOs.User;

namespace mushroomAPI.Repository.Contracts
{
    public interface IUserRepository
    {
        Task<IEnumerable<T>> GetAll<T>();
        Task<User?> GetById(int id);
        Task<T?> GetById<T>(int id);
        void Add(User user);
        void Delete(User user);
        void Update(User user);
        void DeleteById(int id);
        void UpdateById(int id, User user);
        Task<bool> UpdateAdminStatus(int userId, bool isAdmin);
        Task<List<UserLocationDTO>> GetUserLocations(int userId);
        Task<bool> SafeChangesAsync();
    }
}
