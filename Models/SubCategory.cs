using System.ComponentModel.DataAnnotations;

namespace PhishFood.Models
{
    public class SubCategory
    {
        public int ID { get; set; }
        [Required]
        public string Type { get; set; } = string.Empty;

        public int CategoryID { get; set; }
        public Category? Category { get; set; }
        public ICollection<Testing> Testings { get; set; } = new List<Testing>();
        public ICollection<Training> Trainings { get; set; } = new List<Training>();
    }
}
