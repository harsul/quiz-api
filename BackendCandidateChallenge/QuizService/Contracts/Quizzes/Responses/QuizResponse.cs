using System.Collections.Generic;
using QuizService.Contracts.Questions.Responses;

namespace QuizService.Contracts.Quizzes.Responses;

public class QuizResponse
{
    public long Id { get; set; }
    public string Title { get; set; }
    public IEnumerable<QuestionResponse> Questions { get; set; }
    public IDictionary<string, string> Links { get; set; }
}