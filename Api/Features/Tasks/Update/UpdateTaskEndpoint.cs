using Api.Common.Persistence;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Api.Features.Tasks.Update;

public static class UpdateTaskEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        UpdateTaskRequest request,
        AppDbContext context,
        IValidator<UpdateTaskRequest> validator,
        ClaimsPrincipal user)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(new { errors = validationResult.Errors.Select(e => e.ErrorMessage) });
        }

        var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var task = await context.Tasks
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

        if (task is null)
        {
            return Results.NotFound();
        }

        task.Title = request.Title;
        task.Description = request.Description;
        task.Status = request.Status;
        task.DueDate = request.DueDate;
        task.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();

        var response = new UpdateTaskResponse(
            task.Id,
            task.Title,
            task.Description,
            task.Status,
            task.DueDate,
            task.UpdatedAt
        );

        return Results.Ok(response);
    }
}