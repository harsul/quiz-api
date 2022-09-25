using System;
using MediatR;
using QuizService.Application.Models;
using QuizService.Domain.Models;

namespace QuizService.Application.Features.Quizzes.Commands
{
    public class CreateQuizCommand : IRequest<OperationResult<Quiz>>
    {
        public string Title { get; set; }
    }
}