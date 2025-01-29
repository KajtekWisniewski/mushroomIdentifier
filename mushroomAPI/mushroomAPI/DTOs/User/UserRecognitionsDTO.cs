using mushroomAPI.DTOs.Mushroom.Predictions;
using mushroomAPI.Entities;

namespace mushroomAPI.DTOs.User
{
    public class UserRecognitionsDTO
    {
        public int UserId { get; init; }
        public List<RecognitionDTO> SavedRecognitions { get; init; } = new();
    }
}
