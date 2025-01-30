using mushroomAPI.DTOs;
using mushroomAPI.Entities;

namespace mushroomAPI.Repository.Contracts
{
    public interface IMushroomRepository
    {
        Task<PagedList<T>> GetPaginated<T>(int page, int pageSize);
        Task<PagedList<T>> GetPaginatedByCategory<T>(MushroomCategory category, int page, int pageSize);
        Task<PagedList<T>> SearchMushrooms<T>(
           string? searchTerm,
           MushroomCategory? category = null,
           string? season = null,
           bool? isEdible = null,
           int page = 1,
           int pageSize = 10
        );
        Task<Mushroom?> GetById(int id);
        Task<T?> GetById<T>(int id);
        void Add(Mushroom mushroom);
        void Delete(Mushroom mushroom);
        void Update(Mushroom mushroom);
        Task<bool> SafeChangesAsync();
    }
}
