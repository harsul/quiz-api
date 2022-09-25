using System;
using System.Data;
using Dapper;
using System.Net;
using MediatR;
using QuizService.Application.Features.Questions.Commands;
using QuizService.Application.Models;
using QuizService.Domain.Models;
using System.Security.Cryptography;

namespace QuizService.Application.Features.Questions.CommandHandlers;

public class UpdateQuestionCommandHandler : IRequestHandler<UpdateQuestionCommand, OperationResult<Question>>
{
    private readonly IDbConnection _connection;

    public UpdateQuestionCommandHandler(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<OperationResult<Question>> Handle(UpdateQuestionCommand request, CancellationToken cancellationToken)
    {
        var result = new OperationResult<Question>();

        var sqlCommand = "UPDATE Question SET Text = @Text, CorrectAnswerId = @CorrectAnswerId WHERE Id = @QuestionId";
       
        try
        {
            int rowsUpdated = await _connection
                .ExecuteAsync(sqlCommand,
                    new { QuestionId = request.QuestionId, Text = request.Text, CorrectAnswerId = request.CorrectAnswerId });

            if (rowsUpdated is 0)
            {
                result.AddError(HttpStatusCode.NotFound, $"No question found with ID {request.QuestionId}");
            }
        }
        catch (Exception ex)
        {
            result.AddError(HttpStatusCode.NotFound, ex.Message);
        }

        return result;
    }
}