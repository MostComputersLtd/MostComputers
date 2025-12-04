using Microsoft.AspNetCore.Identity;
using MOSTComputers.Services.Identity.Models;

namespace MOSTComputers.Services.Identity.DAL;
internal sealed class PasswordsTableOnlyRoleToUserRelationshipStore : IUserRoleStore<PasswordsTableOnlyUser>
{
    public PasswordsTableOnlyRoleToUserRelationshipStore(PasswordsTableOnlyAuthenticationDBContext dBContext)
    {
        _dBContext = dBContext;
    }

    private readonly PasswordsTableOnlyAuthenticationDBContext _dBContext;

    private bool _disposedValue;

    public Task AddToRoleAsync(PasswordsTableOnlyUser user, string roleName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IdentityResult> CreateAsync(PasswordsTableOnlyUser user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IdentityResult> DeleteAsync(PasswordsTableOnlyUser user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<PasswordsTableOnlyUser?> FindByIdAsync(string userId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<PasswordsTableOnlyUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<string?> GetNormalizedUserNameAsync(PasswordsTableOnlyUser user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IList<string>> GetRolesAsync(PasswordsTableOnlyUser user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<string> GetUserIdAsync(PasswordsTableOnlyUser user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<string?> GetUserNameAsync(PasswordsTableOnlyUser user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IList<PasswordsTableOnlyUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<bool> IsInRoleAsync(PasswordsTableOnlyUser user, string roleName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task RemoveFromRoleAsync(PasswordsTableOnlyUser user, string roleName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task SetNormalizedUserNameAsync(PasswordsTableOnlyUser user, string? normalizedName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task SetUserNameAsync(PasswordsTableOnlyUser user, string? userName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IdentityResult> UpdateAsync(PasswordsTableOnlyUser user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _dBContext.Dispose();
            }

            _disposedValue = true;
        }
    }
    public void Dispose()
    {
        Dispose(disposing: true);
    }
}