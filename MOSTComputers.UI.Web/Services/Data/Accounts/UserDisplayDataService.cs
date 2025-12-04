using OneOf;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.Identity.Models;
using MOSTComputers.Services.Identity.Services;
using MOSTComputers.UI.Web.Models.Authentication;
using MOSTComputers.UI.Web.Services.Data.Accounts.Contracts;

namespace MOSTComputers.UI.Web.Services.Data.Accounts;
internal sealed class UserDisplayDataService : IUserDisplayDataService
{
    public UserDisplayDataService(IIdentityService<PasswordsTableOnlyUser, PasswordsTableOnlyRole> identityService)
    {
        _identityService = identityService;
    }

    private readonly IIdentityService<PasswordsTableOnlyUser, PasswordsTableOnlyRole> _identityService;

    public async Task<OneOf<List<UserDisplayData>, UnexpectedFailureResult>> GetAllUsersAsync()
    {
        List<PasswordsTableOnlyUser> users = await _identityService.GetAllUsersAsync();

        return await GetUserDisplayDataFromUsersAsync(users);
    }

    public async Task<OneOf<List<UserDisplayData>, UnexpectedFailureResult>> GetUsersAsync(List<string> userIds)
    {
        List<PasswordsTableOnlyUser> users = await _identityService.GetUsersAsync(userIds);

        return await GetUserDisplayDataFromUsersAsync(users);
    }

    public async Task<OneOf<UserDisplayData?, UnexpectedFailureResult>> GetUserAsync(string userId)
    {
        PasswordsTableOnlyUser? user = await _identityService.GetUserAsync(userId);

        if (user is null) return null;

        OneOf<UserDisplayData, UnexpectedFailureResult> getUserDataResult = await GetUserDisplayDataFromUserAsync(user);

        return getUserDataResult.Match<OneOf<UserDisplayData?, UnexpectedFailureResult>>(
            user => user,
            unexpectedFailureResult => unexpectedFailureResult);
    }

    private async Task<OneOf<List<UserDisplayData>, UnexpectedFailureResult>> GetUserDisplayDataFromUsersAsync(
        IEnumerable<PasswordsTableOnlyUser> users)
    {
        List<UserDisplayData> output = new();

        foreach (PasswordsTableOnlyUser user in users)
        {
            OneOf<UserDisplayData, UnexpectedFailureResult> getUserDataResult = await GetUserDisplayDataFromUserAsync(user);

            if (getUserDataResult.IsT1)
            {
                return getUserDataResult.AsT1;
            }

            output.Add(getUserDataResult.AsT0);
        }

        return output;
    }

    public async Task<OneOf<UserDisplayData, UnexpectedFailureResult>> GetUserDisplayDataFromUserAsync(PasswordsTableOnlyUser user)
    {
        List<UserRoles>? userRoles = new();

        IList<string> roleNames = await _identityService.GetUserRoleNamesAsync(user);

        foreach (string roleName in roleNames)
        {
            UserRoles? role = UserRoles.GetRoleWithName(roleName);

            if (role is null) return new UnexpectedFailureResult();

            userRoles.Add(role);
        }

        UserDisplayData userDisplayData = new()
        {
            User = user,
            UserRoles = userRoles
        };

        return userDisplayData;
    }
}