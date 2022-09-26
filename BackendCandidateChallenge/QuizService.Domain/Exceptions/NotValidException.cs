using System;
namespace QuizService.Domain.Exceptions;

internal class NotValidException : Exception
{
    internal List<string> ValidationErrors { get; }

    internal NotValidException()
    {
        ValidationErrors = new();
    }

    internal NotValidException(string message) : base(message)
    {
        ValidationErrors = new();
    }

    internal NotValidException(string message, Exception inner) : base(message, inner)
    {
        ValidationErrors = new();
    }
}

