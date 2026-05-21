using application.Configuration;
using infrastructure.Configurations;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Logging.AddConsole(options =>
{
    options.FormatterName = "json";
});

builder.Services.AddControllers();
builder.Services.AddApplication();
builder.Services.AddInfrastructure();

var infoApi = new OpenApiInfo
{
    Version = "v1",
    Title = "SRP Example API",
    Description = "API demonstrating the Single Responsibility Principle",
    Contact = new()
    {
        Name = "",
        Email = "",
        Url = new Uri(uriString: "https://www.linkedin.com/in/fernandes-marcelo/")
    },
    License = new()
    {
        Name = "MIT",
        Url = new Uri(uriString: "https://opensorce.org/licenses/MIT")
    }
};

builder.Services.AddSwaggerGen(g =>
{
    g.SwaggerDoc("v1", infoApi);
    g.CustomSchemaIds(type => type.FullName);
});

var app = builder.Build();

app.InitializeDatabase();

app.UseSwagger();          
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SRP Example API v1");
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
