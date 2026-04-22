using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using x99AssessmentByTva.Application.Common.Exceptions;

namespace x99AssessmentByTva.Server.Infrastructure;

public sealed class ProblemDetailsExceptionHandler(
    IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var problemDetails = GetProblemDetails(exception);
        if (problemDetails is null)
            return false;

        httpContext.Response.StatusCode = problemDetails.Status!.Value;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = problemDetails
        });
    }

    private static ProblemDetails? GetProblemDetails(Exception exception)
    {
        return exception switch
        {
            ValidationException ve => new ValidationProblemDetails(ve.Errors)
            {
                Status = StatusCodes.Status400BadRequest,
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1"
            },
            BadHttpRequestException bad => new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Bad request",
                Detail = bad.Message,
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1"
            },
            ArgumentException arg => new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Invalid input",
                Detail = arg.Message,
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1"
            },
            NotSupportedException ns => new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Unsupported operation",
                Detail = ns.Message,
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1"
            },
            UnauthorizedAccessException ua => new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Unauthorized",
                Detail = ua.Message,
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.2"
            },
            ForbiddenAccessException fa => new ProblemDetails
            {
                Status = StatusCodes.Status403Forbidden,
                Title = "Forbidden",
                Detail = fa.Message,
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.4"
            },
            _ => null
        };
    }
}
