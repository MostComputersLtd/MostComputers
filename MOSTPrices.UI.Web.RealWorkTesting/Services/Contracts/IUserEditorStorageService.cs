using MOSTComputers.UI.Web.RealWorkTesting.Models.Authentication;

namespace MOSTComputers.UI.Web.RealWorkTesting.Services.Contracts;

public interface IUserEditorStorageService
{
    IReadOnlyList<UserDisplayData>? UsersToEdit { get; }

    void Renew(IEnumerable<UserDisplayData> user);
    bool TryAddUser(UserDisplayData user);
    bool TryAddUsers(IEnumerable<UserDisplayData> users);
    bool TryUpdateUser(int index, UserDisplayData user);
    bool TryRemoveUser(UserDisplayData user);
    bool TryRemoveUser(string userId);
}