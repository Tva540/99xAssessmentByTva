using x99AssessmentByTva.Application.Auth.Commands.Login;
using x99AssessmentByTva.Application.Common.Interfaces;
using x99AssessmentByTva.Application.Common.Models;

namespace x99AssessmentByTva.Application.UnitTests.Auth.Commands;

public sealed class LoginCommandHandlerTests
{
    [Fact]
    public async Task Handle_OnSuccess_ReturnsSignedTokenForUser()
    {
        var identity = new FakeIdentityService
        {
            AuthResult = (AuthenticationStatus.Success, new LoginResult(
                "user-1",
                "admin@jondell.local",
                "Admin User",
                ["Admin"]))
        };
        var token = new FakeTokenService("signed-token", DateTime.UtcNow.AddHours(1));
        var handler = new LoginCommandHandler(identity, token);

        var response = await handler.Handle(
            new LoginCommand("admin@jondell.local", "Admin@12345"),
            default);

        Assert.Equal("signed-token", response.Token);
        Assert.Equal("admin@jondell.local", response.Email);
        Assert.Equal("Admin User", response.DisplayName);
        Assert.Contains("Admin", response.Roles);
        Assert.Equal("admin@jondell.local", identity.LastEmail);
        Assert.Equal("user-1", token.LastUserId);
    }

    [Theory]
    [InlineData(AuthenticationStatus.UserNotFound)]
    [InlineData(AuthenticationStatus.InvalidCredentials)]
    public async Task Handle_WhenAuthFails_ThrowsUnauthorized(AuthenticationStatus status)
    {
        var identity = new FakeIdentityService { AuthResult = (status, null) };
        var handler = new LoginCommandHandler(identity, new FakeTokenService());

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            handler.Handle(new LoginCommand("x@x.com", "nope"), default));
    }

    #region Fake Service
    private sealed class FakeIdentityService : IIdentityService
    {
        public (AuthenticationStatus Status, LoginResult? Result) AuthResult { get; set; }
            = (AuthenticationStatus.UserNotFound, null);
        public string? LastEmail { get; private set; }

        public Task<(AuthenticationStatus Status, LoginResult? AuthResult)> AuthenticateAsync(string email, string password)
        {
            LastEmail = email;
            return Task.FromResult(AuthResult);
        }

        public Task<(Result Result, string UserId)> CreateUserAsync(string email, string password, string displayName)
            => throw new NotImplementedException();
        public Task<string?> GetUserNameAsync(string userId) => throw new NotImplementedException();
        public Task<bool> IsInRoleAsync(string userId, string role) => throw new NotImplementedException();
    }

    private sealed class FakeTokenService(
        string token = "token",
        DateTime? expires = null) : ITokenService
    {
        public string? LastUserId { get; private set; }

        public (string Token, DateTime ExpiresAt) CreateToken(
            string userId, string email, string? displayName, IEnumerable<string> roles)
        {
            LastUserId = userId;
            return (token, expires ?? DateTime.UtcNow.AddHours(1));
        }
    }
    #endregion
}
