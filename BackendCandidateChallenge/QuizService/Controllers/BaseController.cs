using System;
using Microsoft.AspNetCore.Mvc;
using QuizService.Contracts.Common;
using System.Net;

namespace QuizService.Controllers;

public class BaseController : ControllerBase
{
    protected IActionResult HandleErrorResponse(HttpStatusCode code, string error)
    {
        var errorResponse = new ErrorResponse
        {
            StatusCode = code,
            Timestamp = DateTime.UtcNow
        };

        errorResponse.ErrorMessages.Add(error);

        return code switch
        {
            HttpStatusCode.NotFound => NotFound(errorResponse),
            HttpStatusCode.Unauthorized => Unauthorized(errorResponse),
            _ => BadRequest(errorResponse)
        };
    }
}