namespace mushroomAPI.DTOs.Mushroom.Location
{
    public class LocationDTO
    {
        public int Id { get; init; }
        public double Latitude { get; init; }
        public double Longitude { get; init; }
        public int UserId { get; init; }
        public string Username { get; init; } = "";
    }
}
