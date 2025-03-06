using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhishFood.Models
{
    public class Student
    {
        [Required]
        [Display(Name = "NMC ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string ID { get; set; } = string.Empty;

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        public ICollection<Result> Results { get; set; } = new List<Result>();
    }
}
