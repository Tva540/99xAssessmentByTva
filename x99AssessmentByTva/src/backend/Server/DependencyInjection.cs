using x99AssessmentByTva.Application.Common.Interfaces;
using x99AssessmentByTva.Server.Infrastructure;
using x99AssessmentByTva.Server.Services;

namespace Microsoft.Extensions.DependencyInjection;

public static class WebDependencyInjection
{
    private static readonly string[] DefaultCorsOrigins =
    [
        "https://localhost:50810",
        "http://localhost:50810"
    ];

    public static void AddWebServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<IUser, CurrentUser>();

        builder.Services.AddExceptionHandler<ProblemDetailsExceptionHandler>();
        builder.Services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = ctx =>
            {
                ctx.ProblemDetails.Instance ??=
                    $"{ctx.HttpContext.Request.Method} {ctx.HttpContext.Request.Path}";
                ctx.ProblemDetails.Extensions.TryAdd(
                    "traceId", ctx.HttpContext.TraceIdentifier);
            };
        });

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddOpenApi();

        var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                      ?? DefaultCorsOrigins;

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(p => p
                .WithOrigins(origins)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials());
        });
    }
}
