using System;
using QuizService.Domain.Exceptions;
using QuizService.Domain.Validations;

namespace QuizService.Domain.Models;

public class Quiz
{
    public int Id { get; private set; }
    public string Title { get; private set; }

    public Quiz()
    {
    }

    public static Quiz CreateQuiz(string title)
    {
        var objectToValidate = new Quiz
        {
            Title = title
        };

        var validationResult = new QuizValidator().Validate(objectToValidate);

        if (validationResult.IsValid) return objectToValidate;

        var exception = new NotValidException("The post is not valid");

        validationResult.Errors.ForEach(vr => exception.ValidationErrors.Add(vr.ErrorMessage));

        throw exception;
    }
}
