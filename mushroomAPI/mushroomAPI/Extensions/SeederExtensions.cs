using mushroomAPI.Data;
using mushroomAPI.Seeding;

namespace mushroomAPI.Extensions
{
    public static class SeederExtensions
    {
        public static async Task SeedDatabaseAsync(
            this WebApplication app,
            int minimumCount = 20,
            int numberToAdd = 100)
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var seeder = new DatabaseSeeder(context);
            await seeder.SeedDatabase(minimumCount, numberToAdd);
        }
    }
}
