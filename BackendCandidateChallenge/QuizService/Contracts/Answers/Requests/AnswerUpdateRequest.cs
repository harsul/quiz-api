using System.ComponentModel.DataAnnotations;

namespace QuizService.Contracts.Answers.Request;

public class AnswerUpdateRequest
{
    [Required]
    public string Text { get; set; }
}