using Microsoft.Extensions.Logging;

namespace x99AssessmentByTva.Application.Common.Behaviours;

public sealed class UnhandledExceptionBehaviour<TRequest, TResponse>(
    ILogger<TRequest> logger) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        try
        {
            return await next(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unhandled Exception for Request {Name} {@Request}",
                typeof(TRequest).Name,
                request);

            throw;
        }
    }
}
