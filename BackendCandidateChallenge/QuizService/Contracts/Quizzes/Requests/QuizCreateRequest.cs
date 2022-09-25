using System.ComponentModel.DataAnnotations;

namespace QuizService.Model;

public class QuizCreateRequest
{
    [Required]
    public string Title { get; set; }
}