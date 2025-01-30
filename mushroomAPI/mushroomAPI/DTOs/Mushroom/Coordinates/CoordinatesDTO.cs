namespace mushroomAPI.DTOs.Mushroom.Coordinates
{
    public class CoordinatesDTO
    {
        public int Id { get; init; }
        public double Latitude { get; init; }
        public double Longitude { get; init; }
        public int UserId { get; init; }
        public string Username { get; init; } = "";
    }
}
