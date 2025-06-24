using System.ComponentModel.DataAnnotations;

namespace PhishFood.ViewModels
{
    public class TrainingListViewModel
    {
        public int ID { get; set; }
        [Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;
        [Display(Name = "Category")]
        public string CategoryType { get; set; } = string.Empty;
        [Display(Name = "Subcategory")]
        public string SubcategoryType { get; set; } = string.Empty;
        [Display(Name = "Include in Training:")]
        public bool IsActive { get; set; }
    }
}
