using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using mushroomAPI.Data;
using mushroomAPI.Entities;

namespace mushroomAPI.ApiTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Test");

            builder.ConfigureAppConfiguration((context, configBuilder) =>
            {
                var testConfig = new Dictionary<string, string>
                {
                    { "AppSettings:Token", "super-test-token-1234567890" },
                    { "TestSettings:FixedAuthToken", "fixed-test-token-for-authentication" }
                };

                configBuilder.AddInMemoryCollection(testConfig);
            });

            builder.ConfigureServices(services =>
            {
                
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptor is not null)
                    services.Remove(descriptor);

                
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseInMemoryDatabase("InMemoryDbForTesting"));

                
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<CustomWebApplicationFactory>>();

                try
                {
                    db.Database.EnsureDeleted(); 
                    db.Database.EnsureCreated(); 
                    SeedTestDatabase(db);        
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred seeding the database: {Message}", ex.Message);
                }
            });
        }

        private void SeedTestDatabase(ApplicationDbContext context)
        {
            
            var users = new List<User>
            {
                new User
                {
                    Username = "testuser",
                    Email = "testuser@example.com",
                    PasswordHash = Convert.FromBase64String("6PaNQ+jAn8+GKj8mLpNL9U8Ki8YoGXYQJJqKTHVjh08qwJPC0yYLNz5v5QKmTnrYQO9KvOT5Ow6X2DQN9jNPoQ=="),
                    PasswordSalt = Convert.FromBase64String("r4BgGJidqPEOQUKV0qzf1j9VEHVhkYhaj8RGO2TzP/pxvKm45n0sUmEovO8ECHPqTSJJaZsfeUcD3e5RaJWx+tcfyg9lLWvE2QP6M4fyfz9ZU2UTg6GJtqMq8peCrOMXgpS3Mj0K0FTIv3fwyuYGiXgH+0s5DfXwAKqMgabRL1Q=")
                },
                new User
                {
                    Username = "admin",
                    Email = "admin@example.com",
                    IsAdmin = true,
                    PasswordHash = Convert.FromBase64String("6PaNQ+jAn8+GKj8mLpNL9U8Ki8YoGXYQJJqKTHVjh08qwJPC0yYLNz5v5QKmTnrYQO9KvOT5Ow6X2DQN9jNPoQ=="),
                    PasswordSalt = Convert.FromBase64String("r4BgGJidqPEOQUKV0qzf1j9VEHVhkYhaj8RGO2TzP/pxvKm45n0sUmEovO8ECHPqTSJJaZsfeUcD3e5RaJWx+tcfyg9lLWvE2QP6M4fyfz9ZU2UTg6GJtqMq8peCrOMXgpS3Mj0K0FTIv3fwyuYGiXgH+0s5DfXwAKqMgabRL1Q=")
                }
            };

            var mushrooms = new List<Mushroom>
            {
                new Mushroom
                {
                    Name = "Field Mushroom",
                    ScientificName = "Agaricus campestris",
                    Category = MushroomCategory.Agaricus,
                    Description = "Common edible mushroom found in grasslands",
                    IsEdible = true,
                    Habitat = "Meadows and fields",
                    Season = "Summer-Fall",
                    CommonNames = new List<string> { "Meadow Mushroom", "Pink Bottom" },
                    ImageUrls = new List<string> { "image1.jpg", "image2.jpg" },
                    LastUpdated = DateTime.UtcNow,
                    Locations = new List<Coordinates>
                    {
                        new Coordinates
                        {
                            Latitude = 51.5074,
                            Longitude = -0.1278,
                            UserId = 1,
                            Username = "testuser"
                        }
                    }
                },
                new Mushroom
                {
                    Name = "Death Cap",
                    ScientificName = "Amanita phalloides",
                    Category = MushroomCategory.Amanita,
                    Description = "One of the most poisonous mushrooms known",
                    IsEdible = false,
                    Habitat = "Woodland",
                    Season = "Summer-Fall",
                    CommonNames = new List<string> { "Death Cup", "Green Death Cap" },
                    ImageUrls = new List<string> { "image3.jpg", "image4.jpg" },
                    LastUpdated = DateTime.UtcNow,
                    Locations = new List<Coordinates>
                    {
                        new Coordinates
                        {
                            Latitude = 48.8566,
                            Longitude = 2.3522,
                            UserId = 2,
                            Username = "admin"
                        }
                    }
                }
            };

            var forumPosts = new List<ForumPost>
            {
                new ForumPost
                {
                    Content = "This is a post about Field Mushroom",
                    MushroomId = 1,
                    UserId = 1,
                    CreatedAt = DateTime.UtcNow.AddDays(-1)
                },
                new ForumPost
                {
                    Content = "This is a post about Death Cap",
                    MushroomId = 2,
                    UserId = 2,
                    CreatedAt = DateTime.UtcNow
                }
            };

            context.Users.AddRange(users);
            context.Mushrooms.AddRange(mushrooms);
            context.ForumPosts.AddRange(forumPosts);
            context.SaveChanges();
        }
    }
}