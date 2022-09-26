using System;
using System.Data;
using Dapper;
using System.Net;
using MediatR;
using QuizService.Application.Features.Answers.Commands;
using QuizService.Application.Models;
using QuizService.Domain.Models;

namespace QuizService.Application.Features.Answers.CommandHandlers;

public class DeleteAnswerCommandHandler : IRequestHandler<DeleteAnswerCommand, OperationResult<Answer>>
{
    private readonly IDbConnection _connection;

    public DeleteAnswerCommandHandler(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<OperationResult<Answer>> Handle(DeleteAnswerCommand request, CancellationToken cancellationToken)
    {
        var result = new OperationResult<Answer>();

        const string sqlCommand = "DELETE FROM Answer WHERE Id = @AnswerId";

        try
        {
            int rowsDeleted = await _connection.ExecuteAsync(sqlCommand, new { request.AnswerId });

            if (rowsDeleted is 0)
            {
                result.AddError(HttpStatusCode.NotFound, $"No answer found with ID {request.AnswerId}");
            }
        }
        catch (Exception ex)
        {
            result.AddError(HttpStatusCode.NotFound, ex.Message);
        }

        return result;
    }
}
