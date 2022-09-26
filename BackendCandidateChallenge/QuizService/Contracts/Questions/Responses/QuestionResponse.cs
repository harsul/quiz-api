using System;
using System.Collections.Generic;
using QuizService.Contracts.Answers.Responses;

namespace QuizService.Contracts.Questions.Responses;

public class QuestionResponse
{
    public int Id { get; set; }
    public string Text { get; set; }
    public IEnumerable<AnswerResponse> Answers { get; set; }
    public int CorrectAnswerId { get; set; }
}