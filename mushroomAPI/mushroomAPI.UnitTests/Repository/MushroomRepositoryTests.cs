using AutoMapper;
using Microsoft.EntityFrameworkCore;
using mushroomAPI.Configuration;
using mushroomAPI.Data;
using mushroomAPI.DTOs.Mushroom.Entries;
using mushroomAPI.Entities;
using mushroomAPI.Repository;
using Xunit;
using Moq;
using mushroomAPI.Repository.Contracts;
using mushroomAPI.DTOs;

namespace mushroomAPI.UnitTests.Repository
{
    public class MushroomRepositoryTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;
        private readonly IMapper _mapper;

        public MushroomRepositoryTests()
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
            context.Mushrooms.AddRange(
                new Mushroom
                {
                    Id = 1,
                    Name = "Field Mushroom",
                    ScientificName = "Agaricus campestris",
                    Category = MushroomCategory.Agaricus,
                    Description = "Common edible mushroom found in grasslands",
                    IsEdible = true,
                    Habitat = "Meadows and fields",
                    Season = "Summer-Fall",
                    CommonNames = new List<string> { "Meadow Mushroom", "Pink Bottom" },
                    ImageUrls = new List<string> { "image1.jpg", "image2.jpg" }
                },
                new Mushroom
                {
                    Id = 2,
                    Name = "Death Cap",
                    ScientificName = "Amanita phalloides",
                    Category = MushroomCategory.Amanita,
                    Description = "One of the most poisonous mushrooms known",
                    IsEdible = false,
                    Habitat = "Woodland",
                    Season = "Summer-Fall",
                    CommonNames = new List<string> { "Death Cup", "Green Death Cap" },
                    ImageUrls = new List<string> { "image3.jpg", "image4.jpg" }
                },
                new Mushroom
                {
                    Id = 3,
                    Name = "Penny Bun",
                    ScientificName = "Boletus edulis",
                    Category = MushroomCategory.Boletus,
                    Description = "Prized edible mushroom with thick stem",
                    IsEdible = true,
                    Habitat = "Mixed woodland",
                    Season = "Late Summer-Fall",
                    CommonNames = new List<string> { "Porcini", "King Bolete" },
                    ImageUrls = new List<string> { "image5.jpg", "image6.jpg" }
                }
            );

