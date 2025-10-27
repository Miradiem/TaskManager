using Api.Features.Auth.Login;
using Api.Features.Auth.Register;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace TaskManager.Tests.IntegrationTests;

public class AuthIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_ShouldReturnCreated_WhenValidRequest()
    {
        var request = new RegisterRequest(
            $"test{Guid.NewGuid()}@example.com",
            "Password123!",
            "Test",
            "User"
        );

        var response = await _client.PostAsJsonAsync("/auth/register", request);

        response.StatusCode.Should().BeOneOf(HttpStatusCode.Created, HttpStatusCode.OK);
    }

    [Fact]
    public async Task Login_ShouldReturnOk_WhenValidCredentials()
    {
        var registerRequest = new RegisterRequest(
            $"login{Guid.NewGuid()}@example.com",
            "Password123!",
            "Login",
            "User"
        );
        await _client.PostAsJsonAsync("/auth/register", registerRequest);

        var loginRequest = new LoginRequest(
            registerRequest.Email,
            registerRequest.Password
        );

        var response = await _client.PostAsJsonAsync("/auth/login", loginRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
        loginResponse.Should().NotBeNull();
        loginResponse!.Token.Should().NotBeNullOrEmpty();
    }
}