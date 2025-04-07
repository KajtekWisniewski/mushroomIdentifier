using Microsoft.EntityFrameworkCore;
using mushroomAPI.Data;

namespace mushroomAPI.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAppDbContext(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
        {
            if (environment.IsEnvironment("Test"))
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseInMemoryDatabase("InMemoryDbForTesting"));
            }
            else
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseNpgsql(configuration.GetConnectionString("Postgres")));
            }

            return services;
        }
    }
}
