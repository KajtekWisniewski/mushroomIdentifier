namespace mushroomAPI.Entities
{
   
    public class Mushroom
    {
        
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string ScientificName { get; set; } = "";
        public MushroomCategory Category { get; set; }
        public string Description { get; set; } = "";
        public bool IsEdible { get; set; }
        public string Habitat { get; set; } = "";
        public string Season { get; set; } = "";
        public List<string> CommonNames { get; set; } = new List<string>();
        public List<string> ImageUrls { get; set; } = new List<string>();
        public List<Coordinates> Locations { get; set; } = new List<Coordinates>();
        public DateTime LastUpdated { get; set; }
    
    }
}
