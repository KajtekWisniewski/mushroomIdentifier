namespace mushroomAPI.DTOs.Mushroom.Predictions
{
    public class RecognitionBatchDTO
    {
        public string BatchId { get; init; } = "";
        public DateTime SavedAt { get; init; }
        public List<RecognitionDTO> Predictions { get; init; } = new();
    }
}
