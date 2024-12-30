using in2Task.Models;
using Microsoft.EntityFrameworkCore;

namespace in2Task.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<Comment> Comments { get; set; }

        // Dodaj konstruktor
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<User>()
                .HasMany(u => u.BlogPosts)
                .WithOne(b => b.User)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Comments)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .Property(u => u.Username)
                .HasMaxLength(255);

            modelBuilder.Entity<User>()
                .Property(u => u.Password)
                .HasMaxLength(255);

            modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .HasMaxLength(255);

            // BlogPost entity
            modelBuilder.Entity<BlogPost>()
                .HasMany(b => b.Comments)
                .WithOne(c => c.BlogPost)
                .HasForeignKey(c => c.BlogPostId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BlogPost>()
                .Property(b => b.Title)
                .HasMaxLength(255);

            
           // modelBuilder.Entity<User>().HasData(
           //    new User { Id = 1, Username = "user1", Password = "password1", Email = "user1@example.com" },
           //    new User { Id = 2, Username = "user2", Password = "password2", Email = "user2@example.com" }
           //);

           // // Seed BlogPosts
           // modelBuilder.Entity<BlogPost>().HasData(
           //     new BlogPost { Id = 1, Title = "First Post", Content = "This is the first blog post.", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, UserId = 1 },
           //     new BlogPost { Id = 2, Title = "Second Post", Content = "This is the second blog post.", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, UserId = 2 }
           // );

           // //// Seed Comments
           // modelBuilder.Entity<Comment>().HasData(
           //     new Comment { Id = 1, Content = "Great post!", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, UserId = 2, BlogPostId = 1 },
           //     new Comment { Id = 2, Content = "Very informative.", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, UserId = 1, BlogPostId = 2 }
           // );


        }
    }
}
