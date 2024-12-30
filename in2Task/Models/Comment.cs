namespace in2Task.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int UserId { get; set; }
        public int BlogPostId { get; set; }

        // Navigation properties
        public User User { get; set; }
        public BlogPost BlogPost { get; set; }
    }
}
