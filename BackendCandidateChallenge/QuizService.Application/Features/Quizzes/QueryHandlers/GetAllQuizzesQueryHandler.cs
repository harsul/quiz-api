using System;
using System.Data;
using Dapper;
using MediatR;
using QuizService.Application.Features.Quizzes.Queries;
using QuizService.Application.Models;
using QuizService.Domain.Models;

namespace QuizService.Application.Features.Quizzes.QueryHandlers
{
    public class GetAllQuizzesQueryHandler : IRequestHandler<GetAllQuizzesQuery, OperationResult<List<Quiz>>>
    {
        private readonly IDbConnection _connection;

        public GetAllQuizzesQueryHandler(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<OperationResult<List<Quiz>>> Handle(GetAllQuizzesQuery request, CancellationToken cancellationToken)
        {
            var result = new OperationResult<List<Quiz>>();

            //TODO move const string in static SqlQueries class so all queries could be managed from one place
            const string sqlCommand = "SELECT * FROM Quiz;";
            var quizzes = await _connection.QueryAsync<Quiz>(sqlCommand);

            result.AddValue(quizzes.ToList());

            return result;
        }
    }
}
