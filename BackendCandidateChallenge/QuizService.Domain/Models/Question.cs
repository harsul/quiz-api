using System;
using QuizService.Domain.Exceptions;
using QuizService.Domain.Validations;

namespace QuizService.Domain.Models;

public class Question
{
    public int Id { get; }
    public int QuizId { get; private set; }
    public string Text { get; private set; }
    public int CorrectAnswerId { get; private set; }

    public Question()
    {
    }

    public static Question CreateQuestion(int quizId, string text)
    {
        var objectToValidate = new Question
        {
            QuizId = quizId,
            Text = text,
        };

        var validationResult = new QuestionValidator().Validate(objectToValidate);

        if (validationResult.IsValid) return objectToValidate;

        var exception = new NotValidException("The question is not valid");

        validationResult.Errors.ForEach(vr => exception.ValidationErrors.Add(vr.ErrorMessage));

        throw exception;
    }

    //TODO add method to update Text so full encapsulation could be done
    //and add validation for that model
}