using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using QuizService.Contracts.Common;

namespace QuizService.Filters;

public class ApplicationExceptionHandler : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        var error = new ErrorResponse
        {
            StatusCode = HttpStatusCode.InternalServerError,
            Timestamp = DateTime.UtcNow
        };
        error.ErrorMessages.Add(context.Exception.Message);

        context.Result = new JsonResult(error) { StatusCode = 500 };
    }
}