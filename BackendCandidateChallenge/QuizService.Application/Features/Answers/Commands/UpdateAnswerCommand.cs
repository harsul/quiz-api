using System;
using MediatR;
using QuizService.Application.Models;
using QuizService.Domain.Models;

namespace QuizService.Application.Features.Answers.Commands;

public class UpdateAnswerCommand : IRequest<OperationResult<Answer>>
{
    public int AnswerId { get; set; }
    public string Text { get; set; }
}
