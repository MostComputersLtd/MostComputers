async function toggleUserRole(userId, roleValue, roleElementId, roleSelectCheckboxId)
{
    const url = "/Accounts/Admin/AdminPanel" + "?handler=ToggleUserRole" + "&userId=" + userId + "&roleValue=" + roleValue;

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

function openChangePassword_modal(userId)
{
    openModalWithDataFuncAsync('ChangePassword_modal',
        'ChangePassword_modal_dialog',
        'ChangePassword_popup_modal_content',
        async function ()
        {
            return await getChangeUserPasswordPartial(userId)
        })
}

async function getChangeUserPasswordPartial(userId)
{
    const url = "/Accounts/Admin/AdminPanel" + "?handler=GetChangeUserPasswordPartial" + "&userId=" + userId;

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

    const url = "/Accounts/Admin/AdminPanel" + "?handler=ChangeUserPassword" + "&userId=" + userId + "&newPassword=" + newPasswordInput.value;

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

async function deleteUser(userId, containerElementId, notificationBoxId = null)
{
    const url = "/Accounts/Admin/AdminPanel" + "?handler=DeleteUser" + "&userId=" + userId;

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

    const containerElement = document.getElementById(containerElementId);

    containerElement.innerHTML = await response.text();
}