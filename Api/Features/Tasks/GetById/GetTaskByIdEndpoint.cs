using Api.Common.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Api.Features.Tasks.GetById;

public static class GetTaskByIdEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        AppDbContext context,
        ClaimsPrincipal user)
    {
        var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var task = await context.Tasks
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

        if (task is null)
        {
            return Results.NotFound();
        }

        var response = new GetTaskByIdResponse(
            task.Id,
            task.Title,
            task.Description,
            task.Status,
            task.DueDate,
            task.CreatedAt,
            task.UpdatedAt
        );

        return Results.Ok(response);
    }
}