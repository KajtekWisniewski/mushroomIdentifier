using AutoMapper;
using Microsoft.EntityFrameworkCore;
using mushroomAPI.Configuration;
using mushroomAPI.Data;
using mushroomAPI.DTOs.User;
using mushroomAPI.Entities;
using mushroomAPI.Repository;
using Xunit;

namespace mushroomAPI.UnitTests.Repository
{
    public class UserRepositoryTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;
        private readonly IMapper _mapper;

        public UserRepositoryTests()
        {

            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;


            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AutoMapperProfiles>();
            });
            _mapper = mapperConfig.CreateMapper();


            using var context = new ApplicationDbContext(_options);
            SeedDatabase(context);
        }

        private void SeedDatabase(ApplicationDbContext context)
        {

            var users = new List<User>
            {
                new User
                {
                    Id = 1,
                    Username = "user1",
                    Email = "user1@example.com",
                    IsAdmin = false,
                    PasswordHash = new byte[] { 1, 2, 3 },
                    PasswordSalt = new byte[] { 4, 5, 6 }
                },
                new User
                {
                    Id = 2,
                    Username = "admin",
                    Email = "admin@example.com",
                    IsAdmin = true,
                    PasswordHash = new byte[] { 1, 2, 3 },
                    PasswordSalt = new byte[] { 4, 5, 6 }
                }
            };


            var mushroom1 = new Mushroom
            {
                Id = 1,
                Name = "Field Mushroom",
                Locations = new List<Coordinates>
                {
                    new Coordinates
                    {
                        Id = 1,
                        Latitude = 51.5074,
                        Longitude = -0.1278,
                        UserId = 1,
                        Username = "user1"
                    }
                }
            };

            var mushroom2 = new Mushroom
            {
                Id = 2,
                Name = "Death Cap",
                Locations = new List<Coordinates>
                {
                    new Coordinates
                    {
                        Id = 2,
                        Latitude = 48.8566,
                        Longitude = 2.3522,
                        UserId = 1,
                        Username = "user1"
                    },
                    new Coordinates
                    {
                        Id = 3,
                        Latitude = 52.5200,
                        Longitude = 13.4050,
                        UserId = 2,
                        Username = "admin"
                    }
                }
            };

            context.Users.AddRange(users);
            context.Mushrooms.AddRange(new[] { mushroom1, mushroom2 });
            context.SaveChanges();
        }

        [Fact]
        public async Task GetAll_ReturnsAllUsers()
        {

            using var context = new ApplicationDbContext(_options);
            var repository = new UserRepository(context, _mapper);


            var result = await repository.GetAll<UserDTO>();


            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetById_WithValidId_ReturnsUser()
        {

            using var context = new ApplicationDbContext(_options);
            var repository = new UserRepository(context, _mapper);


            var result = await repository.GetById(1);


            Assert.NotNull(result);
            Assert.Equal("user1", result.Username);
            Assert.Equal("user1@example.com", result.Email);
        }

        [Fact]
        public async Task GetById_WithInvalidId_ReturnsNull()
        {

            using var context = new ApplicationDbContext(_options);
            var repository = new UserRepository(context, _mapper);


            var result = await repository.GetById(999);


            Assert.Null(result);
        }

        [Fact]
        public async Task Add_AddsUser()
        {

            using var context = new ApplicationDbContext(_options);
            var repository = new UserRepository(context, _mapper);
            var newUser = new User
            {
                Username = "newuser",
                Email = "newuser@example.com",
                PasswordHash = new byte[] { 1, 2, 3 },
                PasswordSalt = new byte[] { 4, 5, 6 }
            };


            repository.Add(newUser);
            await repository.SafeChangesAsync();


            var addedUser = await context.Users.FirstOrDefaultAsync(u => u.Username == "newuser");
            Assert.NotNull(addedUser);
            Assert.Equal("newuser@example.com", addedUser.Email);
        }

        [Fact]
        public async Task Update_UpdatesUser()
        {

            using var context = new ApplicationDbContext(_options);
            var repository = new UserRepository(context, _mapper);
            var user = await repository.GetById(1);
            Assert.NotNull(user);


            user.Email = "updated@example.com";
            repository.Update(user);
            await repository.SafeChangesAsync();


            var updatedUser = await context.Users.FindAsync(1);
            Assert.NotNull(updatedUser);
            Assert.Equal("updated@example.com", updatedUser.Email);
        }

        [Fact]
        public async Task Delete_RemovesUser()
        {

            using var context = new ApplicationDbContext(_options);
            var repository = new UserRepository(context, _mapper);
            var user = await repository.GetById(1);
            Assert.NotNull(user);


            repository.Delete(user);
            await repository.SafeChangesAsync();


            var deletedUser = await context.Users.FindAsync(1);
            Assert.Null(deletedUser);
        }

        [Fact]
        public async Task UpdateAdminStatus_ChangesAdminRole()
        {

            using var context = new ApplicationDbContext(_options);
            var repository = new UserRepository(context, _mapper);


            var result = await repository.UpdateAdminStatus(1, true);


            Assert.True(result);
            var user = await context.Users.FindAsync(1);
            Assert.NotNull(user);
            Assert.True(user.IsAdmin);
        }

        [Fact]
        public async Task GetUserLocations_ReturnsUserLocations()
        {

            using var context = new ApplicationDbContext(_options);
            var repository = new UserRepository(context, _mapper);


            var result = await repository.GetUserLocations(1);


            Assert.Equal(2, result.Count);
            Assert.Contains(result, l => l.MushroomId == 1);
            Assert.Contains(result, l => l.MushroomId == 2);
            Assert.All(result, l => Assert.Contains(l.Coordinates, c => c.UserId == 1));
        }
    }
}