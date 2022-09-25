namespace QuizService.Contracts.Questions.Request;

public class QuestionUpdateRequest
{
    public string Text { get; set; }
    public int CorrectAnswerId { get; set; }
}