using Microsoft.EntityFrameworkCore;

namespace PhishFood.Models
{
    public class PhishFoodContext : DbContext
    {
        public PhishFoodContext(DbContextOptions<PhishFoodContext> options) : base(options) { }
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Subcategory> Subcategories { get; set; } = null!;
        public DbSet<Testing> Testings { get; set; } = null!;
        public DbSet<Training> Trainings { get; set; } = null!;
        public DbSet<Result> Results { get; set; } = null!;
        public DbSet<Student> Students { get; set; } = null!;
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { ID = 1, Type = "Phishing" }
                );
            modelBuilder.Entity<Subcategory>().HasData(
                new Subcategory { ID = 1, Type = "Email Phishing", CategoryID = 1 },
                new Subcategory { ID = 2, Type = "SMS Phishing", CategoryID = 1 },
                new Subcategory { ID = 3, Type = "Voice Phishing", CategoryID = 1 },
                new Subcategory { ID = 4, Type = "Spear Phishing", CategoryID = 1 },
                new Subcategory { ID = 5, Type = "Whale Phishing", CategoryID = 1 }
                );
        }
    }
}
