using AutoMapper;
using Dapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using QuizService.Application.Features.Answers.Commands;
using QuizService.Application.Features.Answers.Queries;
using QuizService.Application.Features.Questions.Commands;
using QuizService.Application.Features.Questions.Queries;
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
    public async Task<IActionResult> Get(int id)
    {
        var quizResult = await _mediator
            .Send(new GetQuizByIdQuery { Id = id });

        if (quizResult.IsError) return HandleErrorResponse(quizResult.StatusCode, quizResult.ErrorMessage);

        var questionsResult = await _mediator
            .Send(new GetQuestionsByQuizIdQuery { QuizId = id });

        if (questionsResult.Value.Count is 0)
            return Ok(quizResult.Value);

        var asnwersResult = await _mediator
            .Send(new GetAnswersByQuizIdQuery { QuizId = id });

        var quiz =  new QuizResponse
        {
            Id = quizResult.Value.Id,
            Title = quizResult.Value.Title,
            Questions = questionsResult.Value.Select(question => new QuestionResponse
            {
                Id = question.Id,
                Text = question.Text,
                Answers = asnwersResult.Value.ContainsKey(question.Id)
                    ? asnwersResult.Value[question.Id].Select(answer => new AnswerResponse
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

        return Ok(quiz);
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
    public async Task<IActionResult> PostAnswer(int id, int qid, [FromBody] AnswerCreateRequest value)
    {
        var result = await _mediator
            .Send(new CreateAnswerCommand { QuestionId = qid, Text = value.Text });

        return result.IsError
            ? HandleErrorResponse(result.StatusCode, result.ErrorMessage)
            : Created($"/api/quizzes/{id}/questions/{qid}/answers/{result.Value}", null);
    }

    // PUT api/quizzes/5/questions/6/answers/7
    [HttpPut("{id}/questions/{qid}/answers/{aid}")]
    public async Task<IActionResult> PutAnswer(int id, int qid, int aid, [FromBody] AnswerUpdateRequest value)
    {
        var result = await _mediator
            .Send(new UpdateAnswerCommand { AnswerId = aid, Text = value.Text });

        return result.IsError
            ? HandleErrorResponse(result.StatusCode, result.ErrorMessage)
            : NoContent();
    }

    // DELETE api/quizzes/5/questions/6/answers/7
    [HttpDelete]
    [Route("{id}/questions/{qid}/answers/{aid}")]
    public async Task<IActionResult> DeleteAnswer(int id, int qid, int aid)
    {
        var result = await _mediator
             .Send(new DeleteAnswerCommand { AnswerId = aid });

        return result.IsError
            ? HandleErrorResponse(result.StatusCode, result.ErrorMessage)
            : NoContent();
    }
}