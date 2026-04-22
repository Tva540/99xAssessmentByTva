using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using x99AssessmentByTva.Application.Common.Interfaces;
using x99AssessmentByTva.Infrastructure.Data;
using x99AssessmentByTva.Infrastructure.Data.Interceptors;
using x99AssessmentByTva.Infrastructure.Parsers;
using x99AssessmentByTva.Infrastructure.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace x99AssessmentByTva.Infrastructure;

public static class InfrastructureDependencyInjectionHelper
{
    public static void AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

        builder.Services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();

        builder.Services.AddDbContext<ApplicationDbContext>((
            sp,
            options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseNpgsql(
                connectionString,
                npgsql => npgsql.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorCodesToAdd: null));
            options.ConfigureWarnings(w =>
                w.Ignore(RelationalEventId.PendingModelChangesWarning));
        });

        builder.Services.AddScoped<IApplicationDbContext>(sp =>
            sp.GetRequiredService<ApplicationDbContext>());

        builder.Services.AddScoped<ApplicationDbContextInitialiser>();

        builder.Services
            .AddIdentityCore<ApplicationUser>(opts =>
            {
                opts.Password.RequireDigit = true;
                opts.Password.RequireUppercase = true;
                opts.Password.RequireNonAlphanumeric = true;
                opts.Password.RequiredLength = 8;
                opts.User.RequireUniqueEmail = true;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        builder.Services.AddSingleton(TimeProvider.System);
        builder.Services.AddTransient<IIdentityService, IdentityService>();
        builder.Services.AddTransient<ITokenService, TokenService>();

        builder.Services.AddSingleton<IBalanceFileParser, ExcelBalanceParser>();
        builder.Services.AddSingleton<IBalanceFileParser, TsvBalanceParser>();
        builder.Services.AddSingleton<IBalanceFileParserFactory, BalanceFileParserFactory>();
    }
}
