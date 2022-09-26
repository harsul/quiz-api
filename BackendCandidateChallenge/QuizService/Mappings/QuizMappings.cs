using System;
using AutoMapper;
using QuizService.Contracts.Answers.Responses;
using QuizService.Contracts.Questions.Responses;
using QuizService.Contracts.Quizzes.Responses;
using QuizService.Domain.Models;

namespace QuizService.Mappings;

public class QuizMappings : Profile
{
    public QuizMappings()
    {
        CreateMap<Quiz, QuizResponse>();
        CreateMap<Question, QuestionResponse>();
        CreateMap<Answer, AnswerResponse>();
    }
}