using System.ComponentModel.DataAnnotations;

namespace PhishFood.Models
{
    public class Training
    {
        public int ID { get; set; }
        [Required]
        public string Content { get; set; } = string.Empty;

        [Required]
        public int? CategoryID { get; set; }
        public Category? Category { get; set; }

        public int? SubCategoryID { get; set; }
        public SubCategory? SubCategory { get; set; }
    }
}
