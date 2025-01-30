using mushroomAPI.DTOs.Mushroom.Coordinates;

namespace mushroomAPI.DTOs.User
{
    public class UserLocationDTO
    {
        public List<CoordinatesDTO> Coordinates { get; init; } = new();
        public string MushroomName { get; init; } = "";
        public int MushroomId { get; init; }
        public List<string> MushroomImages { get; init; } = new();
    }
}
