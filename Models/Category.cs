using System.ComponentModel.DataAnnotations;

namespace PhishFood.Models
{
    public class Category
    {
        public int ID { get; set; }
        [Required]
        public string Type { get; set; } = string.Empty;

        public ICollection<SubCategory> SubCategories { get; set; } = new List<SubCategory>();
        public ICollection<Testing> Testings { get; set; } = new List<Testing>();
        public ICollection<Training> Trainings { get; set;} = new List<Training>();
        public ICollection<Result> Results { get; set; } = new List<Result>();
    }
}
