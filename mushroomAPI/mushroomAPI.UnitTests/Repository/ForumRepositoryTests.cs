using AutoMapper;
using Microsoft.EntityFrameworkCore;
using mushroomAPI.Configuration;
using mushroomAPI.Data;
using mushroomAPI.DTOs.Forum;
using mushroomAPI.Entities;
using mushroomAPI.Repository;
using Xunit;

namespace mushroomAPI.UnitTests.Repository
{
    public class ForumRepositoryTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;
        private readonly IMapper _mapper;

        public ForumRepositoryTests()
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
                    Email = "user1@example.com"
                },
                new User
                {
                    Id = 2,
                    Username = "user2",
                    Email = "user2@example.com"
                }
            };

            
            var mushrooms = new List<Mushroom>
            {
                new Mushroom
                {
                    Id = 1,
                    Name = "Field Mushroom",
                    ScientificName = "Agaricus campestris",
                    Category = MushroomCategory.Agaricus
                },
                new Mushroom
                {
                    Id = 2,
                    Name = "Death Cap",
                    ScientificName = "Amanita phalloides",
                    Category = MushroomCategory.Amanita
                }
            };

            
            var posts = new List<ForumPost>
            {
                new ForumPost
                {
                    Id = 1,
                    Content = "This is a post about Field Mushroom",
                    MushroomId = 1,
                    UserId = 1,
                    CreatedAt = DateTime.UtcNow.AddDays(-2)
                },
                new ForumPost
                {
                    Id = 2,
                    Content = "Another post about Field Mushroom",
                    MushroomId = 1,
                    UserId = 2,
                    CreatedAt = DateTime.UtcNow.AddDays(-1)
                },
                new ForumPost
                {
                    Id = 3,
                    Content = "Post about Death Cap",
                    MushroomId = 2,
                    UserId = 1,
                    CreatedAt = DateTime.UtcNow
                }
            };

            context.Users.AddRange(users);
            context.Mushrooms.AddRange(mushrooms);
            context.SaveChanges();
            
            context.ForumPosts.AddRange(posts);
            context.SaveChanges();
        }

        [Fact]
        public async Task GetAllByMushroomIdPaginated_ReturnsCorrectPosts()
        {
            
            using var context = new ApplicationDbContext(_options);
            var repository = new ForumRepository(context, _mapper);

            
            var result = await repository.GetAllByMushroomIdPaginated<ForumPostDTO>(1, 1, 10);

            
            Assert.Equal(2, result.TotalCount);
            Assert.Equal(2, result.Items.Count());
            Assert.All(result.Items, item => Assert.Equal(1, item.MushroomId));
        }

        [Fact]
        public async Task GetAllByUserIdPaginated_ReturnsCorrectPosts()
        {
            
            using var context = new ApplicationDbContext(_options);
            var repository = new ForumRepository(context, _mapper);

            
            var result = await repository.GetAllByUserIdPaginated<ForumPostDTO>(1, 1, 10);

            
            Assert.Equal(2, result.TotalCount);
            Assert.Equal(2, result.Items.Count());
            Assert.All(result.Items, item => Assert.Equal(1, item.UserId));
        }

        [Fact]
        public async Task GetById_WithValidId_ReturnsPost()
        {
            
            using var context = new ApplicationDbContext(_options);
            var repository = new ForumRepository(context, _mapper);

            
            var result = await repository.GetById(1);

            
            Assert.NotNull(result);
            Assert.Equal("This is a post about Field Mushroom", result.Content);
            Assert.Equal(1, result.MushroomId);
        }

        [Fact]
        public async Task GetById_WithInvalidId_ReturnsNull()
        {
            
            using var context = new ApplicationDbContext(_options);
            var repository = new ForumRepository(context, _mapper);

            
            var result = await repository.GetById(999);

            
            Assert.Null(result);
        }

        [Fact]
        public async Task Add_AddsForumPost()
        {
            
            using var context = new ApplicationDbContext(_options);
            var repository = new ForumRepository(context, _mapper);
            var newPost = new ForumPost
            {
                Content = "New forum post",
                MushroomId = 1,
                UserId = 1,
                CreatedAt = DateTime.UtcNow
            };

            
            repository.Add(newPost);
            await repository.SafeChangesAsync();

            
            var addedPost = await context.ForumPosts.FirstOrDefaultAsync(p => p.Content == "New forum post");
            Assert.NotNull(addedPost);
            Assert.Equal(1, addedPost.MushroomId);
        }

        [Fact]
        public async Task Update_UpdatesForumPost()
        {
            
            using var context = new ApplicationDbContext(_options);
            var repository = new ForumRepository(context, _mapper);
            var post = await repository.GetById(1);
            Assert.NotNull(post);

            
            post.Content = "Updated content";
            post.UpdatedAt = DateTime.UtcNow;
            repository.Update(post);
            await repository.SafeChangesAsync();

            
            var updatedPost = await context.ForumPosts.FindAsync(1);
            Assert.NotNull(updatedPost);
            Assert.Equal("Updated content", updatedPost.Content);
            Assert.NotNull(updatedPost.UpdatedAt);
        }

        [Fact]
        public async Task Delete_RemovesForumPost()
        {
            
            using var context = new ApplicationDbContext(_options);
            var repository = new ForumRepository(context, _mapper);
            var post = await repository.GetById(1);
            Assert.NotNull(post);

            
            repository.Delete(post);
            await repository.SafeChangesAsync();

            
            var deletedPost = await context.ForumPosts.FindAsync(1);
            Assert.Null(deletedPost);
        }
    }
}