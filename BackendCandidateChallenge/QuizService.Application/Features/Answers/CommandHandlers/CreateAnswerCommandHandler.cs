using System;
using System.Data;
using Dapper;
using System.Net;
using MediatR;
using QuizService.Application.Features.Answers.Commands;
using QuizService.Application.Models;
using QuizService.Domain.Models;
using System.Security.Cryptography;

namespace QuizService.Application.Features.Answers.CommandHandlers
{
    public class CreateAnswerCommandHandler : IRequestHandler<CreateAnswerCommand, OperationResult<int>>
    {
        private readonly IDbConnection _connection;

        public CreateAnswerCommandHandler(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<OperationResult<int>> Handle(CreateAnswerCommand request, CancellationToken cancellationToken)
        {
            var result = new OperationResult<int>();

            var answer = Answer.CreateAnswer(request.QuestionId, request.Text);

            const string sqlCommand = "INSERT INTO Answer (Text, QuestionId) VALUES(@Text, @QuestionId); SELECT LAST_INSERT_ROWID();";

            try
            {
                var answerId = await _connection.ExecuteScalarAsync(sqlCommand, new { answer.Text, answer.QuestionId });
                result.AddValue((int)Convert.ToInt64(answerId));
            }
            catch (Exception ex)
            {
                result.AddError(HttpStatusCode.InternalServerError, ex.Message);
            }

            return result;
        }
    }
}
