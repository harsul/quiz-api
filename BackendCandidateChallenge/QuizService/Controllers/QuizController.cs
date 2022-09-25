using AutoMapper;
using Dapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using QuizService.Application.Features.Questions.Commands;
using QuizService.Application.Features.Quizzes.Commands;
using QuizService.Application.Features.Quizzes.Queries;
using QuizService.Contracts.Answers.Request;
using QuizService.Contracts.Answers.Responses;
using QuizService.Contracts.Questions.Request;
using QuizService.Contracts.Questions.Responses;
using QuizService.Contracts.Quizzes.Responses;
using QuizService.Domain.Models;
using QuizService.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace QuizService.Controllers;

[Route("api/quizzes")]
public class QuizController : BaseController
{
    private readonly IDbConnection _connection;
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public QuizController(IDbConnection connection, IMediator mediator, IMapper mapper)
    {
        _connection = connection;
        _mediator = mediator;
        _mapper = mapper;
    }

    // GET api/quizzes
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = await _mediator
            .Send(new GetAllQuizzesQuery());

        return Ok(result.Value);
    }

    // GET api/quizzes/5
    [HttpGet("{id}")]
    public object Get(int id)
    {
        const string quizSql = "SELECT * FROM Quiz WHERE Id = @Id;";
        var quiz = _connection.QuerySingle<Quiz>(quizSql, new { Id = id });
        if (quiz == null)
            return NotFound();
        const string questionsSql = "SELECT * FROM Question WHERE QuizId = @QuizId;";
        var questions = _connection.Query<Question>(questionsSql, new { QuizId = id });
        const string answersSql = "SELECT a.Id, a.Text, a.QuestionId FROM Answer a INNER JOIN Question q ON a.QuestionId = q.Id WHERE q.QuizId = @QuizId;";
        var answers = _connection.Query<Answer>(answersSql, new { QuizId = id })
            .Aggregate(new Dictionary<int, IList<Answer>>(), (dict, answer) =>
            {
                if (!dict.ContainsKey(answer.QuestionId))
                    dict.Add(answer.QuestionId, new List<Answer>());
                dict[answer.QuestionId].Add(answer);
                return dict;
            });

        return new QuizResponse
        {
            Id = quiz.Id,
            Title = quiz.Title,
            Questions = questions.Select(question => new QuestionResponse
            {
                Id = question.Id,
                Text = question.Text,
                Answers = answers.ContainsKey(question.Id)
                    ? answers[question.Id].Select(answer => new AnswerResponse
                    {
                        Id = answer.Id,
                        Text = answer.Text
                    })
                    : Array.Empty<AnswerResponse>(),
                CorrectAnswerId = question.CorrectAnswerId
            }),
            Links = new Dictionary<string, string>
            {
                {"self", $"/api/quizzes/{id}"},
                {"questions", $"/api/quizzes/{id}/questions"}
            }
        };
    }

    // POST api/quizzes
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] QuizCreateRequest value)
    {
        var result = await _mediator
            .Send(request: new CreateQuizCommand { Title = value.Title });

        return result.IsError
            ? HandleErrorResponse(result.StatusCode, result.ErrorMessage)
            : CreatedAtAction(nameof(Get), new { id = result.Value.Id }, null);
    }

    // PUT api/quizzes/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] QuizUpdateRequest value)
    {
        var result = await _mediator
            .Send(new UpdateQuizCommand { Id = id, Title = value.Title });

        return result.IsError
            ? HandleErrorResponse(result.StatusCode, result.ErrorMessage)
            : NoContent();
    }

    // DELETE api/quizzes/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _mediator
            .Send(new DeleteQuizCommand { Id = id });

        return result.IsError
            ? HandleErrorResponse(result.StatusCode, result.ErrorMessage)
            : NoContent();
    }

    // POST api/quizzes/5/questions
    [HttpPost]
    [Route("{id}/questions")]
    public async Task<IActionResult> PostQuestion(int id, [FromBody] QuestionCreateRequest value)
    {
        var result = await _mediator
            .Send(new CreateQuestionCommand { QuizId = id, Text = value.Text });

        return result.IsError
            ? HandleErrorResponse(result.StatusCode, result.ErrorMessage)
            : Created($"/api/quizzes/{id}/questions/{result.Value}", null);
    }

    // PUT api/quizzes/5/questions/6
    [HttpPut("{id}/questions/{qid}")]
    public async Task<IActionResult> PutQuestion(int id, int qid, [FromBody] QuestionUpdateRequest value)
    {
        var result = await _mediator
            .Send(new UpdateQuestionCommand { QuestionId = qid, Text = value.Text, CorrectAnswerId = value.CorrectAnswerId });

        return result.IsError
            ? HandleErrorResponse(result.StatusCode, result.ErrorMessage)
            : NoContent();
    }

    // DELETE api/quizzes/5/questions/6
    [HttpDelete]
    [Route("{id}/questions/{qid}")]
    public async Task<IActionResult> DeleteQuestion(int id, int qid)
    {
        var result = await _mediator
            .Send(new DeleteQuestionCommand { QuestionId = qid });

        return result.IsError
            ? HandleErrorResponse(result.StatusCode, result.ErrorMessage)
            : NoContent();
    }

    // POST api/quizzes/5/questions/6/answers
    [HttpPost]
    [Route("{id}/questions/{qid}/answers")]
    public IActionResult PostAnswer(int id, int qid, [FromBody] AnswerCreateRequest value)
    {
        const string sql = "INSERT INTO Answer (Text, QuestionId) VALUES(@Text, @QuestionId); SELECT LAST_INSERT_ROWID();";
        var answerId = _connection.ExecuteScalar(sql, new { Text = value.Text, QuestionId = qid });
        return Created($"/api/quizzes/{id}/questions/{qid}/answers/{answerId}", null);
    }

    // PUT api/quizzes/5/questions/6/answers/7
    [HttpPut("{id}/questions/{qid}/answers/{aid}")]
    public IActionResult PutAnswer(int id, int qid, int aid, [FromBody] AnswerUpdateRequest value)
    {
        const string sql = "UPDATE Answer SET Text = @Text WHERE Id = @AnswerId";
        int rowsUpdated = _connection.Execute(sql, new { AnswerId = qid, Text = value.Text });
        if (rowsUpdated == 0)
            return NotFound();
        return NoContent();
    }

    // DELETE api/quizzes/5/questions/6/answers/7
    [HttpDelete]
    [Route("{id}/questions/{qid}/answers/{aid}")]
    public IActionResult DeleteAnswer(int id, int qid, int aid)
    {
        const string sql = "DELETE FROM Answer WHERE Id = @AnswerId";
        _connection.ExecuteScalar(sql, new { AnswerId = aid });
        return NoContent();
    }
}