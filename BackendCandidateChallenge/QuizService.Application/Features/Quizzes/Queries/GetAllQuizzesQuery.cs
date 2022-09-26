using System;
using MediatR;
using QuizService.Application.Models;
using QuizService.Domain.Models;

namespace QuizService.Application.Features.Quizzes.Queries;

public class GetAllQuizzesQuery : IRequest<OperationResult<List<Quiz>>>
{
}