using System.ComponentModel.DataAnnotations;

namespace PhishFood.Models
{
    public class Training
    {
        public int ID { get; set; }
        [Display(Name = "Content Name")]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "The Content is required.")]
        public string Content { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;

        [Required(ErrorMessage = "The Category is required.")]
        [Display(Name = "Category")]
        public int? CategoryID { get; set; }
        public Category? Category { get; set; }

        [Display(Name = "Subcategory")]
        public int? SubcategoryID { get; set; }
        public Subcategory? Subcategory { get; set; }
        [Display(Name = "Include in Training:")]
        public bool IsActive { get; set; } = true;
    }
}
