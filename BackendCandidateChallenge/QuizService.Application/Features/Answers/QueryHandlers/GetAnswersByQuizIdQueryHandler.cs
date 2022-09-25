using System;
using System.Data;
using Dapper;
using MediatR;
using QuizService.Application.Features.Answers.Queries;
using QuizService.Application.Models;
using QuizService.Domain.Models;

namespace QuizService.Application.Features.Answers.QueryHandlers
{
    public class GetAnswersByQuizIdQueryHandler : IRequestHandler<GetAnswersByQuizIdQuery, OperationResult<Dictionary<int, IList<Answer>>>>
    {
        private readonly IDbConnection _connection;

        public GetAnswersByQuizIdQueryHandler(IDbConnection connection)
        {
            _connection = connection;
        }

        public Task<OperationResult<Dictionary<int, IList<Answer>>>> Handle(GetAnswersByQuizIdQuery request, CancellationToken cancellationToken)
        {
            var result = new OperationResult<Dictionary<int, IList<Answer>>>();

            const string answersSql = "SELECT a.Id, a.Text, a.QuestionId FROM Answer a INNER JOIN Question q ON a.QuestionId = q.Id WHERE q.QuizId = @QuizId;";
            var answers = _connection.Query<Answer>(answersSql, new { QuizId = request.QuizId })
                .Aggregate(new Dictionary<int, IList<Answer>>(), (dict, answer) =>
                {
                    if (!dict.ContainsKey(answer.QuestionId))
                        dict.Add(answer.QuestionId, new List<Answer>());
                    dict[answer.QuestionId].Add(answer);
                    return dict;
                });

            result.AddValue(answers);

            return Task.FromResult(result);
        }
    }
}
