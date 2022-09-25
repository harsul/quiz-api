using System.ComponentModel.DataAnnotations;

namespace QuizService.Contracts.Questions.Request;

public class QuestionUpdateRequest
{
    [Required]
    public string Text { get; set; }
    public int CorrectAnswerId { get; set; }
}