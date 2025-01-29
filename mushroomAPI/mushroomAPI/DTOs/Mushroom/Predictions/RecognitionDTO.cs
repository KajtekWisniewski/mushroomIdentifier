using mushroomAPI.Entities;

namespace mushroomAPI.DTOs.Mushroom.Predictions
{
    public class RecognitionDTO
    {
        public MushroomCategory Category { get; init; }
        public string Confidence { get; init; } = "";
        public DateTime SavedAt { get; init; }
        public string BatchId { get; set; } = "";
    }
}
