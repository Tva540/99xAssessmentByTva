using Microsoft.AspNetCore.Identity;

namespace x99AssessmentByTva.Infrastructure.Identity;

public sealed class ApplicationUser : IdentityUser
{
    public string? DisplayName { get; set; }
}
