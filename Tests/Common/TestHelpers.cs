using Api.Common.Jwt;

namespace TaskManager.Tests.Common;

public static class TestHelpers
{
    public static JwtSettings CreateTestJwtSettings()
    {
        return new JwtSettings
        {
            SecretKey = "super-secret-key-for-testing-purposes-only",
            Issuer = "TaskManagerTest",
            Audience = "TaskManagerTest",
            ExpirationMinutes = 30
        };
    }

    public static JwtService CreateJwtService()
    {
        return new JwtService(CreateTestJwtSettings());
    }
}