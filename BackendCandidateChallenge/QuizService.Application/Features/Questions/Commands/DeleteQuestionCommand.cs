using System;
using MediatR;
using QuizService.Application.Models;
using QuizService.Domain.Models;

namespace QuizService.Application.Features.Questions.Commands;

public class DeleteQuestionCommand : IRequest<OperationResult<Question>>
{
    public int QuestionId { get; set; }
}

