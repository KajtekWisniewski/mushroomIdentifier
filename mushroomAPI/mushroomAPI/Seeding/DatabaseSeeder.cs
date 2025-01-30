using mushroomAPI.Data;
using mushroomAPI.Entities;
using Bogus;
using Microsoft.EntityFrameworkCore;

namespace mushroomAPI.Seeding
{
    public class DatabaseSeeder
    {
        private readonly ApplicationDbContext _context;
        public DatabaseSeeder(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SeedDatabase(int minimumCount = 20, int numberToAdd = 100)
        {
            var currentCount = await _context.Mushrooms.CountAsync();
            if (currentCount >= minimumCount)
                return;

            var adminUser = await _context.Users.FirstOrDefaultAsync(u => u.IsAdmin);
            if (adminUser == null)
            {
                throw new InvalidOperationException("No admin user found in database to assign coordinates to");
            }

            var mushroomFaker = new Faker<Mushroom>()
                .RuleFor(m => m.Name, f => f.Commerce.ProductName())
                .RuleFor(m => m.ScientificName, f => $"{f.Lorem.Word()} {f.Lorem.Word()}")
                .RuleFor(m => m.Category, f => f.PickRandom<MushroomCategory>())
                .RuleFor(m => m.Description, f => f.Lorem.Paragraphs(1))
                .RuleFor(m => m.IsEdible, f => f.Random.Bool(0.7f))
                .RuleFor(m => m.Habitat, f => f.PickRandom(new[] {
                "Deciduous forest", "Coniferous forest", "Mixed forest", "Alpine meadow",
                "Urban parks", "Mountain slopes", "Wetlands", "Grassland",
                "Oak woodland", "Pine forest", "Beech forest", "Garden areas"
                }))
                .RuleFor(m => m.Season, f => f.PickRandom(new[] {
                "Spring", "Summer", "Fall", "Winter",
                "Spring-Summer", "Summer-Fall", "Fall-Winter",
                "Late Spring", "Early Summer", "Late Summer", "Early Fall"
                }))
                .RuleFor(m => m.CommonNames, f => f.Make(f.Random.Int(2, 5), () =>
                    f.Commerce.ProductName()).Distinct().ToList())
                .RuleFor(m => m.ImageUrls, f => new List<string> {
                "https://media.istockphoto.com/id/1147544807/vector/thumbnail-image-vector-graphic.jpg?s=612x612&w=0&k=20&c=rnCKVbdxqkjlcs3xH87-9gocETqpspHFXu5dIGB4wuM=",
                "https://media.istockphoto.com/id/1147544807/vector/thumbnail-image-vector-graphic.jpg?s=612x612&w=0&k=20&c=rnCKVbdxqkjlcs3xH87-9gocETqpspHFXu5dIGB4wuM=",
                "https://media.istockphoto.com/id/1147544807/vector/thumbnail-image-vector-graphic.jpg?s=612x612&w=0&k=20&c=rnCKVbdxqkjlcs3xH87-9gocETqpspHFXu5dIGB4wuM="
                }.Where(url => url != null).ToList())
                .RuleFor(m => m.Locations, f => f.Make(f.Random.Int(2, 6), () =>
                    new Coordinates
                    {
                        Latitude = f.Random.Double(-90, 90),
                        Longitude = f.Random.Double(-180, 180),
                        UserId = adminUser.Id,
                        Username = adminUser.Username 
                    }).ToList())
                .RuleFor(m => m.LastUpdated, f => DateTime.SpecifyKind(f.Date.Past(1), DateTimeKind.Utc));

            var mushrooms = mushroomFaker.Generate(numberToAdd);
            await _context.Mushrooms.AddRangeAsync(mushrooms);
            await _context.SaveChangesAsync();
            Console.WriteLine($"Added {numberToAdd} mushrooms. Total count is now {currentCount + numberToAdd}");
        }
    }
}