            context.SaveChanges();
        }

        [Fact]
        public async Task GetPaginated_ReturnsCorrectPage()
        {
            
            using var context = new ApplicationDbContext(_options);
            var repository = new MushroomRepository(context, _mapper);

            var result = await repository.GetPaginated<MushroomDTO>(1, 2);
            
            Assert.Equal(1, result.CurrentPage);
            Assert.Equal(2, result.PageSize);
            Assert.Equal(3, result.TotalCount);
            Assert.Equal(2, result.Items.Count());
            Assert.Equal("Field Mushroom", result.Items.First().Name);
        }

        [Fact]
        public async Task GetPaginatedByCategory_ReturnsCorrectMushrooms()
        {
            
            using var context = new ApplicationDbContext(_options);
            var repository = new MushroomRepository(context, _mapper);
            
            var result = await repository.GetPaginatedByCategory<MushroomDTO>(MushroomCategory.Amanita, 1, 10);
            
            Assert.Single(result.Items);
            Assert.Equal("Death Cap", result.Items.First().Name);
            Assert.Equal(MushroomCategory.Amanita, result.Items.First().Category);
        }

        [Fact]
        public async Task GetById_WithValidId_ReturnsMushroom()
        {
            
            using var context = new ApplicationDbContext(_options);
            var repository = new MushroomRepository(context, _mapper);

            
            var result = await repository.GetById(1);

            
            Assert.NotNull(result);
            Assert.Equal("Field Mushroom", result.Name);
            Assert.Equal(MushroomCategory.Agaricus, result.Category);
        }

        [Fact]
        public async Task GetById_WithInvalidId_ReturnsNull()
        {
            
            using var context = new ApplicationDbContext(_options);
            var repository = new MushroomRepository(context, _mapper);

            
            var result = await repository.GetById(999);

            
            Assert.Null(result);
        }

        [Fact]
        public async Task Add_AddsMushroom()
        {
            
            using var context = new ApplicationDbContext(_options);
            var repository = new MushroomRepository(context, _mapper);
            var newMushroom = new Mushroom
            {
                Name = "Chanterelle",
                ScientificName = "Cantharellus cibarius",
                Category = MushroomCategory.Agaricus,
                Description = "Golden funnel-shaped mushroom",
                IsEdible = true,
                Habitat = "Forest",
                Season = "Summer-Fall",
                CommonNames = new List<string> { "Girolle" },
                ImageUrls = new List<string> { "chanterelle.jpg" }
            };

            
            repository.Add(newMushroom);
            await repository.SafeChangesAsync();

            
            var addedMushroom = await context.Mushrooms.FirstOrDefaultAsync(m => m.Name == "Chanterelle");
            Assert.NotNull(addedMushroom);
            Assert.Equal("Cantharellus cibarius", addedMushroom.ScientificName);
        }

        [Fact]
        public async Task Update_UpdatesMushroom()
        {
            
            using var context = new ApplicationDbContext(_options);
            var repository = new MushroomRepository(context, _mapper);
            var mushroom = await repository.GetById(1);
            Assert.NotNull(mushroom);

            
            mushroom.Description = "Updated description";
            repository.Update(mushroom);
            await repository.SafeChangesAsync();

            
            var updatedMushroom = await context.Mushrooms.FindAsync(1);
            Assert.NotNull(updatedMushroom);
            Assert.Equal("Updated description", updatedMushroom.Description);
        }

        [Fact]
        public async Task Delete_RemovesMushroom()
        {
            
            using var context = new ApplicationDbContext(_options);
            var repository = new MushroomRepository(context, _mapper);
            var mushroom = await repository.GetById(1);
            Assert.NotNull(mushroom);

            
            repository.Delete(mushroom);
            await repository.SafeChangesAsync();

            
            var deletedMushroom = await context.Mushrooms.FindAsync(1);
            Assert.Null(deletedMushroom);
        }

        [Fact]
        public async Task SearchMushrooms_ByName_ReturnsCorrectMushrooms()
        {
            using var context = new ApplicationDbContext(_options);
            
            var mockRepository = new Mock<IMushroomRepository>();
            var mushrooms = new List<Mushroom>
            {
                new Mushroom { Id = 1, Name = "Field Mushroom" }
            };
            var pagedList = new PagedList<MushroomDTO>
            {
                Items = new List<MushroomDTO> { new MushroomDTO { Id = 1, Name = "Field Mushroom" } },
                CurrentPage = 1,
                PageSize = 10,
                TotalCount = 1,
                TotalPages = 1
            };
            
            mockRepository.Setup(repo => repo.SearchMushrooms<MushroomDTO>(
                It.Is<string>(s => s == "Field"), 
                It.IsAny<MushroomCategory?>(), 
                It.IsAny<string>(), 
                It.IsAny<bool?>(), 
                It.IsAny<int>(), 
                It.IsAny<int>()))
                .ReturnsAsync(pagedList);
            
            
            var result = await mockRepository.Object.SearchMushrooms<MushroomDTO>("Field", null, null, null);

            
            Assert.Single(result.Items);
            Assert.Equal("Field Mushroom", result.Items.First().Name);
        }

        [Fact]
        public async Task SearchMushrooms_ByEdibility_ReturnsCorrectMushrooms()
        {
            
            using var context = new ApplicationDbContext(_options);
            var repository = new MushroomRepository(context, _mapper);

            
            var result = await repository.SearchMushrooms<MushroomDTO>(null, null, null, true);

            
            Assert.Equal(2, result.Items.Count());
            Assert.All(result.Items, item => Assert.True(item.IsEdible));
        }
    }
}