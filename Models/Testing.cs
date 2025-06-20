using System.ComponentModel.DataAnnotations;

namespace PhishFood.Models
{
    public class Testing    // Question
    {
        public int ID { get; set; }
        [Required]
        public string Question { get; set; } = string.Empty;
        [Required]
        public string Key { get; set; } = string.Empty;
        [Required]
        public string Option1 { get; set; } = string.Empty;
        [Required]
        public string Option2 { get; set; } = string.Empty;
        [Required]
        public string Option3 { get; set; } = string.Empty;
        [Required]
        public string Explanation { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Category")]
        public int? CategoryID { get; set; }
        public Category? Category { get; set; }

        [Display(Name = "Subcategory")]
        public int? SubcategoryID { get; set; }
        public Subcategory? Subcategory { get; set; }
        [Display(Name = "Include in Testing:")]
        public bool IsActive { get; set; } = true;
    }
}
