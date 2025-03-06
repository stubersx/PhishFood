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
        public int? CategoryID { get; set; }
        public Category? Category { get; set; }

        public int? SubCategoryID { get; set; }
        public SubCategory? SubCategory { get; set; }
    }
}
