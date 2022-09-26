using System;
using System.Data;
using System.Net;
using Dapper;
using MediatR;
using QuizService.Application.Features.Questions.Commands;
using QuizService.Application.Models;
using QuizService.Domain.Models;

namespace QuizService.Application.Features.Questions.CommandHandlers
{
    public class CreateQuestionCommandHandler : IRequestHandler<CreateQuestionCommand, OperationResult<int>>
    {
        private readonly IDbConnection _connection;

        public CreateQuestionCommandHandler(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<OperationResult<int>> Handle(CreateQuestionCommand request, CancellationToken cancellationToken)
        {
            var result = new OperationResult<int>();

            const string sqlCommand = "INSERT INTO Question (Text, QuizId) VALUES(@Text, @QuizId); SELECT LAST_INSERT_ROWID();";

            var question = Question.CreateQuestion(request.QuizId, request.Text);

            try
            {
                var questionId = await _connection
                    .ExecuteScalarAsync(sqlCommand, new { question.Text, question.QuizId });

                result.AddValue((int)Convert.ToInt64(questionId));
            }
            catch (Exception ex)
            {
                result.AddError(HttpStatusCode.NotFound, ex.Message);
            }

            return result;
        }
    }
}