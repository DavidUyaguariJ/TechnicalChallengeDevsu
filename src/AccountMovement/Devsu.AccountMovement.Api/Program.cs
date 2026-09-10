using Devsu.AccountMovement.Api.ErrorHandling;
using Devsu.AccountMovement.Application.Services;
using Devsu.AccountMovement.Infrastructure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

LoadDotEnvFile(Path.Combine(builder.Environment.ContentRootPath, ".env"));

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddAccountMovementInfrastructure(builder.Configuration);
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IMovementService, MovementService>();
builder.Services.AddScoped<IReportService, ReportService>();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseExceptionHandler();
var urls = app.Configuration["ASPNETCORE_URLS"];
var httpsPorts = app.Configuration["ASPNETCORE_HTTPS_PORTS"];
var httpsEnabled = (urls?.Contains("https://", StringComparison.OrdinalIgnoreCase) ?? false)
                   || !string.IsNullOrWhiteSpace(httpsPorts);

if (httpsEnabled)
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();

app.Run();

static void LoadDotEnvFile(string path)
{
    if (!File.Exists(path))
    {
        return;
    }

    foreach (var line in File.ReadAllLines(path))
    {
        var trimmed = line.Trim();
        if (trimmed.Length == 0 || trimmed.StartsWith('#'))
        {
            continue;
        }

        var separatorIndex = trimmed.IndexOf('=');
        if (separatorIndex <= 0)
        {
            continue;
        }

        var key = trimmed[..separatorIndex].Trim();
        var value = trimmed[(separatorIndex + 1)..].Trim().Trim('"');
        if (Environment.GetEnvironmentVariable(key) is null)
        {
            Environment.SetEnvironmentVariable(key, value);
        }
    }
}
