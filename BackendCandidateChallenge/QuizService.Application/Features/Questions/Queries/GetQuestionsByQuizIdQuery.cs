using System;
using MediatR;
using QuizService.Application.Models;
using QuizService.Domain.Models;

namespace QuizService.Application.Features.Questions.Queries
{
    public class GetQuestionsByQuizIdQuery : IRequest<OperationResult<List<Question>>>
    {
        public int QuizId { get; set; }
    }
}
