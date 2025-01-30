namespace mushroomAPI.Entities
{
    public class Coordinates
    {
        public int Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; } = "";
    }
}
