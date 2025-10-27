using Api.Features.Auth.Login;
using Api.Features.Auth.Register;
using Api.Features.Tasks.Create;
using Api.Features.Tasks.GetAll;
using Api.Features.Tasks.GetById;
using Api.Features.Tasks.Update;
using Api.Features.Tasks.Delete;

namespace Api.Extensions;

public static class EndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapApiEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/register", RegisterEndpoint.Handle);
        app.MapPost("/auth/login", LoginEndpoint.Handle);

        app.MapPost("/tasks", CreateTaskEndpoint.Handle).RequireAuthorization();
        app.MapGet("/tasks", GetTasksEndpoint.Handle).RequireAuthorization();
        app.MapGet("/tasks/{id:guid}", GetTaskByIdEndpoint.Handle).RequireAuthorization();
        app.MapPut("/tasks/{id:guid}", UpdateTaskEndpoint.Handle).RequireAuthorization();
        app.MapDelete("/tasks/{id:guid}", DeleteTaskEndpoint.Handle).RequireAuthorization();

        return app;
    }
}