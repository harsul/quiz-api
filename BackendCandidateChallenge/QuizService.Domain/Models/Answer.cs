using System;
using QuizService.Domain.Exceptions;
using QuizService.Domain.Validations;

namespace QuizService.Domain.Models;

public class Answer
{
    public int Id { get; }
    public int QuestionId { get; private set; }
    public string Text { get; private set; }

    public Answer()
    {
    }

    public static Answer CreateAnswer(int questionId, string text)
    {
        var objectToValidate = new Answer
        {
            QuestionId = questionId,
            Text = text
        };

        var validationResult = new AnswerValidator().Validate(objectToValidate);

        if (validationResult.IsValid) return objectToValidate;

        var exception = new NotValidException("The answer is not valid");

        validationResult.Errors.ForEach(vr => exception.ValidationErrors.Add(vr.ErrorMessage));

        throw exception;
    }

    //TODO add method to update Text so full encapsulation could be done
    //and add validation for that model
}

