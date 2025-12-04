const adminPanelPagePath = "/Accounts/Admin/AdminPanel";

async function toggleUserRole(userId, roleValue, roleElementId, roleSelectCheckboxId)
{
    const url = adminPanelPagePath + "?handler=ToggleUserRole" + "&userId=" + userId + "&roleValue=" + roleValue;

    const response = await fetch(url,
        {
            method: "POST",
            headers: {
                'Content-Type': 'application/json',
                RequestVerificationToken:
                    $('input:hidden[name="__RequestVerificationToken"]').val()
            },
        });

    if (response.status !== 200) return null;

    const responseData = await response.json();

    const roleSelectCheckbox = document.getElementById(roleSelectCheckboxId);

    if (roleSelectCheckbox.checked !== responseData.isUserInRole)
    {
        toggleRoleSelect(roleElementId, roleSelectCheckboxId);
    }
}

async function openChangePasswordModalAsync(userId, changePasswordModalId, changePasswordModalDialogId, changePasswordModalContentId)
{
    await openModalWithDataFuncAsync(
        changePasswordModalId,
        changePasswordModalDialogId,
        changePasswordModalContentId,
        async function ()
        {
            return await getChangeUserPasswordPartial(userId)
        })
}

async function getChangeUserPasswordPartial(userId)
{
    const url = adminPanelPagePath + "?handler=GetChangeUserPasswordPartial" + "&userId=" + userId;

    const response = await fetch(url,
    {
        method: "GET",
        headers: {
            'Content-Type': 'application/json',
            RequestVerificationToken:
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    });

    if (response.status !== 200) return null;

    return response.text();
}

async function changeUserPassword(userId, newPasswordInputId, notificationBoxId = null)
{
    const newPasswordInput = document.getElementById(newPasswordInputId);

    if (newPasswordInput.value == null) return;

    const url = adminPanelPagePath + "?handler=ChangeUserPassword" + "&userId=" + userId + "&newPassword=" + newPasswordInput.value;

    const response = await fetch(url,
    {
        method: "PUT",
        headers: {
            'Content-Type': 'application/json',
            RequestVerificationToken:
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    });

    if (response.status !== 200)
    {
        showNotificationWithText(notificationBoxId, "A failure occured", "notificationBox-long-message");

        return response;
    }

    showNotificationWithText(notificationBoxId, "Password changed", "notificationBox-long-message");
}

async function openDeleteUserConfirmationModalAsync(userId, deleteUserConfirmationModalId, deleteUserConfirmationModalDialogId, deleteUserConfirmationModalContentId)
{
    await openModalWithDataFuncAsync(
        deleteUserConfirmationModalId,
        deleteUserConfirmationModalDialogId,
        deleteUserConfirmationModalContentId,
        async function ()
        {
            return await getDeleteUserConfirmationPartial(userId)
        })
}


async function getDeleteUserConfirmationPartial(userId)
{
    const url = adminPanelPagePath + "?handler=GetDeleteUserConfirmationPartial" + "&userId=" + userId;

    const response = await fetch(url,
    {
        method: "GET",
        headers: {
            'Content-Type': 'application/json',
            RequestVerificationToken:
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    });

    if (response.status !== 200) return null;

    return response.text();
}

async function deleteUser(userId, userListPartialContainerElementId, notificationBoxId = null)
{
    const url = adminPanelPagePath + "?handler=DeleteUser" + "&userId=" + userId;

    const response = await fetch(url,
    {
        method: "DELETE",
        headers: {
            'Content-Type': 'application/json',
            RequestVerificationToken:
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    });

    if (response.status !== 200)
    {
        showNotificationWithText(notificationBoxId, "A failure occured", "notificationBox-long-message");

        return response;
    }
    
    showNotificationWithText(notificationBoxId, "Deleted", "notificationBox-long-message");

    const userListPartialContainerElement = document.getElementById(userListPartialContainerElementId);

    userListPartialContainerElement.innerHTML = await response.text();
}