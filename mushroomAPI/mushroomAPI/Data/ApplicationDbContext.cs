using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using mushroomAPI.Entities;
using System.Text.Json;

namespace mushroomAPI.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<Mushroom> Mushrooms { get; init; } = null!;
        public DbSet<User> Users { get; init; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var fixedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var jsonOptions = new JsonSerializerOptions();

            modelBuilder.Entity<Mushroom>()
               .HasMany(m => m.Locations)
               .WithOne()
               .HasForeignKey("MushroomId");

            modelBuilder.Entity<Mushroom>()
           .Property(m => m.CommonNames)
           .HasConversion(
               v => JsonSerializer.Serialize(v ?? new List<string>(), jsonOptions),
               v => JsonSerializer.Deserialize<List<string>>(v, jsonOptions) ?? new List<string>(),
               new ValueComparer<List<string>>(
                   (c1, c2) => (c1 ?? new List<string>()).SequenceEqual(c2 ?? new List<string>()),
                   c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                   c => c.ToList()));

            modelBuilder.Entity<Mushroom>()
                .Property(m => m.ImageUrls)
                .HasConversion(
                    v => JsonSerializer.Serialize(v ?? new List<string>(), jsonOptions),
                    v => JsonSerializer.Deserialize<List<string>>(v, jsonOptions) ?? new List<string>(),
                    new ValueComparer<List<string>>(
                        (c1, c2) => (c1 ?? new List<string>()).SequenceEqual(c2 ?? new List<string>()),
                        c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                        c => c.ToList()));

            modelBuilder.Entity<User>()
                .Property(u => u.SavedRecognitions)
                .HasConversion(
                    v => JsonSerializer.Serialize(v ?? new List<MushroomCategory>(), jsonOptions),
                    v => JsonSerializer.Deserialize<List<MushroomCategory>>(v, jsonOptions) ?? new List<MushroomCategory>(),
                    new ValueComparer<List<MushroomCategory>>(
                        (c1, c2) => (c1 ?? new List<MushroomCategory>()).SequenceEqual(c2 ?? new List<MushroomCategory>()),
                        c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                        c => c.ToList()));

            modelBuilder.Entity<Coordinates>().HasData(
                new { Id = 1, Latitude = 51.5074, Longitude = -0.1278, MushroomId = 3 },
                new { Id = 2, Latitude = 48.8566, Longitude = 2.3522, MushroomId = 3 },
                new { Id = 3, Latitude = 52.5200, Longitude = 13.4050, MushroomId = 2 },
                new { Id = 4, Latitude = 45.4215, Longitude = -75.6972, MushroomId = 2 },
                new { Id = 5, Latitude = 41.9028, Longitude = 12.4964, MushroomId = 1 },
                new { Id = 6, Latitude = 59.9139, Longitude = 10.7522, MushroomId = 1 }
            );

            modelBuilder.Entity<Mushroom>().HasData(new List<Mushroom> {
                  new Mushroom {
                        Id = 1,
                        Name = "Field Mushroom",
                        ScientificName = "Agaricus campestris",
                        Category = MushroomCategory.Agaricus,
                        Description = "Common edible mushroom found in grasslands",
                        IsEdible = true,
                        Habitat = "Meadows and fields",
                        Season = "Summer-Fall",
                        CommonNames = new List<string> { "Meadow Mushroom", "Pink Bottom" },
                        ImageUrls = new List<string> {
                            "https://upload.wikimedia.org/wikipedia/commons/thumb/3/3a/Agaricus_campestris.jpg/1200px-Agaricus_campestris.jpg",
                            "https://upload.wikimedia.org/wikipedia/commons/d/d7/Agaricus-campestris-michoacan.jpg"
                        },
                        LastUpdated = fixedDate
                  },
                   new Mushroom {
                       Id = 2,
                       Name = "Death Cap",
                       ScientificName = "Amanita phalloides",
                       Category = MushroomCategory.Amanita,
                       Description = "One of the most poisonous mushrooms known",
                       IsEdible = false,
                       Habitat = "Woodland",
                       Season = "Summer-Fall",
                       CommonNames = new List<string> { "Death Cup", "Green Death Cap" },
                       ImageUrls = new List<string> {
                            "https://upload.wikimedia.org/wikipedia/commons/thumb/9/99/Amanita_phalloides_1.JPG/800px-Amanita_phalloides_1.JPG",
                            "https://foodsafety.osu.edu/sites/cfi/files/imgclean/711-body-1692292442-1.jpg"
                       },
                       LastUpdated = fixedDate
                   },
                   new Mushroom {
                       Id = 3,
                       Name = "Penny Bun",
                       ScientificName = "Boletus edulis",
                       Category = MushroomCategory.Boletus,
                       Description = "Prized edible mushroom with thick stem",
                       IsEdible = true,
                       Habitat = "Mixed woodland",
                       Season = "Late Summer-Fall",
                       CommonNames = new List<string> { "Porcini", "King Bolete" },
                       ImageUrls = new List<string> {
                            "https://upload.wikimedia.org/wikipedia/commons/3/34/Boletus_edulis_IT.jpg",
                            "https://i.ytimg.com/vi/m1yqWtcPFSQ/maxresdefault.jpg"
                       },
                       LastUpdated = fixedDate
                   }
            });
            modelBuilder.Entity<User>().HasData(new List<User> {
                    new User {
                        Id = 1,
                        Username = "admin",
                        Email = "admin@example.com",
                        IsAdmin = true,
                        PasswordHash = Convert.FromBase64String("6PaNQ+jAn8+GKj8mLpNL9U8Ki8YoGXYQJJqKTHVjh08qwJPC0yYLNz5v5QKmTnrYQO9KvOT5Ow6X2DQN9jNPoQ=="),
                        PasswordSalt = Convert.FromBase64String("r4BgGJidqPEOQUKV0qzf1j9VEHVhkYhaj8RGO2TzP/pxvKm45n0sUmEovO8ECHPqTSJJaZsfeUcD3e5RaJWx+tcfyg9lLWvE2QP6M4fyfz9ZU2UTg6GJtqMq8peCrOMXgpS3Mj0K0FTIv3fwyuYGiXgH+0s5DfXwAKqMgabRL1Q=")
                    },
                    new User {
                        Id = 2,
                        Username = "user",
                        Email = "user@example.com",
                        IsAdmin = false,
                        PasswordHash = Convert.FromBase64String("6PaNQ+jAn8+GKj8mLpNL9U8Ki8YoGXYQJJqKTHVjh08qwJPC0yYLNz5v5QKmTnrYQO9KvOT5Ow6X2DQN9jNPoQ=="),
                        PasswordSalt = Convert.FromBase64String("r4BgGJidqPEOQUKV0qzf1j9VEHVhkYhaj8RGO2TzP/pxvKm45n0sUmEovO8ECHPqTSJJaZsfeUcD3e5RaJWx+tcfyg9lLWvE2QP6M4fyfz9ZU2UTg6GJtqMq8peCrOMXgpS3Mj0K0FTIv3fwyuYGiXgH+0s5DfXwAKqMgabRL1Q=")
                    }
            });
            base.OnModelCreating(modelBuilder);
        }
    }
}
