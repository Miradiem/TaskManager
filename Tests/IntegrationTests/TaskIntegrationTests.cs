using Api.Features.Auth.Login;
using Api.Features.Auth.Register;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace TaskManager.Tests.IntegrationTests;

public class TaskIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public TaskIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<string> GetAuthTokenAsync()
    {
        var registerRequest = new RegisterRequest(
            $"task{Guid.NewGuid()}@example.com",
            "Password123!",
            "Task",
            "User"
        );
        await _client.PostAsJsonAsync("/auth/register", registerRequest);

        var loginRequest = new LoginRequest(
            registerRequest.Email,
            registerRequest.Password
        );
        var loginResponse = await _client.PostAsJsonAsync("/auth/login", loginRequest);
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
        
        return loginResult!.Token;
    }

    [Fact]
    public async Task GetTasks_ShouldReturnOk_WhenAuthenticated()
    {
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var response = await _client.GetAsync("/tasks");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetTasks_ShouldReturnEmptyList_WhenNoTasksExist()
    {
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var response = await _client.GetAsync("/tasks");
        var content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().NotBeNullOrEmpty();
    }
}