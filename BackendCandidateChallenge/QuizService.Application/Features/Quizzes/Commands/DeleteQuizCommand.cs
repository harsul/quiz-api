using System;
using MediatR;
using QuizService.Application.Models;
using QuizService.Domain.Models;

namespace QuizService.Application.Features.Quizzes.Commands
{
    public class DeleteQuizCommand : IRequest<OperationResult<Quiz>>
    {
        public int Id { get; set; }
    }
}
