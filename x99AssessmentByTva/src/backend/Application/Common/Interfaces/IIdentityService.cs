using x99AssessmentByTva.Application.Common.Models;

namespace x99AssessmentByTva.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<string?> GetUserNameAsync(string userId);

    Task<bool> IsInRoleAsync(
        string userId,
        string role);

    Task<(Result Result, string UserId)> CreateUserAsync(
        string email,
        string password,
        string displayName);

    Task<(AuthenticationStatus Status, LoginResult? AuthResult)> AuthenticateAsync(
        string email,
        string password);
}

public enum AuthenticationStatus
{
    UserNotFound = 1,
    InvalidCredentials = 2,
    Success = 3
}

public sealed record LoginResult(
    string UserId,
    string Email,
    string? DisplayName,
    IReadOnlyList<string> Roles);
