using System;
using System.Net;

namespace QuizService.Application.Models;

public class OperationResult<T>
{
    public bool IsError { get; private set; }
    public HttpStatusCode StatusCode { get; private set; }
    public T Value { get; private set; }
    public string ErrorMessage { get; private set; }

    public void AddError(HttpStatusCode code, string message)
    {
        IsError = true;
        StatusCode = code;
        ErrorMessage = message;
    }

    public void AddValue(T value)
    {
        Value = value;
    }
}