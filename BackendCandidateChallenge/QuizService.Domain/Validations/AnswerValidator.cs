using System;
using FluentValidation;
using QuizService.Domain.Models;

namespace QuizService.Domain.Validations;

public class AnswerValidator : AbstractValidator<Answer>
{
    public AnswerValidator()
    {
        RuleFor(answer => answer.Text).NotNull().WithMessage("Text content could not be null")
            .NotEmpty().WithMessage("Text content could not be empty");
    }
}

