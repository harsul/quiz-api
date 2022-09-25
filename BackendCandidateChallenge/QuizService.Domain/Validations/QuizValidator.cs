using System;
using FluentValidation;
using QuizService.Domain.Models;

namespace QuizService.Domain.Validations;

public class QuizValidator : AbstractValidator<Quiz>
{
    public QuizValidator()
    {
        RuleFor(quiz => quiz.Title).NotNull().WithMessage("Text content could not be null")
            .NotEmpty().WithMessage("Text content could not be empty");
    }
}

