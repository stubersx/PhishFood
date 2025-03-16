using System.ComponentModel.DataAnnotations;

namespace PhishFood.Models
{
    public class Result     // automatically recorded?
    {
        public int ID { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }
        [Required]
        [Range(1, 10)]
        public int Score { get; set; }

        [Required]
        [Display(Name = "Category")]
        public int CategoryID { get; set; }
        public Category? Category { get; set; }

        [Required]
        [Display(Name = "NMC ID")]
        public string StudentID { get; set; } = string.Empty;
        public Student? Student { get; set; }
    }
}
