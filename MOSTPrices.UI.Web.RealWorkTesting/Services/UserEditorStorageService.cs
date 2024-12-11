using MOSTComputers.UI.Web.RealWorkTesting.Models.Authentication;
using MOSTComputers.UI.Web.RealWorkTesting.Services.Contracts;

namespace MOSTComputers.UI.Web.RealWorkTesting.Services;

internal sealed class UserEditorStorageService : IUserEditorStorageService
{
    private List<UserDisplayData>? _usersToEdit;
    public IReadOnlyList<UserDisplayData>? UsersToEdit => _usersToEdit;

    public void Renew(IEnumerable<UserDisplayData> users)
    {
        List<UserDisplayData> newUsersToEdit = new();

        foreach (UserDisplayData userData in users)
        {
            UserDisplayData? userDataInEditor = _usersToEdit?.FirstOrDefault(x => x.User.Id == userData.User.Id);

            if (userDataInEditor is not null)
            {
                newUsersToEdit.Add(userDataInEditor);

                continue;
            }

            newUsersToEdit.Add(userData);
        }

        _usersToEdit = newUsersToEdit;
    }

    public bool TryAddUser(UserDisplayData user)
    {
        if (_usersToEdit is not null
            && DoesUserExistAlready(user))
        {
            return false;
        }

        _usersToEdit ??= new();

        _usersToEdit.Add(user);

        return true;
    }

    public bool TryAddUsers(IEnumerable<UserDisplayData> users)
    {
        foreach (UserDisplayData user in users)
        {
            bool success = TryAddUser(user);

            if (!success) return false;
        }

        return true;
    }

    public bool TryUpdateUser(int index, UserDisplayData user)
    {
        if (_usersToEdit == null
            || index < 0
            || index >= _usersToEdit.Count)
        {
            return false;
        }

        _usersToEdit[index] = user;

        return true;
    }

    public bool TryRemoveUser(UserDisplayData user)
    {
        if (_usersToEdit is not null
            && _usersToEdit.Contains(user))
        {
            _usersToEdit.Remove(user);

            return true;
        }

        return false;
    }

    public bool TryRemoveUser(string userId)
    {
        if (_usersToEdit is null) return false;

        int indexOfUser = _usersToEdit.FindIndex(x => x.User.Id == userId);

        if (indexOfUser < 0) return false;

        _usersToEdit.RemoveAt(indexOfUser);

        return true;
    }

    private bool DoesUserExistAlready(UserDisplayData userData)
    {
        return _usersToEdit?.Any(x => x.User.Id == userData.User.Id) ?? false;
    }
}