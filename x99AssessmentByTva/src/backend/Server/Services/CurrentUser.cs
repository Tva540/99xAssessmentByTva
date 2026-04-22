using System.Security.Claims;
using x99AssessmentByTva.Application.Common.Interfaces;

namespace x99AssessmentByTva.Server.Services;

public sealed class CurrentUser(
    IHttpContextAccessor httpContextAccessor) : IUser
{
    public string? Id => httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
}
