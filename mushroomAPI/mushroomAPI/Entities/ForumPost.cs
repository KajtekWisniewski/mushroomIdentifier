namespace mushroomAPI.Entities
{
    public class ForumPost
    {
        public int Id { get; set; }
        public string Content { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public int MushroomId { get; set; }
        public Mushroom Mushroom { get; set; } = null!;

        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
