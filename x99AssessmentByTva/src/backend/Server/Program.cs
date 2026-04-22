using Azure.Identity;
using Scalar.AspNetCore;
using x99AssessmentByTva.Application;
using x99AssessmentByTva.Infrastructure;
using x99AssessmentByTva.Infrastructure.Data;
using x99AssessmentByTva.Server;

var builder = WebApplication.CreateBuilder(args);

var keyVaultUri = builder.Configuration["VaultUri"];

//INFO: Use Azure Key Vault for Production Env
if (!string.IsNullOrWhiteSpace(keyVaultUri) && builder.Environment.IsProduction())
{
    builder.Configuration.AddAzureKeyVault(
        new Uri(keyVaultUri),
        new DefaultAzureCredential());
}

builder.Services.AddApplicationInsightsTelemetry();

builder.AddApplicationServices();
builder.AddInfrastructureServices();
builder.AddWebServices();

var app = builder.Build();

await app.InitialiseDatabaseAsync();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("99x Assessment API")
           .WithTheme(ScalarTheme.BluePlanet)
           .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

app.UseDefaultFiles();
app.MapStaticAssets();

app.UseExceptionHandler(options => { });
app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
