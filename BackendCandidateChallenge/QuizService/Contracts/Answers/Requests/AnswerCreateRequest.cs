using System.ComponentModel.DataAnnotations;

namespace QuizService.Contracts.Answers.Request;

public class AnswerCreateRequest
{
    [Required]
    public string Text { get; set; }
}