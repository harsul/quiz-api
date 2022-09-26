using AutoMapper;
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
using QuizService.Filters;
using QuizService.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace QuizService.Controllers;

[Route(ApiRoutes.BaseRoute)]
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

    //TODO in case that we don't want to show some data from database operation result
    //we could use AutoMapper to map data transfer object that will be presented in API response
    //this could be implemented for all Get responses

    // GET api/quizzes/5
    [HttpGet(ApiRoutes.IdRoute)]
    public async Task<IActionResult> Get(int quizId)
    {
        var quizResult = await _mediator
            .Send(new GetQuizByIdQuery { Id = quizId });

        if (quizResult.IsError) return HandleErrorResponse(quizResult.StatusCode, quizResult.ErrorMessage);

        var questionsResult = await _mediator
            .Send(new GetQuestionsByQuizIdQuery { QuizId = quizId });

        if (questionsResult.Value.Count is 0)
            return Ok(quizResult.Value);

        var asnwersResult = await _mediator
            .Send(new GetAnswersByQuizIdQuery { QuizId = quizId });

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
                {"self", $"/api/quizzes/{quizId}"},
                {"questions", $"/api/quizzes/{quizId}/questions"}
            }
        };

        return Ok(quiz);
    }

    // POST api/quizzes
    [HttpPost]
    [ValidateModel]
    public async Task<IActionResult> Post([FromBody] QuizCreateRequest request)
    {
        var result = await _mediator
            .Send(request: new CreateQuizCommand { Title = request.Title });

        return result.IsError
            ? HandleErrorResponse(result.StatusCode, result.ErrorMessage)
            : CreatedAtAction(nameof(Get), new { quizId = result.Value }, null);
    }

    // PUT api/quizzes/5
    [HttpPut(ApiRoutes.IdRoute)]
    [ValidateModel]
    public async Task<IActionResult> Put(int quizId, [FromBody] QuizUpdateRequest rewuest)
    {
        var result = await _mediator
            .Send(new UpdateQuizCommand { Id = quizId, Title = rewuest.Title });

        return result.IsError
            ? HandleErrorResponse(result.StatusCode, result.ErrorMessage)
            : NoContent();
    }

    // DELETE api/quizzes/5
    [HttpDelete(ApiRoutes.IdRoute)]
    public async Task<IActionResult> Delete(int quizId)
    {
        var result = await _mediator
            .Send(new DeleteQuizCommand { Id = quizId });

        return result.IsError
            ? HandleErrorResponse(result.StatusCode, result.ErrorMessage)
            : NoContent();
    }

    // POST api/quizzes/5/questions
    [HttpPost]
    [Route(ApiRoutes.Questions.BaseRoute)]
    [ValidateModel]
    public async Task<IActionResult> PostQuestion(int quizId, [FromBody] QuestionCreateRequest request)
    {
        var result = await _mediator
            .Send(new CreateQuestionCommand { QuizId = quizId, Text = request.Text });

        return result.IsError
            ? HandleErrorResponse(result.StatusCode, result.ErrorMessage)
            : Created($"/api/quizzes/{quizId}/questions/{result.Value}", null);
    }

    // PUT api/quizzes/5/questions/6
    [HttpPut(ApiRoutes.Questions.IdRoute)]
    [ValidateModel]
    public async Task<IActionResult> PutQuestion(int quizId, int questionId, [FromBody] QuestionUpdateRequest request)
    {
        var result = await _mediator
            .Send(new UpdateQuestionCommand { QuestionId = questionId, Text = request.Text, CorrectAnswerId = request.CorrectAnswerId });

        return result.IsError
            ? HandleErrorResponse(result.StatusCode, result.ErrorMessage)
            : NoContent();
    }

    // DELETE api/quizzes/5/questions/6
    [HttpDelete]
    [Route(ApiRoutes.Questions.IdRoute)]
    public async Task<IActionResult> DeleteQuestion(int quizId, int questionId)
    {
        var result = await _mediator
            .Send(new DeleteQuestionCommand { QuestionId = questionId });

        return result.IsError
            ? HandleErrorResponse(result.StatusCode, result.ErrorMessage)
            : NoContent();
    }

    // POST api/quizzes/5/questions/6/answers
    [HttpPost]
    [Route(ApiRoutes.Answers.BaseRoute)]
    [ValidateModel]
    public async Task<IActionResult> PostAnswer(int quizId, int questionId, [FromBody] AnswerCreateRequest request)
    {
        var result = await _mediator
            .Send(new CreateAnswerCommand { QuestionId = questionId, Text = request.Text });

        return result.IsError
            ? HandleErrorResponse(result.StatusCode, result.ErrorMessage)
            : Created($"/api/quizzes/{quizId}/questions/{questionId}/answers/{result.Value}", null);
    }

    // PUT api/quizzes/5/questions/6/answers/7
    [HttpPut(ApiRoutes.Answers.IdRoute)]
    [ValidateModel]
    public async Task<IActionResult> PutAnswer(int quizId, int questionId, int answerId, [FromBody] AnswerUpdateRequest request)
    {
        var result = await _mediator
            .Send(new UpdateAnswerCommand { AnswerId = answerId, Text = request.Text });

        return result.IsError
            ? HandleErrorResponse(result.StatusCode, result.ErrorMessage)
            : NoContent();
    }

    // DELETE api/quizzes/5/questions/6/answers/7
    [HttpDelete]
    [Route(ApiRoutes.Answers.IdRoute)]
    public async Task<IActionResult> DeleteAnswer(int quizId, int questionId, int answerId)
    {
        var result = await _mediator
             .Send(new DeleteAnswerCommand { AnswerId = answerId });

        return result.IsError
            ? HandleErrorResponse(result.StatusCode, result.ErrorMessage)
            : NoContent();
    }
}