using mushroomAPI.DTOs.Mushroom.Predictions;
using mushroomAPI.Entities;

namespace mushroomAPI.DTOs.User
{
    public class UserProfileDTO
    {
        public string Username { get; init; } = "";
        public string Email { get; init; } = "";
        public bool IsAdmin { get; init; }
        public List<RecognitionDTO> SavedRecognitions { get; init; } = new();
    }
}
