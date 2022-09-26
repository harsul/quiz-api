using System;
using MediatR;
using QuizService.Application.Models;

namespace QuizService.Application.Features.Answers.Commands
{
    public class CreateAnswerCommand : IRequest<OperationResult<int>>
    {
        public int QuestionId { get; set; }
        public string Text { get; set; }
    }
}
