namespace mushroomAPI.Entities
{
    public class Recognition
    {
        public MushroomCategory Category { get; set; }
        public string Confidence { get; set; } = "";
        public DateTime SavedAt { get; set; }
    }
}
