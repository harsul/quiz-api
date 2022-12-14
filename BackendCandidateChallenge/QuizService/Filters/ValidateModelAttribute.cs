using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using QuizService.Contracts.Common;
using Microsoft.AspNetCore.Components.Web;
using System.Net;

namespace QuizService.Filters;

public class ValidateModelAttribute : ActionFilterAttribute
{
    //TODO add separate validation for the each ID in route to check if it is valid integer

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var validationError = new ErrorResponse
            {
                StatusCode = HttpStatusCode.BadRequest,
                Timestamp = DateTime.UtcNow
            };
            var errors = context.ModelState.AsEnumerable();

            foreach (var error in errors)
            {
                foreach (var inner in error.Value.Errors)
                {
                    validationError.ErrorMessages.Add(inner.ErrorMessage);
                }
            }

            context.Result = new BadRequestObjectResult(validationError);
        }
    }
}