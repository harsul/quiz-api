using MediatR;
using QuizService.Application.Models;

namespace QuizService.Application.Features.Questions.Commands
{
    public class CreateQuestionCommand : IRequest<OperationResult<int>>
    {
        public int QuizId { get; set; }
        public string Text { get; set; }
    }
}