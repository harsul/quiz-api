using System;
using MediatR;
using QuizService.Application.Models;
using QuizService.Domain.Models;

namespace QuizService.Application.Features.Questions.Commands
{
    public class UpdateQuestionCommand : IRequest<OperationResult<Question>>
    {
        public int QuestionId { get; set; }
        public string Text { get; set; }
        public int CorrectAnswerId { get; set; }
    }
}