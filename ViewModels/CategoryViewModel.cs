using PhishFood.Models;

namespace PhishFood.ViewModels
{
    public class CategoryViewModel
    {
        public List<Category> Categories { get; set; } = new List<Category>();
        public List<Subcategory> Subcategories { get; set; } = new List<Subcategory>();
    }
}
