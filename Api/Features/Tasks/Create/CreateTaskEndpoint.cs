using Api.Common.Persistence;
using FluentValidation;
using System.Security.Claims;

namespace Api.Features.Tasks.Create;

public static class CreateTaskEndpoint
{
    public static async Task<IResult> Handle(
        CreateTaskRequest request,
        AppDbContext context,
        IValidator<CreateTaskRequest> validator,
        ClaimsPrincipal user)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(new { errors = validationResult.Errors.Select(e => e.ErrorMessage) });
        }

        var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var task = new Api.Features.Tasks.Common.TaskItem
        {
            Title = request.Title,
            Description = request.Description,
            DueDate = request.DueDate,
            UserId = userId
        };

        context.Tasks.Add(task);
        await context.SaveChangesAsync();

        var response = new CreateTaskResponse(
            task.Id,
            task.Title,
            task.Description,
            task.Status,
            task.DueDate,
            task.CreatedAt
        );

        return Results.Created($"/tasks/{task.Id}", response);
    }
}