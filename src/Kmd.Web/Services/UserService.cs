using Kmd.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Kmd.Web.Services;

public class UserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly CachedUserStore _cache;

    public UserService(UserManager<ApplicationUser> userManager, CachedUserStore cache)
    {
        _userManager = userManager;
        _cache = cache;
    }

    public async Task<List<ApplicationUser>> GetAllMembersAsync()
    {
        var members = await _userManager.GetUsersInRoleAsync("Member");
        return members.OrderBy(u => u.Email).ToList();
    }

    public async Task<ApplicationUser?> GetByIdAsync(string id) =>
        await _userManager.FindByIdAsync(id);

    public async Task<(bool Success, string[] Errors)> CreateMemberAsync(string email, string? className, string? otherContact, string password)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            ClassName = className,
            OtherContact = otherContact
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
            return (false, result.Errors.Select(e => e.Description).ToArray());

        await _userManager.AddToRoleAsync(user, "Member");
        _cache.Set(user);
        return (true, []);
    }

    public async Task<(bool Success, string[] Errors)> UpdateMemberAsync(string id, string email, string? className, string? otherContact)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return (false, ["User not found."]);

        _cache.Invalidate(user);

        user.Email = email;
        user.UserName = email;
        user.ClassName = className;
        user.OtherContact = otherContact;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return (false, result.Errors.Select(e => e.Description).ToArray());

        _cache.Set(user);
        return (true, []);
    }

    public async Task<(bool Success, string[] Errors)> ResetPasswordAsync(string id, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return (false, ["User not found."]);

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
        if (!result.Succeeded)
            return (false, result.Errors.Select(e => e.Description).ToArray());

        _cache.Invalidate(user);
        return (true, []);
    }

    public async Task<(bool Success, string[] Errors)> SetActiveStatusAsync(string id, bool active)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return (false, ["User not found."]);

        user.LockoutEnd = active ? null : DateTimeOffset.MaxValue;
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return (false, result.Errors.Select(e => e.Description).ToArray());

        _cache.Invalidate(user);
        return (true, []);
    }

    public bool IsActive(ApplicationUser user) =>
        user.LockoutEnd == null || user.LockoutEnd <= DateTimeOffset.UtcNow;
}
