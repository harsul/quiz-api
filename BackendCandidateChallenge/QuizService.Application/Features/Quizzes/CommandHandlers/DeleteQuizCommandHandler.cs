using System.Data;
using System.Net;
using Dapper;
using MediatR;
using QuizService.Application.Features.Quizzes.Commands;
using QuizService.Application.Models;
using QuizService.Domain.Models;

namespace QuizService.Application.Features.Quizzes.CommandHandlers
{
    public class DeleteQuizCommandHandler : IRequestHandler<DeleteQuizCommand, OperationResult<Quiz>>
    {
        private readonly IDbConnection _connection;

        public DeleteQuizCommandHandler(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<OperationResult<Quiz>> Handle(DeleteQuizCommand request, CancellationToken cancellationToken)
        {
            var result = new OperationResult<Quiz>();

            const string sqlCommand = "DELETE FROM Quiz WHERE Id = @Id";

            try
            {
                int rowsDeleted = await _connection.ExecuteAsync(sqlCommand, new { request.Id });

                if (rowsDeleted is 0)
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
