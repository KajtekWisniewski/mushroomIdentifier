using mushroomAPI.Entities;

namespace mushroomAPI.Repository.Contracts
{
    public interface IMushroomRepository
    {
        Task<IEnumerable<T>> GetAll<T>();
        Task<Mushroom?> GetById(int id);
        Task<T?> GetById<T>(int id);
        void Add(Mushroom mushroom);
        void Delete(Mushroom mushroom);
        void Update(Mushroom mushroom);
        Task<bool> SafeChangesAsync();
    }
}
