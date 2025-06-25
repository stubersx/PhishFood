using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhishFood.Models
{
    public class Student    // automatically created from registration information?
    {
        [Required]
        [Display(Name = "NMC ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string ID { get; set; } = string.Empty;

        [Required]
        [Display(Name = "First Name")]
        [StringLength(20, ErrorMessage = "First name cannot exceed 20 characters.")]
        [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "Only letters are allowed.")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Last Name")]
        [StringLength(20, ErrorMessage = "Last name cannot exceed 20 characters.")]
        [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "Only letters are allowed.")]
        public string LastName { get; set; } = string.Empty;

        public ICollection<Result> Results { get; set; } = new List<Result>();
    }
}
