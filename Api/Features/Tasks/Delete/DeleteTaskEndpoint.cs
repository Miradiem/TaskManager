using Api.Common.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Api.Features.Tasks.Delete;

public static class DeleteTaskEndpoint
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

        context.Tasks.Remove(task);
        await context.SaveChangesAsync();

        return Results.NoContent();
    }
}