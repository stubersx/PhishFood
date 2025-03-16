using System.ComponentModel.DataAnnotations;

namespace PhishFood.Models
{
    public class Training
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "The Content is required.")]
        public string Content { get; set; } = string.Empty;

        [Required(ErrorMessage = "The Category is required.")]
        [Display(Name = "Category")]
        public int? CategoryID { get; set; }
        public Category? Category { get; set; }

        [Display(Name = "Subcategory")]
        public int? SubcategoryID { get; set; }
        public Subcategory? Subcategory { get; set; }
    }
}
