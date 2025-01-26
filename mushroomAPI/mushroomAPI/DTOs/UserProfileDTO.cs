using mushroomAPI.Entities;

namespace mushroomAPI.DTOs
{
    public class UserProfileDTO
    {
        public string Username { get; init; } = "";
        public string Email { get; init; } = "";
        public bool IsAdmin { get; init; }
        public List<MushroomCategory> SavedRecognitions { get; init; } = new List<MushroomCategory>();
    }
}
