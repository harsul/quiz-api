using System;
using System.Data;
using Dapper;
using System.Net;
using MediatR;
using QuizService.Application.Features.Quizzes.Commands;
using QuizService.Application.Models;
using QuizService.Domain.Models;

namespace QuizService.Application.Features.Quizzes.CommandHandlers
{
    public class CreateQuizCommandHandler : IRequestHandler<CreateQuizCommand, OperationResult<int>>
    {
        private readonly IDbConnection _connection;

        public CreateQuizCommandHandler(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<OperationResult<int>> Handle(CreateQuizCommand request, CancellationToken cancellationToken)
        {
            var result = new OperationResult<int>();

            var quiz = Quiz.CreateQuiz(request.Title);
            var sqlCommand = $"INSERT INTO Quiz (Title) VALUES('{quiz.Title}'); SELECT LAST_INSERT_ROWID();";

            try
            {
                var id = await _connection.ExecuteScalarAsync(sqlCommand);
                result.AddValue((int)Convert.ToInt64(id));
            }
            catch (Exception ex)
            {
                result.AddError(HttpStatusCode.InternalServerError, ex.Message);
            }

            return result;
        }
    }
}

