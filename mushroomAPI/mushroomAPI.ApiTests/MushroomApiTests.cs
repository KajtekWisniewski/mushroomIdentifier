using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using mushroomAPI.DTOs.Auth;
using mushroomAPI.DTOs.Mushroom.Coordinates;
using mushroomAPI.DTOs.Mushroom.Entries;
using mushroomAPI.Entities;
using Xunit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace mushroomAPI.ApiTests
{
    public class MushroomApiTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _jsonOptions;

        public MushroomApiTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        private async Task<string> GetAuthToken()
        {
            var configuration = _factory.Services.GetRequiredService<IConfiguration>();
            var fixedToken = configuration["TestSettings:FixedAuthToken"];

            return fixedToken;
        }

        [Fact]
        public async Task GetAllMushrooms_ReturnsSuccessAndMushrooms()
        {
            
            var response = await _client.GetAsync("/api/mushrooms");

            
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<dynamic>(responseString, _jsonOptions);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetMushroomById_WithValidId_ReturnsSuccess()
        {
            
            var response = await _client.GetAsync("/api/mushrooms/1");

            
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var mushroom = JsonSerializer.Deserialize<MushroomDTO>(content, _jsonOptions);

            Assert.NotNull(mushroom);
            Assert.Equal("Field Mushroom", mushroom.Name);
            Assert.Equal(MushroomCategory.Agaricus, mushroom.Category);
        }

        [Fact]
        public async Task GetMushroomById_WithInvalidId_ReturnsNotFound()
        {
            
            var response = await _client.GetAsync("/api/mushrooms/999");

            
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetMushroomsByCategory_ReturnsSuccessAndFilteredMushrooms()
        {
            
            var response = await _client.GetAsync("/api/mushrooms/category/Agaricus");

            
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<dynamic>(content, _jsonOptions);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetMockPredictions_ReturnsSuccessAndPredictions()
        {
            
            var response = await _client.GetAsync("/api/mushrooms/mock");

            
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonDocument.Parse(content);

            Assert.True(result.RootElement.TryGetProperty("predictions", out _));
        }
    }
}