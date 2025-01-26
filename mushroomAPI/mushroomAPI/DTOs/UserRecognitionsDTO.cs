using mushroomAPI.Entities;

namespace mushroomAPI.DTOs
{
    public class UserRecognitionsDTO
    {
        public int UserId { get; init; }
        public List<MushroomCategory> SavedRecognitions { get; init; } = new List<MushroomCategory>();
    }
}
