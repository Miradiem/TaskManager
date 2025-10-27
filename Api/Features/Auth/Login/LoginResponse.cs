namespace Api.Features.Auth.Login;

public sealed record LoginResponse(
    string Token,
    Guid UserId,
    string Email,
    string FirstName,
    string LastName
);