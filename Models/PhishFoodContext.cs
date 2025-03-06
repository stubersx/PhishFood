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
        /*
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
        */
    }
}
