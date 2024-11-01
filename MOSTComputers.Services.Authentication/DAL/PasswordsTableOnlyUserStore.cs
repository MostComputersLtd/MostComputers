using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MOSTComputers.Services.Identity.Models;

namespace MOSTComputers.Services.Identity.DAL;

internal class PasswordsTableOnlyUserStore : IUserStore<PasswordsTableOnlyUser>, IUserPasswordStore<PasswordsTableOnlyUser>
{
    private readonly PasswordsTableOnlyAuthenticationDBContext _context;

    public PasswordsTableOnlyUserStore(PasswordsTableOnlyAuthenticationDBContext context)
    {
        _context = context;
    }

    public async Task<IdentityResult> CreateAsync(PasswordsTableOnlyUser user, CancellationToken cancellationToken)
    {
        _context.Users.Add(user);

        await _context.SaveChangesAsync(cancellationToken);

        return IdentityResult.Success;
    }

    public async Task<IdentityResult> DeleteAsync(PasswordsTableOnlyUser user, CancellationToken cancellationToken)
    {
        _context.Users.Remove(user);
        await _context.SaveChangesAsync(cancellationToken);
        return IdentityResult.Success;
    }

    public async Task<PasswordsTableOnlyUser?> FindByIdAsync(string userId, CancellationToken cancellationToken)
    {
        return await _context.Users.FindAsync(new object[] { userId }, cancellationToken);
    }

    public async Task<PasswordsTableOnlyUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
        return await _context.Users.SingleOrDefaultAsync(u => u.NormalizedUserName == normalizedUserName, cancellationToken);
    }

    public async Task<IdentityResult> UpdateAsync(PasswordsTableOnlyUser user, CancellationToken cancellationToken)
    {
        _context.Users.Update(user);

        await _context.SaveChangesAsync(cancellationToken);

        return IdentityResult.Success;
    }

    public Task<string> GetUserIdAsync(PasswordsTableOnlyUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.Id);
    }

    public Task<string?> GetUserNameAsync(PasswordsTableOnlyUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.UserName);
    }

    public Task SetUserNameAsync(PasswordsTableOnlyUser user, string userName, CancellationToken cancellationToken)
    {
        user.UserName = userName;

        return Task.CompletedTask;
    }

    public Task<string?> GetNormalizedUserNameAsync(PasswordsTableOnlyUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.NormalizedUserName);
    }

    public Task SetNormalizedUserNameAsync(PasswordsTableOnlyUser user, string? normalizedName, CancellationToken cancellationToken)
    {
        user.NormalizedUserName = normalizedName;

        return Task.CompletedTask;
    }

    public Task SetPasswordHashAsync(PasswordsTableOnlyUser user, string? passwordHash, CancellationToken cancellationToken)
    {
        user.PasswordHash = passwordHash;

        return Task.CompletedTask;
    }

    public Task<string?> GetPasswordHashAsync(PasswordsTableOnlyUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.PasswordHash);
    }

    public Task<bool> HasPasswordAsync(PasswordsTableOnlyUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.PasswordHash != null);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}