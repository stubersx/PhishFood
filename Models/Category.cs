using System.ComponentModel.DataAnnotations;

namespace PhishFood.Models
{
    public class Category
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "The Category Name is required.")]
        [StringLength(30, ErrorMessage = "Category name cannot exceed 30 characters.")]
        [RegularExpression(@"^[a-zA-Z\s\p{P}]+$", ErrorMessage = "Only letters, spaces, and special characters are allowed.")]
        [Display(Name = "Category Name")]
        public string Type { get; set; } = string.Empty;
        public ICollection<Subcategory> Subcategories { get; set; } = new List<Subcategory>();
        public ICollection<Testing> Testings { get; set; } = new List<Testing>();
        public ICollection<Training> Trainings { get; set;} = new List<Training>();
        public ICollection<Result> Results { get; set; } = new List<Result>();
    }
}
