using mushroomAPI.DTOs;
using mushroomAPI.Entities;

namespace mushroomAPI.Repository.Contracts
{
    public interface IForumRepository
    {
        Task<PagedList<T>> GetAllByMushroomIdPaginated<T>(int mushroomId, int page, int pageSize);
        Task<PagedList<T>> GetAllByUserIdPaginated<T>(int userId, int page, int pageSize);
        Task<ForumPost?> GetById(int id);
        Task<T?> GetById<T>(int id);
        void Add(ForumPost post);
        void Update(ForumPost post);
        void Delete(ForumPost post);
        Task<bool> SafeChangesAsync();
    }
}
