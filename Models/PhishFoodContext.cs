using Microsoft.EntityFrameworkCore;

namespace PhishFood.Models
{
    public class PhishFoodContext : DbContext
    {
        public PhishFoodContext(DbContextOptions<PhishFoodContext> options) : base(options) { }
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<SubCategory> SubCategories { get; set; } = null!;
        public DbSet<Testing> Testings { get; set; } = null!;
        public DbSet<Training> Trainings { get; set; } = null!;
        public DbSet<Result> Results { get; set; } = null!;
        public DbSet<Student> Students { get; set; } = null!;
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { ID = 1, Type = "Phishing" }
                );
            modelBuilder.Entity<SubCategory>().HasData(
                new SubCategory { ID = 1, Type = "Email Phishing" },
                new SubCategory { ID = 2, Type = "SMS Phishing" },
                new SubCategory { ID = 3, Type = "Voice Phishing" },
                new SubCategory { ID = 4, Type = "Spear Phishing" },
                new SubCategory { ID = 5, Type = "Whale Phishing" }
                );
        }
    }
}
