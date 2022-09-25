using System;
using System.Data;
using Dapper;
using System.Net;
using MediatR;
using QuizService.Application.Features.Questions.Commands;
using QuizService.Application.Features.Quizzes.Commands;
using QuizService.Application.Models;
using QuizService.Domain.Models;

namespace QuizService.Application.Features.Questions.CommandHandlers;

public class DeleteQUestionCommandHandler : IRequestHandler<DeleteQuestionCommand, OperationResult<Question>>
{
    private readonly IDbConnection _connection;

    public DeleteQUestionCommandHandler(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<OperationResult<Question>> Handle(DeleteQuestionCommand request, CancellationToken cancellationToken)
    {
        var result = new OperationResult<Question>();

        const string sqlCommand = "DELETE FROM Question WHERE Id = @QuestionId";

        try
        {
            int rowsDeleted = await _connection.ExecuteAsync(sqlCommand, new { QuestionId = request.QuestionId });

            if (rowsDeleted is 0)
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


