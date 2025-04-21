public class TestSessionViewModel
{
    public List<QuestionDTO> Questions { get; set; } = new();
    public int CurrentIndex { get; set; }
    public int Score { get; set; }
    public bool ShowExplanation { get; set; }
    public bool IsFinished { get; set; }
    public string? SelectedAnswer { get; set; }
    public bool IsCorrect { get; set; }
    public int CategoryID { get; set; }
    public int? SubcategoryID { get; set; }
}
