using System.ComponentModel.DataAnnotations;

namespace in2Task.Models
{
    public class User
    {
        public int Id { get; set; }
       
        public string Username { get; set; }
        
        public string Password { get; set; }
       
        public string Email { get; set; }

        // Navigation properties
        public ICollection<BlogPost> BlogPosts { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }
}
