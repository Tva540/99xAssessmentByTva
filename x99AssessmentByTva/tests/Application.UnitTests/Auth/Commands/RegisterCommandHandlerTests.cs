using x99AssessmentByTva.Application.Auth.Commands.Register;
using x99AssessmentByTva.Application.Common.Exceptions;
using x99AssessmentByTva.Application.Common.Interfaces;
using x99AssessmentByTva.Application.Common.Models;

namespace x99AssessmentByTva.Application.UnitTests.Auth.Commands;

public sealed class RegisterCommandHandlerTests
{
    [Fact]
    public async Task Handle_OnSuccess_ReturnsResponse()
    {
        var identity = new FakeIdentityService
        {
            CreateResult = (Result.Success(), "user-1")
        };
        var handler = new RegisterCommandHandler(identity);

        var response = await handler.Handle(
            new RegisterCommand("new@jondell.local", "Passw0rd!", "New User"),
            default);

        Assert.Equal("user-1", response.UserId);
        Assert.Equal("new@jondell.local", response.Email);
        Assert.Equal("New User", response.DisplayName);
    }

    [Fact]
    public async Task Handle_WhenCreateFails_ThrowsValidationExceptionWithErrors()
    {
        var identity = new FakeIdentityService
        {
            CreateResult = (Result.Failure(["Email already taken", "Password too weak"]), string.Empty)
        };
        var handler = new RegisterCommandHandler(identity);

        var ex = await Assert.ThrowsAsync<ValidationException>(() =>
            handler.Handle(new RegisterCommand("dup@jondell.local", "weak", "User"), default));

        Assert.Contains("Registration", ex.Errors.Keys);
        Assert.Contains("Email already taken", ex.Errors["Registration"]);
    }

    #region Fake Service
    private sealed class FakeIdentityService : IIdentityService
    {
        public (Result Result, string UserId) CreateResult { get; set; } = (Result.Success(), "user-1");

        public Task<(Result Result, string UserId)> CreateUserAsync(string email, string password, string displayName)
            => Task.FromResult(CreateResult);

        public Task<(AuthenticationStatus Status, LoginResult? AuthResult)> AuthenticateAsync(string email, string password)
            => throw new NotImplementedException();
        public Task<string?> GetUserNameAsync(string userId) => throw new NotImplementedException();
        public Task<bool> IsInRoleAsync(string userId, string role) => throw new NotImplementedException();
    }
    #endregion
}
