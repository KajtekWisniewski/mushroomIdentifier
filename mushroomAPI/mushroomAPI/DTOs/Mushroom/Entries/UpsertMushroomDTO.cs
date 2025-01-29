using mushroomAPI.Entities;

namespace mushroomAPI.DTOs.Mushroom.Entries
{
    public class UpsertMushroomDTO
    {
        public string Name { get; init; } = "";
        public string ScientificName { get; init; } = "";
        public MushroomCategory Category { get; init; }
        public string Description { get; init; } = "";
        public bool IsEdible { get; init; }
        public string Habitat { get; init; } = "";
        public string Season { get; init; } = "";
        public List<string> CommonNames { get; init; } = new List<string>();
        public List<string> ImageUrls { get; init; } = new List<string>();
        public List<Coordinates> Locations { get; init; } = new List<Coordinates>();
        public DateTime LastUpdated { get; init; }
    }
}
