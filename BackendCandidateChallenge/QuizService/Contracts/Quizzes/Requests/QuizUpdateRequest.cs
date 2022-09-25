using System.ComponentModel.DataAnnotations;

namespace QuizService.Model;

public class QuizUpdateRequest
{
    [Required]
    public string Title { get; set; }
}