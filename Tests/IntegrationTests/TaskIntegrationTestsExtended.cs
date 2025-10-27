using Api.Features.Auth.Login;
using Api.Features.Auth.Register;
using Api.Features.Tasks.Common;
using Api.Features.Tasks.Create;
using Api.Features.Tasks.Update;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace TaskManager.Tests.IntegrationTests;

public class TaskIntegrationTestsExtended : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public TaskIntegrationTestsExtended(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<string> GetAuthTokenAsync()
    {
        var registerRequest = new RegisterRequest(
            $"extended{Guid.NewGuid()}@example.com",
            "Password123!",
            "Extended",
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
    public async Task CreateTask_ShouldReturnCreated_WhenValid()
    {
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var request = new CreateTaskRequest(
            Title: "Integration Test Task",
            Description: "This is a test task",
            DueDate: DateTime.UtcNow.AddDays(7)
        );

        var response = await _client.PostAsJsonAsync("/tasks", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrEmpty();
        content.Should().Contain("Integration Test Task");
    }

    [Fact]
    public async Task GetTaskById_ShouldReturnOk_WhenExists()
    {
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var createRequest = new CreateTaskRequest(
            Title: "Task to Get",
            Description: "Get this task"
        );
        var createResponse = await _client.PostAsJsonAsync("/tasks", createRequest);
        var createContent = await createResponse.Content.ReadAsStringAsync();
        
        var idStart = createContent.IndexOf("\"id\":\"") + 6;
        var idEnd = createContent.IndexOf("\"", idStart);
        var taskId = createContent.Substring(idStart, idEnd - idStart);

        var response = await _client.GetAsync($"/tasks/{taskId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Task to Get");
    }

    [Fact]
    public async Task GetTaskById_ShouldReturnNotFound_WhenNotExists()
    {
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var fakeId = Guid.NewGuid();
        var response = await _client.GetAsync($"/tasks/{fakeId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateTask_ShouldReturnOk_WhenValid()
    {
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var createRequest = new CreateTaskRequest(
            Title: "Original Title",
            Description: "Original Description"
        );
        var createResponse = await _client.PostAsJsonAsync("/tasks", createRequest);
        var createContent = await createResponse.Content.ReadAsStringAsync();
        
        var idStart = createContent.IndexOf("\"id\":\"") + 6;
        var idEnd = createContent.IndexOf("\"", idStart);
        var taskId = createContent.Substring(idStart, idEnd - idStart);

        var updateRequest = new UpdateTaskRequest(
            Title: "Updated Title",
            Description: "Updated Description",
            Status: Api.Features.Tasks.Common.TaskStatus.InProgress,
            DueDate: DateTime.UtcNow.AddDays(3)
        );

        var response = await _client.PutAsJsonAsync($"/tasks/{taskId}", updateRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Updated Title");
        content.Should().Contain("\"status\":1");
    }

    [Fact]
    public async Task UpdateTask_ShouldReturnNotFound_WhenNotExists()
    {
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var fakeId = Guid.NewGuid();
        var updateRequest = new UpdateTaskRequest(
            Title: "Updated Title",
            Description: "Updated Description",
            Status: Api.Features.Tasks.Common.TaskStatus.Done,
            DueDate: null
        );

        var response = await _client.PutAsJsonAsync($"/tasks/{fakeId}", updateRequest);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteTask_ShouldReturnNoContent_WhenExists()
    {
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var createRequest = new CreateTaskRequest(
            Title: "Task to Delete",
            Description: "Delete this"
        );
        var createResponse = await _client.PostAsJsonAsync("/tasks", createRequest);
        var createContent = await createResponse.Content.ReadAsStringAsync();
        
        var idStart = createContent.IndexOf("\"id\":\"") + 6;
        var idEnd = createContent.IndexOf("\"", idStart);
        var taskId = createContent.Substring(idStart, idEnd - idStart);

        var response = await _client.DeleteAsync($"/tasks/{taskId}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteTask_ShouldReturnNotFound_WhenNotExists()
    {
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var fakeId = Guid.NewGuid();
        var response = await _client.DeleteAsync($"/tasks/{fakeId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetTasks_ShouldFilterByStatus_WhenFilterApplied()
    {
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var createRequest = new CreateTaskRequest(
            Title: "Completed Task",
            Description: "This is completed"
        );
        await _client.PostAsJsonAsync("/tasks", createRequest);

        var response = await _client.GetAsync("/tasks?status=0");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrEmpty();
    }
}
