using Microsoft.EntityFrameworkCore;
using Api.Common.Persistence;

namespace Api.Extensions;

public static class DbContextExtensions
{
    public static async Task EnsureDatabaseCreatedAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await context.Database.EnsureCreatedAsync();
    }
}