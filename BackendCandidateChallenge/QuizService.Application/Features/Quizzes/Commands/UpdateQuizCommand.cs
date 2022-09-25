using System;
using MediatR;
using QuizService.Application.Models;
using QuizService.Domain.Models;

namespace QuizService.Application.Features.Quizzes.Commands
{
    public class UpdateQuizCommand : IRequest<OperationResult<Quiz>>
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }
}

