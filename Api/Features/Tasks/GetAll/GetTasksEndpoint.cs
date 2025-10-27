using Api.Common.Persistence;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Api.Features.Tasks.GetAll;

public static class GetTasksEndpoint
{
    public static async Task<IResult> Handle(
        [AsParameters] GetTasksRequest request,
        AppDbContext context,
        IValidator<GetTasksRequest> validator,
        ClaimsPrincipal user)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(new { errors = validationResult.Errors.Select(e => e.ErrorMessage) });
        }

        var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var query = context.Tasks
            .Where(t => t.UserId == userId);

        if (request.Status.HasValue)
        {
            query = query.Where(t => t.Status == request.Status.Value);
        }

        if (request.DueDateFrom.HasValue)
        {
            query = query.Where(t => t.DueDate >= request.DueDateFrom.Value);
        }

        if (request.DueDateTo.HasValue)
        {
            query = query.Where(t => t.DueDate <= request.DueDateTo.Value);
        }

        var totalCount = await query.CountAsync();

        var tasks = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(t => new TaskDto(
                t.Id,
                t.Title,
                t.Description,
                t.Status,
                t.DueDate,
                t.CreatedAt,
                t.UpdatedAt
            ))
            .ToListAsync();

        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        var response = new GetTasksResponse(
            tasks,
            totalCount,
            request.Page,
            request.PageSize,
            totalPages
        );

        return Results.Ok(response);
    }
}