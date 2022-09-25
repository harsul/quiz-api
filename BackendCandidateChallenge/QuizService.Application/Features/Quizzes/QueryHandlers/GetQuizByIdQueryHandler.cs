using System;
using System.Data;
using System.Net;
using Dapper;
using MediatR;
using QuizService.Application.Features.Quizzes.Queries;
using QuizService.Application.Models;
using QuizService.Domain.Models;

namespace QuizService.Application.Features.Quizzes.QueryHandlers
{
    public class GetQuizByIdQueryHandler : IRequestHandler<GetQuizByIdQuery, OperationResult<Quiz>>
    {
        private readonly IDbConnection _connection;

        public GetQuizByIdQueryHandler(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<OperationResult<Quiz>> Handle(GetQuizByIdQuery request, CancellationToken cancellationToken)
        {
            var result = new OperationResult<Quiz>();

            const string quizSql = "SELECT * FROM Quiz WHERE Id = @Id;";

            try
            {
                var quiz = await _connection.QuerySingleAsync<Quiz>(quizSql, new { Id = request.Id });
                result.AddValue(quiz);
            }
            catch (InvalidOperationException ex)
            {
                result.AddError(
                    HttpStatusCode.NotFound,
                    $"Quiz with {request.Id} doesn't exists! {ex.Message}");
            }

            return result;
        }
    }
}