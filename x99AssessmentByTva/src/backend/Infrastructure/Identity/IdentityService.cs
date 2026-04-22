using Microsoft.AspNetCore.Identity;
using x99AssessmentByTva.Application.Common.Interfaces;
using x99AssessmentByTva.Application.Common.Models;

namespace x99AssessmentByTva.Infrastructure.Identity;

public sealed class IdentityService(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager) : IIdentityService
{
    public async Task<string?> GetUserNameAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        return user?.UserName;
    }

    public async Task<bool> IsInRoleAsync(
        string userId,
        string role)
    {
        var user = await userManager.FindByIdAsync(userId);
        return user != null && await userManager.IsInRoleAsync(
            user,
            role);
    }

    public async Task<(Result Result, string UserId)> CreateUserAsync(
        string email,
        string password,
        string displayName)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            DisplayName = displayName
        };

        var result = await userManager.CreateAsync(
            user,
            password);

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(
                user,
                Domain.Constants.Roles.Viewer);
        }

        return
        (
            result.Succeeded ? 
                Result.Success() :
                Result.Failure(result.Errors.Select(e => e.Description)),
            user.Id
        );
    }

    public async Task<(AuthenticationStatus Status, LoginResult? AuthResult)> AuthenticateAsync(
        string email,
        string password)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
            return (AuthenticationStatus.UserNotFound, null);

        var check = await signInManager.CheckPasswordSignInAsync(
            user,
            password,
            lockoutOnFailure: false);
        if (!check.Succeeded)
            return (AuthenticationStatus.InvalidCredentials, null);

        return
        (
            AuthenticationStatus.Success,
            new LoginResult
            (
                user.Id,
                user.Email ?? string.Empty,
                user.DisplayName,
                (await userManager.GetRolesAsync(user)).ToList()
            )
        );
    }
}
