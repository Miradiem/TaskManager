using FluentAssertions;
using System.Net;

namespace TaskManager.Tests.IntegrationTests;

public class ApiIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ApiIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetTasks_ShouldReturnUnauthorized_WhenNoToken()
    {
        var response = await _client.GetAsync("/tasks");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateTask_ShouldReturnUnauthorized_WhenNoToken()
    {
        var response = await _client.PostAsync("/tasks", null);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}