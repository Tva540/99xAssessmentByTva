namespace x99AssessmentByTva.Application.Common.Interfaces;

public interface ITokenService
{
    (string Token, DateTime ExpiresAt) CreateToken(
        string userId,
        string email,
        string? displayName,
        IEnumerable<string> roles);
}
