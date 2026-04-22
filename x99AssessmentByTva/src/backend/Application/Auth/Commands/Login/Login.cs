using x99AssessmentByTva.Application.Common.Interfaces;

namespace x99AssessmentByTva.Application.Auth.Commands.Login;

#region Request and Response models
public sealed record LoginCommand(
    string Email,
    string Password) : IRequest<LoginResponseDto>;

public sealed record LoginResponseDto(
    string Token,
    DateTime ExpiresAt,
    string Email,
    string DisplayName,
    IReadOnlyList<string> Roles);
#endregion

public sealed class LoginCommandHandler(
    IIdentityService identityService,
    ITokenService tokenService) : IRequestHandler<LoginCommand, LoginResponseDto>
{
    public async Task<LoginResponseDto> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var (status, authResult) = await identityService.AuthenticateAsync(
            request.Email,
            request.Password);

        // INFO: Return the same error for both cases to prevent user enumeration attacks
        if (status is AuthenticationStatus.UserNotFound or 
                      AuthenticationStatus.InvalidCredentials)
            throw new UnauthorizedAccessException("Invalid credentials");

        var (token, expiresAt) = tokenService.CreateToken(
            authResult!.UserId,
            authResult.Email,
            authResult.DisplayName,
            authResult.Roles);

        return new LoginResponseDto(
            token,
            expiresAt,
            authResult.Email,
            authResult.DisplayName ?? string.Empty,
            authResult.Roles);
    }
}
