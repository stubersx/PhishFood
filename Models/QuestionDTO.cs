public class QuestionDTO
{
    public int ID { get; set; }
    public string Question { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string Option1 { get; set; } = string.Empty;
    public string Option2 { get; set; } = string.Empty;
    public string Option3 { get; set; } = string.Empty;
    public string Explanation { get; set; } = string.Empty;
    public List<string> ShuffledOptions { get; set; } = new();
    public int CategoryID { get; set; }
}
