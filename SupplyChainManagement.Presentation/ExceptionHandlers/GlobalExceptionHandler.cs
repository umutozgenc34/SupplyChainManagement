using Core.Application.Responses;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;

namespace SupplyChainManagement.Presentation.ExceptionHandlers;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var result = exception switch
        {
            ValidationException validationException => ServiceResult.Failure(
                "Validation failed.",
                validationException.Errors.Select(e => e.ErrorMessage).ToList()),
            _ => ServiceResult.Failure("An unexpected error occurred.", HttpStatusCode.InternalServerError)
        };

        if (result.Status == HttpStatusCode.InternalServerError)
            logger.LogError(exception, "Unhandled exception occurred");

        httpContext.Response.StatusCode = (int)result.Status;
        await httpContext.Response.WriteAsJsonAsync(result, cancellationToken);
        return true;
    }
}
