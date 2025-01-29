using mushroomAPI.Entities;

namespace mushroomAPI.Repository.Contracts
{
    public interface IForumRepository
    {
        Task<IEnumerable<T>> GetAllByMushroomId<T>(int mushroomId);
        Task<IEnumerable<T>> GetAllByUserId<T>(int userId);
        Task<ForumPost?> GetById(int id);
        Task<T?> GetById<T>(int id);
        void Add(ForumPost post);
        void Update(ForumPost post);
        void Delete(ForumPost post);
        Task<bool> SafeChangesAsync();
    }
}
