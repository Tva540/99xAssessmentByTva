using x99AssessmentByTva.Application.Common.Interfaces;

namespace x99AssessmentByTva.Application.Auth.Commands.Register;

#region Request and Response models
public sealed record RegisterCommand(
    string Email,
    string Password,
    string DisplayName) : IRequest<RegisterResponseDto>;

public sealed record RegisterResponseDto(
    string UserId,
    string Email,
    string DisplayName);
#endregion

public sealed class RegisterCommandHandler(
    IIdentityService identityService) : IRequestHandler<RegisterCommand, RegisterResponseDto>
{
    public async Task<RegisterResponseDto> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        var (result, userId) = await identityService.CreateUserAsync(
            request.Email,
            request.Password,
            request.DisplayName);

        if (!result.Succeeded)
        {
            throw new Common.Exceptions.ValidationException
            (
                result.Errors
                    .Select(e => new FluentValidation.Results.ValidationFailure("Registration", e))
            );
        }

        return new RegisterResponseDto(
            userId,
            request.Email,
            request.DisplayName);
    }
}
