namespace mushroomAPI.DTOs.Mushroom.Predictions
{
    public class SaveRecognitionDTO
    {
        public List<RecognitionDTO> Predictions { get; init; } = new();
    }
}
