using System;
using System.Data;
using Dapper;
using System.Net;
using MediatR;
using QuizService.Application.Features.Answers.Commands;
using QuizService.Application.Models;
using QuizService.Domain.Models;
using System.Security.Cryptography;

namespace QuizService.Application.Features.Answers.CommandHandlers;

public class UpdateAnswerCommandHandler : IRequestHandler<UpdateAnswerCommand, OperationResult<Answer>>
{
    private readonly IDbConnection _connection;

    public UpdateAnswerCommandHandler(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<OperationResult<Answer>> Handle(UpdateAnswerCommand request, CancellationToken cancellationToken)
    {
        var result = new OperationResult<Answer>();

        const string sqlCommand = "UPDATE Answer SET Text = @Text WHERE Id = @AnswerId";

        try
        {
            int rowsUpdated = await _connection.ExecuteAsync(sqlCommand, new { request.AnswerId, request.Text });

            if (rowsUpdated is 0)
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
