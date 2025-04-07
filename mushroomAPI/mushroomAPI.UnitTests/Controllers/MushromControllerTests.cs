using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using mushroomAPI.Controllers;
using mushroomAPI.DTOs;
using mushroomAPI.DTOs.Mushroom.Entries;
using mushroomAPI.Entities;
using mushroomAPI.Repository.Contracts;
using System.Security.Claims;
using Xunit;

namespace mushroomAPI.UnitTests.Controllers
{
    public class MushroomControllerTests
    {
        private readonly Mock<IMushroomRepository> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly MushroomController _controller;

        public MushroomControllerTests()
        {
            _mockRepository = new Mock<IMushroomRepository>();
            _mockMapper = new Mock<IMapper>();
            _controller = new MushroomController(_mockRepository.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetAllMushrooms_ReturnsOkWithPagedList()
        {
            
            var pagedList = new PagedList<MushroomDTO>
            {
                Items = new List<MushroomDTO>
                {
                    new MushroomDTO { Id = 1, Name = "Mushroom 1" },
                    new MushroomDTO { Id = 2, Name = "Mushroom 2" }
                },
                CurrentPage = 1,
                PageSize = 10,
                TotalCount = 2,
                TotalPages = 1
            };

            _mockRepository.Setup(repo => repo.GetPaginated<MushroomDTO>(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(pagedList);

            
            var result = await _controller.GetAllMushrooms();

            
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<PagedList<MushroomDTO>>(okResult.Value);
            Assert.Equal(2, returnValue.Items.Count());
        }

        [Fact]
        public async Task GetMushroomById_WithValidId_ReturnsOkWithMushroom()
        {
            
            var mushroom = new MushroomDTO { Id = 1, Name = "Mushroom 1" };
            _mockRepository.Setup(repo => repo.GetById<MushroomDTO>(1))
                .ReturnsAsync(mushroom);

            
            var result = await _controller.GetMushroomById(1);

            
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<MushroomDTO>(okResult.Value);
            Assert.Equal("Mushroom 1", returnValue.Name);
        }

        [Fact]
        public async Task GetMushroomById_WithInvalidId_ReturnsNotFound()
        {
            
            _mockRepository.Setup(repo => repo.GetById<MushroomDTO>(It.IsAny<int>()))
                .ReturnsAsync((MushroomDTO?)null);

            
            var result = await _controller.GetMushroomById(999);

            
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetMushroomsByCategory_ReturnsOkWithPagedList()
        {
            
            var pagedList = new PagedList<MushroomDTO>
            {
                Items = new List<MushroomDTO>
                {
                    new MushroomDTO { Id = 1, Name = "Mushroom 1", Category = MushroomCategory.Agaricus }
                },
                CurrentPage = 1,
                PageSize = 10,
                TotalCount = 1,
                TotalPages = 1
            };

            _mockRepository.Setup(repo => repo.GetPaginatedByCategory<MushroomDTO>(
                    It.IsAny<MushroomCategory>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(pagedList);

            
            var result = await _controller.GetMushroomsByCategory(MushroomCategory.Agaricus);

            
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<PagedList<MushroomDTO>>(okResult.Value);
            Assert.Single(returnValue.Items);
            Assert.Equal(MushroomCategory.Agaricus, returnValue.Items.First().Category);
        }

       [Fact]
        public void GetMockPredictions_ReturnsOkWithPredictions()
        {          
            var result = _controller.GetMockPredictions();
         
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            
            var resultType = okResult.Value.GetType();
            var predictionsProperty = resultType.GetProperty("predictions");
            Assert.NotNull(predictionsProperty);
        }

        [Fact]
        public async Task CreateMushroom_WithValidData_ReturnsCreatedMushroom()
        {
            
            var mushroomDto = new UpsertMushroomDTO { Name = "New Mushroom" };
            var mushroom = new Mushroom { Id = 1, Name = "New Mushroom" };

            _mockMapper.Setup(m => m.Map<Mushroom>(mushroomDto)).Returns(mushroom);
            _mockRepository.Setup(repo => repo.SafeChangesAsync()).ReturnsAsync(true);

            
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, "Admin"),
            }, "mock"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            
            var result = await _controller.CreateMushroom(mushroomDto);

            
            var createdAtResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(_controller.GetMushroomById), createdAtResult.ActionName);
            Assert.Equal(mushroom, createdAtResult.Value);
        }
    }
}