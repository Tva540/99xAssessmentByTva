using System.Reflection;
using x99AssessmentByTva.Application.Common.Behaviours;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace x99AssessmentByTva.Application;

public static class ApplicationDependencyInjectionHelper
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddOpenBehavior(typeof(UnhandledExceptionBehaviour<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehaviour<,>));
        });
    }
}
