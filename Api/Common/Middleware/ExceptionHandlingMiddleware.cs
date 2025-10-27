using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace Api.Common.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var errorResponse = exception switch
        {
            ValidationException validationEx => new ErrorResponse(
                "ValidationError",
                "One or more validation errors occurred.",
                validationEx.Errors.Select(e => e.ErrorMessage).ToList(),
                (int)HttpStatusCode.BadRequest
            ),
            DbUpdateException dbEx => new ErrorResponse(
                "DatabaseError",
                "A database error occurred while processing your request.",
                null,
                (int)HttpStatusCode.InternalServerError
            ),
            UnauthorizedAccessException => new ErrorResponse(
                "Unauthorized",
                "You are not authorized to perform this action.",
                null,
                (int)HttpStatusCode.Unauthorized
            ),
            ArgumentException argEx => new ErrorResponse(
                "InvalidArgument",
                argEx.Message,
                null,
                (int)HttpStatusCode.BadRequest
            ),
            _ => new ErrorResponse(
                "InternalServerError",
                "An unexpected error occurred. Please try again later.",
                null,
                (int)HttpStatusCode.InternalServerError
            )
        };

        response.StatusCode = errorResponse.StatusCode;

        var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await response.WriteAsync(jsonResponse);
    }
}

public record ErrorResponse(
    string Error,
    string Message,
    List<string>? Details = null,
    int StatusCode = 500
);

public class ValidationException : Exception
{
    public List<ValidationError> Errors { get; }

    public ValidationException(List<ValidationError> errors) 
        : base("Validation failed")
    {
        Errors = errors;
    }
}

public record ValidationError(string PropertyName, string ErrorMessage);