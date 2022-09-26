using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using QuizService.Contracts.Answers.Request;
using QuizService.Contracts.Answers.Responses;
using QuizService.Contracts.Questions.Request;
using QuizService.Contracts.Quizzes.Responses;
using QuizService.Domain.Models;
using QuizService.Model;
using Xunit;

namespace QuizService.Tests;

public class QuizzesControllerTest
{
    const string QuizApiEndPoint = "/api/quizzes/";

    //TODO try to use RestSharp client library to get and post requests
    //request and response could be serizalized/deserialized automaticaly
    //https://restsharp.dev/serialization.html

    [Fact]
    public async Task PostNewQuizAddsQuiz()
    {
        var quiz = new QuizCreateRequest { Title = "Test title" };
        using var testHost = new TestServer(new WebHostBuilder()
                   .UseStartup<Startup>());
        var client = testHost.CreateClient();
        var content = new StringContent(JsonConvert.SerializeObject(quiz));
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        var response = await client.PostAsync(new Uri(testHost.BaseAddress, QuizApiEndPoint),
            content);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
    }

    [Fact]
    public async Task AQuizExistGetReturnsQuiz()
    {
        using var testHost = new TestServer(new WebHostBuilder()
                   .UseStartup<Startup>());
        var client = testHost.CreateClient();
        const long quizId = 1;
        var response = await client.GetAsync(new Uri(testHost.BaseAddress, $"{QuizApiEndPoint}{quizId}"));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response.Content);
        var quiz = JsonConvert.DeserializeObject<QuizResponse>(await response.Content.ReadAsStringAsync());
        Assert.Equal(quizId, quiz.Id);
        Assert.Equal("My first quiz", quiz.Title);
    }

    [Fact]
    public async Task AQuizDoesNotExistGetFails()
    {
        using var testHost = new TestServer(new WebHostBuilder()
                   .UseStartup<Startup>());
        var client = testHost.CreateClient();
        const long quizId = 999;
        var response = await client.GetAsync(new Uri(testHost.BaseAddress, $"{QuizApiEndPoint}{quizId}"));
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AQuizDoesNotExists_WhenPostingAQuestion_ReturnsNotFound()
    {
        const long quizId = 999;
        string QuizApiEndPoint = $"/api/quizzes/{quizId}/questions";

        using var testHost = new TestServer(new WebHostBuilder()
                   .UseStartup<Startup>());
        var client = testHost.CreateClient();
        var question = new QuestionCreateRequest { Text = "The answer to everything is what?" };
        var content = new StringContent(JsonConvert.SerializeObject(question));
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        var response = await client.PostAsync(new Uri(testHost.BaseAddress, QuizApiEndPoint), content);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PostQuiz_PostTwoQuestions_PostTwoAnswersForEachQuestion()
    {
        const string QuizApiEndPoint = "/api/quizzes";

        using var testHost = new TestServer(new WebHostBuilder().UseStartup<Startup>());
        var client = testHost.CreateClient();

        var quiz = GetQuizCreateRequest();
        var content = new StringContent(JsonConvert.SerializeObject(quiz));
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        var createQuizResponse = await client.PostAsync(new Uri(testHost.BaseAddress, QuizApiEndPoint), content);
        Assert.Equal(HttpStatusCode.Created, createQuizResponse.StatusCode);
        Assert.NotNull(createQuizResponse.Headers.Location);

        Int32.TryParse(createQuizResponse.Headers.Location.ToString().Split("/").Last(), out int quizId);

        foreach (var question in GetQuestions(quizId))
        {
            var questionPostApiEndPoint = $"/api/quizzes/{quizId}/questions";
            content = new StringContent(JsonConvert.SerializeObject(question));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var createQuestionResponse = await client.PostAsync(new Uri(testHost.BaseAddress, questionPostApiEndPoint), content);
            Assert.Equal(HttpStatusCode.Created, createQuestionResponse.StatusCode);
            Assert.NotNull(createQuestionResponse.Headers.Location);

            Int32.TryParse(createQuestionResponse.Headers.Location.ToString().Split("/").Last(), out int questionId);

            int correctAnswerId = 0;
            foreach (var answer in GetAnswers(questionId))
            {
                var answerPostApiEndPoint = $"/api/quizzes/{quizId}/questions/{questionId}/answers";
                content = new StringContent(JsonConvert.SerializeObject(answer));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var createAnswerResponse = await client.PostAsync(new Uri(testHost.BaseAddress, answerPostApiEndPoint), content);
                Assert.Equal(HttpStatusCode.Created, createAnswerResponse.StatusCode);
                Assert.NotNull(createAnswerResponse.Headers.Location);

                Int32.TryParse(createAnswerResponse.Headers.Location.ToString().Split("/").Last(), out correctAnswerId);
            }

            var questionPutApiEndPoint = $"/api/quizzes/{quizId}/questions/{questionId}";
            var updateQuestionRequest = new QuestionUpdateRequest { CorrectAnswerId = correctAnswerId, Text = $"This question with id: {questionId} is updated" };
            content = new StringContent(JsonConvert.SerializeObject(updateQuestionRequest));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var updateQuestionResponse = await client.PutAsync(new Uri(testHost.BaseAddress, questionPutApiEndPoint), content);
            Assert.Equal(HttpStatusCode.NoContent, updateQuestionResponse.StatusCode);
        }
    }

    [Fact]
    public async Task GetQuizById_ContainsTwoQuestions_FirstQuestionContainsTwoAnswers()
    {
        using var testHost = new TestServer(new WebHostBuilder()
                   .UseStartup<Startup>());
        var client = testHost.CreateClient();
        var quizId = 1;
        var response = await client.GetAsync(new Uri(testHost.BaseAddress, $"{QuizApiEndPoint}{quizId}"));

        response.Content.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var quiz = JsonConvert.DeserializeObject<QuizResponse>(await response.Content.ReadAsStringAsync());
        var questions = quiz.Questions.ToList();
        var answers = questions.FirstOrDefault().Answers;

        quiz.Should().BeOfType<QuizResponse>();
        quiz.Questions.Should().HaveCount(2);
        answers.Should().HaveCount(2);
    }

    public QuizCreateRequest GetQuizCreateRequest()
    {
        return new QuizCreateRequest { Title = "New quiz" };
    }

    public List<QuestionCreateRequest> GetQuestions(int quizId)
    {
        return new List<QuestionCreateRequest>
        {
            new QuestionCreateRequest {Text = $"This is first question for quiz id: {quizId}"},
            new QuestionCreateRequest {Text = $"This is second question for quiz id: {quizId}"},
        };
    }

    public List<AnswerCreateRequest> GetAnswers(int questionId)
    {
        return new List<AnswerCreateRequest>
        {
            new AnswerCreateRequest {Text = $"This is first answer for question id: {questionId}"},
            new AnswerCreateRequest {Text = $"This is second answer for question id: {questionId}"},
        };
    }
}