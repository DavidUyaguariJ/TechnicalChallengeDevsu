using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
LoadDotEnvFile(Path.Combine(builder.Environment.ContentRootPath, ".env"));
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
	app.MapScalarApiReference();
}

app.UseHttpsRedirection();

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