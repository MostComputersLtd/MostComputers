using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.Identity.Models;
using MOSTComputers.UI.Web.RealWorkTesting.Authentication;
using MOSTComputers.UI.Web.RealWorkTesting.Models.Authentication;
using MOSTComputers.UI.Web.RealWorkTesting.Pages.Shared.Accounts.Admin;
using MOSTComputers.UI.Web.RealWorkTesting.Services.Contracts;
using OneOf;
using OneOf.Types;

using static MOSTComputers.UI.Web.RealWorkTesting.Validation.ValidationCommonElements;

namespace MOSTComputers.UI.Web.RealWorkTesting.Pages.Accounts.Admin;

[Authorize(Roles = "Admin")]
public class AdminPanelModel : PageModel
{
    public AdminPanelModel(UserManager<PasswordsTableOnlyUser> userManager,
        IUserEditorStorageService userEditorStorageService,
        IAuthenticationService<PasswordsTableOnlyUser> authenticationService)
    {
        _userManager = userManager;
        _userEditorStorageService = userEditorStorageService;
        _authenticationService = authenticationService;
    }

    private readonly UserManager<PasswordsTableOnlyUser> _userManager;
    private readonly IUserEditorStorageService _userEditorStorageService;
    private readonly IAuthenticationService<PasswordsTableOnlyUser> _authenticationService;

    public IReadOnlyList<UserDisplayData>? Users { get; private set; }

    public async Task<IActionResult> OnGetAsync()
    {
        List<PasswordsTableOnlyUser> allUsers = _userManager.Users.ToList();

        OneOf<List<UserDisplayData>, UnexpectedFailureResult> getUserDataResult = await GetUserDisplayDataFromUsersAsync(allUsers, true);

        return getUserDataResult.Match<IActionResult>(
            userData =>
            {
                _userEditorStorageService.Renew(userData);

                Users = _userEditorStorageService.UsersToEdit;

                return Page();
            },
            unexpectedFailureResult => StatusCode(500));
    }

    public IActionResult OnGetGetChangeUserPasswordPartial(string userId)
    {
        UserDisplayData? userData = _userEditorStorageService.UsersToEdit?.FirstOrDefault(userData => userData.User.Id == userId);

        if (userData is null) return NotFound();

        return Partial("Accounts/Admin/_ChangePasswordPopupPartial", new ChangePasswordPopupPartialModel()
        {
            ModalContainerId = "ChangePassword_modal",
            UserData = userData,
            NotificationBoxId = "topNotificationBox",
        });
    }

    public async Task<IActionResult> OnPostToggleUserRoleAsync(string userId, int roleValue)
    {
        PasswordsTableOnlyUser? user = _userManager.Users.FirstOrDefault(x => x.Id == userId);

        if (user is null) return NotFound();

        UserRoles? roleWithValue = UserRoles.GetRoleWithValue(roleValue);

        if (roleWithValue is null) return NotFound();

        bool isUserInRole = await _userManager.IsInRoleAsync(user, roleWithValue.RoleName);

        if (isUserInRole)
        {
            OneOf<IdentityResult, NotFound> removeUserFromRoleResult = await _authenticationService.TryRemoveUserFromRoleAsync(userId, roleWithValue);

            return removeUserFromRoleResult.Match(
                identityResult =>
                {
                    if (identityResult.Succeeded)
                    {
                        return new JsonResult(new { isUserInRole = false });
                    }

                    return this.GetActionResultFromIdentityResult(identityResult);
                },
                notFound => NotFound());
        }

        OneOf<IdentityResult, NotFound> addUserToRoleResult = await _authenticationService.TryAddUserToRoleAsync(userId, roleWithValue);

        return addUserToRoleResult.Match(
            identityResult =>
            {
                if (identityResult.Succeeded)
                {
                    return new JsonResult(new { isUserInRole = true });
                }

                return this.GetActionResultFromIdentityResult(identityResult);
            },
            notFound => NotFound());
    }

    public async Task<IActionResult> OnPutChangeUserPasswordAsync(string userId, string newPassword)
    {
        OneOf<IdentityResult, NotFound> changePasswordResult = await _authenticationService.ChangePasswordForUserAsync(userId, newPassword);

        return changePasswordResult.Match(
            identityResult => this.GetActionResultFromIdentityResult(identityResult),
            notFound => NotFound());
    }

    public async Task<IActionResult> OnDeleteDeleteUserAsync(string userId)
    {
        bool deleteUserResult = await _authenticationService.DeleteAccountAsync(userId);

        if (deleteUserResult)
        {
            _userEditorStorageService.TryRemoveUser(userId);

            return Partial("Accounts/Admin/_UsersListPartial", new UsersListPartialModel()
            {
                Users = _userEditorStorageService.UsersToEdit,
                ContainerElementId = "user_list_container",
                NotificationBoxId = "topNotificationBox",
            });
        }

        return NotFound();
    }   

    private async Task<OneOf<List<UserDisplayData>, UnexpectedFailureResult>> GetUserDisplayDataFromUsersAsync(
        IEnumerable<PasswordsTableOnlyUser> users, bool includeRoles)
    {
        List<UserDisplayData> output = new();

        foreach (PasswordsTableOnlyUser user in users)
        {
            List<UserRoles>? userRoles = null;

            if (includeRoles)
            {
                userRoles = new();

                IList<string> roleNames = await _userManager.GetRolesAsync(user);

                foreach (string roleName in roleNames)
                {
                    UserRoles? role = UserRoles.GetRoleWithName(roleName);

                    if (role is null) return new UnexpectedFailureResult();

                    userRoles.Add(role);
                }
            }

            UserDisplayData userDisplayData = new()
            {
                User = user,
                UserRoles = userRoles
            };

            output.Add(userDisplayData);
        }

        return output;
    }
}