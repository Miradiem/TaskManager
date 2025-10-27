using Api.Common.Middleware;
using Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddSerilog();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Task Manager API",
        Version = "v1",
        Description = "A clean, modern .NET 8 REST API for task management with JWT authentication"
    });

    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddValidation();
builder.Services.AddCorsPolicy(builder.Configuration);

var app = builder.Build();

app.Urls.Add("https://localhost:7000");
app.Urls.Add("http://localhost:5000");

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Task Manager API v1");
    c.RoutePrefix = "swagger";
    c.DocumentTitle = "Task Manager API";
    c.DefaultModelsExpandDepth(-1);
    c.DisplayRequestDuration();
});

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseCors("FrontendPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapApiEndpoints();

app.MapFallback(async context =>
{
    if (context.Request.Method == "OPTIONS")
    {
        var origin = context.Request.Headers.Origin.FirstOrDefault();
        var allowedOrigins = new[] { "http://localhost:4200", "https://localhost:4200", "http://localhost:5000", "https://localhost:7000" };
        
        if (allowedOrigins.Contains(origin))
        {
            context.Response.Headers["Access-Control-Allow-Origin"] = origin;
        }
        
        context.Response.Headers["Access-Control-Allow-Methods"] = "GET, POST, PUT, DELETE, OPTIONS";
        context.Response.Headers["Access-Control-Allow-Headers"] = "Content-Type, Authorization";
        context.Response.Headers["Access-Control-Allow-Credentials"] = "true";
        context.Response.Headers["Access-Control-Max-Age"] = "86400";
        
        context.Response.StatusCode = 200;
        await context.Response.WriteAsync("");
    }
});

await app.Services.EnsureDatabaseCreatedAsync();

app.Run();

public partial class Program { }