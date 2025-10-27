using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Api.Common.Persistence;
using Microsoft.Data.Sqlite;

namespace TaskManager.Tests.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlite(connection);
            });
        });
    }
}