using System.ComponentModel.DataAnnotations;

namespace QuizService.Contracts.Questions.Request;

public class QuestionCreateRequest
{
    [Required]
    public string Text { get; set; }
}