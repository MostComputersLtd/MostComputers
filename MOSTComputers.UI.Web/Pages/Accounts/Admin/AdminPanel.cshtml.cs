using OneOf;
using OneOf.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.Identity.Models;
using MOSTComputers.Services.Identity.Services;
using MOSTComputers.UI.Web.Models;
using MOSTComputers.UI.Web.Models.Authentication;
using MOSTComputers.UI.Web.Validation;
using MOSTComputers.UI.Web.Pages.Shared.Accounts.Admin;

using static MOSTComputers.UI.Web.Validation.ValidationCommonElements;
using MOSTComputers.UI.Web.Services.Data.Accounts.Contracts;
using MOSTComputers.UI.Web.Services.Authentication;

namespace MOSTComputers.UI.Web.Pages.Accounts.Admin;

[Authorize(Roles = "Admin")]
public class AdminPanelModel : PageModel
{
    public AdminPanelModel(IIdentityService<PasswordsTableOnlyUser, PasswordsTableOnlyRole> identityService,
        IUserDisplayDataService userEditorStorageService,
        IAuthenticationService<PasswordsTableOnlyUser> authenticationService)
    {
        _identityService = identityService;
        _userDisplayDataService = userEditorStorageService;
        _authenticationService = authenticationService;
    }

    internal readonly string _changePasswordPartialPath = "Accounts/Admin/_ChangePasswordPopupPartial";
    internal readonly string _userDeleteConfirmationPartialPath = "Accounts/Admin/_UserDeleteConfirmationPartial";
    internal readonly string UserListPartialPath = "Accounts/Admin/_UserListPartial";

    internal readonly string UsersListPartialContainerElementId = "user_list_container";

    internal readonly string NotificationBoxElementId = "topNotificationBox";

    internal readonly ModalData ChangePasswordModalData = new()
    {
        ModalId = "ChangePassword_modal",
        ModalDialogId = "ChangePassword_modal_dialog",
        ModalContentId = "ChangePassword_modal_content",
    };

    internal readonly ModalData UserDeleteConfirmationModalData = new()
    {
        ModalId = "UserDeleteConfirmation_modal",
        ModalDialogId = "UserDeleteConfirmation_modal_dialog",
        ModalContentId = "UserDeleteConfirmation_modal_content",
    };

    private readonly IIdentityService<PasswordsTableOnlyUser, PasswordsTableOnlyRole> _identityService;
    private readonly IUserDisplayDataService _userDisplayDataService;
    private readonly IAuthenticationService<PasswordsTableOnlyUser> _authenticationService;

    public IReadOnlyList<UserDisplayData>? Users { get; private set; }

    public async Task<IActionResult> OnGetAsync()
    {
        OneOf<List<UserDisplayData>, UnexpectedFailureResult> getUserDataResult = await _userDisplayDataService.GetAllUsersAsync();

        return getUserDataResult.Match<IActionResult>(
            userData =>
            {
                Users = userData;

                return Page();
            },
            unexpectedFailureResult => StatusCode(500));
    }

    public async Task<IActionResult> OnGetGetChangeUserPasswordPartialAsync(string userId)
    {
        OneOf<UserDisplayData?, UnexpectedFailureResult> getUserDataResult = await _userDisplayDataService.GetUserAsync(userId);

        return getUserDataResult.Match<IActionResult>(
           userData =>
           {
               if (userData is null) return NotFound();

               return Partial(_changePasswordPartialPath, new ChangePasswordPopupPartialModel()
               {
                   ModalContainerId = ChangePasswordModalData.ModalId,
                   UserData = userData,
                   NotificationBoxId = NotificationBoxElementId,
               });
           },
           unexpectedFailureResult => StatusCode(500));
    }

    public async Task<IActionResult> OnGetGetDeleteUserConfirmationPartialAsync(string userId)
    {
        OneOf<UserDisplayData?, UnexpectedFailureResult> getUserDataResult = await _userDisplayDataService.GetUserAsync(userId);

        return getUserDataResult.Match<IActionResult>(
           userData =>
           {
               if (userData is null) return NotFound();

               return Partial(_userDeleteConfirmationPartialPath, new UserDeleteConfirmationPartialModel()
               {
                   UserId = userData.User.Id,
                   UserName = userData.User.UserName,
                   ModalContainerId = UserDeleteConfirmationModalData.ModalId,
                   UserListModalContainerId = UsersListPartialContainerElementId,
                   NotificationBoxId = NotificationBoxElementId,
               });
           },
           unexpectedFailureResult => StatusCode(500));
    }

    public async Task<IActionResult> OnPostToggleUserRoleAsync(string userId, int roleValue)
    {
        PasswordsTableOnlyUser? user = await _identityService.GetUserAsync(userId);

        if (user is null) return NotFound();

        UserRoles? roleWithValue = UserRoles.GetRoleWithValue(roleValue);

        if (roleWithValue is null) return NotFound();

        bool isUserInRole = await _identityService.IsInRoleAsync(user, roleWithValue.RoleName);

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

        if (!deleteUserResult)
        {
            return NotFound();
        }

        OneOf<List<UserDisplayData>, UnexpectedFailureResult> getUserDataResult = await _userDisplayDataService.GetAllUsersAsync();

        return getUserDataResult.Match<IActionResult>(
            userData =>
            {
                UserListPartialModel model = new()
                {
                    Users = userData,
                    ContainerElementId = UsersListPartialContainerElementId,
                    NotificationBoxId = NotificationBoxElementId,
                    ChangePasswordModalData = ChangePasswordModalData,
                    UserDeleteConfirmationModalData = UserDeleteConfirmationModalData,
                };

                return Partial(UserListPartialPath, model);
            },
            unexpectedFailureResult => StatusCode(500));
    }
}