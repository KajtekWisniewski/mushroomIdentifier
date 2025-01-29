namespace mushroomAPI.DTOs.Forum
{
    public class ForumPostDTO
    {
        public int Id { get; init; }
        public string Content { get; init; } = "";
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
        public int MushroomId { get; init; }
        public string MushroomName { get; init; } = "";
        public int UserId { get; init; }
        public string Username { get; init; } = "";
    }
}
