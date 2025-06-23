namespace PhishFood.ViewModels
{
    public class TrainingListViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CategoryType { get; set; } = string.Empty;
        public string SubcategoryType { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
