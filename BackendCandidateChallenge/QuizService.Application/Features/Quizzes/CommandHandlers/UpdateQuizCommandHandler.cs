using System;
using System.Data;
using System.Net;
using Dapper;
using MediatR;
using QuizService.Application.Features.Quizzes.Commands;
using QuizService.Application.Models;
using QuizService.Domain.Models;

namespace QuizService.Application.Features.Quizzes.CommandHandlers
{
    public class UpdateQuizCommandHandler : IRequestHandler<UpdateQuizCommand, OperationResult<Quiz>>
    {
        private readonly IDbConnection _connection;

        public UpdateQuizCommandHandler(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<OperationResult<Quiz>> Handle(UpdateQuizCommand request, CancellationToken cancellationToken)
        {
            var result = new OperationResult<Quiz>();

            string sql = "UPDATE Quiz SET Title = @Title WHERE Id = @Id";

            try
            {
                int rowsUpdated = await _connection.ExecuteAsync(sql, new { Id = request.Id, Title = request.Title });

                if (rowsUpdated is 0)
                {
                    result.AddError(HttpStatusCode.NotFound, $"No quiz found with ID {request.Id}");
                }
            }
            catch (Exception ex)
            {
                result.AddError(HttpStatusCode.NotFound, ex.Message);
            }

            return result;
        }
    }
}