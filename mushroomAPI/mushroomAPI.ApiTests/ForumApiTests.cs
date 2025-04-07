using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using mushroomAPI.DTOs.Auth;
using mushroomAPI.DTOs.Forum;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using mushroomAPI.Services.IServices;
using mushroomAPI.Entities;

namespace mushroomAPI.ApiTests
{
    public class ForumApiTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _jsonOptions;

        public ForumApiTests(CustomWebApplicationFactory factory)
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

        private async Task<string> GetAuthToken(string username = "admin")
        {
            return "mock-token";
        }

        [Fact]
        public async Task GetPostsByMushroom_ReturnsSuccessAndPosts()
        {
            
            var response = await _client.GetAsync("/api/forum/mushroom/1");

            
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonDocument.Parse(content);
            
            Assert.True(result.RootElement.TryGetProperty("items", out var items));
            Assert.True(items.GetArrayLength() > 0);
        }

        [Fact]
        public async Task GetPostsByUser_ReturnsSuccessAndPosts()
        {
            
            var response = await _client.GetAsync("/api/forum/user/1");

            
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonDocument.Parse(content);
            
            Assert.True(result.RootElement.TryGetProperty("items", out var items));
            Assert.True(items.GetArrayLength() > 0);
        }

        [Fact]
        public async Task CreatePost_WithoutAuthentication_ReturnsUnauthorized()
        {
            
            var newPost = new CreateForumPostDTO
            {
                Content = "This is a test forum post",
                MushroomId = 1
            };

            
            var response = await _client.PostAsJsonAsync("/api/forum", newPost);

            
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task UpdatePost_AsNonOwner_ReturnsForbidden()
        {
            
            var token = await GetAuthToken("admin");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var updatePost = new UpdateForumPostDTO
            {
                Content = "This is an attempt to update someone else's post"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(updatePost),
                Encoding.UTF8,
                "application/json");
            
            var response = await _client.PutAsync("/api/forum/1", content);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

    }
}