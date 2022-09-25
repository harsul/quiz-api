using System;
using MediatR;
using QuizService.Application.Models;
using QuizService.Domain.Models;

namespace QuizService.Application.Features.Quizzes.Queries
{
    public class GetQuizByIdQuery : IRequest<OperationResult<Quiz>>
    {
        public int Id { get; set; }
    }
}