using System;
using MediatR;
using QuizService.Application.Models;
using QuizService.Domain.Models;

namespace QuizService.Application.Features.Answers.Queries;

public class GetAnswersByQuizIdQuery : IRequest<OperationResult<Dictionary<int,IList<Answer>>>>
{
    public int QuizId { get; set; }
}