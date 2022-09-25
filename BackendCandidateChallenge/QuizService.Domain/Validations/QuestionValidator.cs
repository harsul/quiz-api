using System;
using FluentValidation;
using QuizService.Domain.Models;

namespace QuizService.Domain.Validations;

public class QuestionValidator : AbstractValidator<Question>
{
    public QuestionValidator()
    {
        RuleFor(question => question.Text).NotNull().WithMessage("Text content could not be null")
            .NotEmpty().WithMessage("Text content could not be empty");
    }
}

