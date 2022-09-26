using System;
using System.Data;
using Dapper;
using MediatR;
using QuizService.Application.Features.Questions.Queries;
using QuizService.Application.Models;
using QuizService.Domain.Models;

namespace QuizService.Application.Features.Questions.QueryHandlers
{
    public class GetQuestionsByQuizIdQueryHandler : IRequestHandler<GetQuestionsByQuizIdQuery, OperationResult<List<Question>>>
    {
        private readonly IDbConnection _connection;

        public GetQuestionsByQuizIdQueryHandler(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<OperationResult<List<Question>>> Handle(GetQuestionsByQuizIdQuery request, CancellationToken cancellationToken)
        {
            var result = new OperationResult<List<Question>>();
            const string sqlCommand = "SELECT * FROM Question WHERE QuizId = @QuizId;";

            var questions = await _connection.QueryAsync<Question>(sqlCommand, new { request.QuizId });

            result.AddValue(questions.ToList());

            return result;
        }
    }
}