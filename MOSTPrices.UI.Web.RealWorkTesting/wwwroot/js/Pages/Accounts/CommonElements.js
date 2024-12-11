const roleSelectedElementClassName = "role-element-selected";

function toggleRoleSelect(roleElementId, roleSelectCheckboxId)
{
    const roleElement = document.getElementById(roleElementId);
    const roleSelectCheckbox = document.getElementById(roleSelectCheckboxId);

    roleSelectCheckbox.checked = !roleSelectCheckbox.checked;

    if (roleSelectCheckbox.checked)
    {
        roleElement.classList.add(roleSelectedElementClassName);
    }
    else
    {
        roleElement.classList.remove(roleSelectedElementClassName);
    }
}

async function redirectToSignInPage(usage, returnUrlInputElementId)
{
    const returnUrlInputElement = document.getElementById(returnUrlInputElementId);

    if (returnUrlInputElement == null) return;

    const returnUrl = returnUrlInputElement.value;

    return await redirectToSignInPageCommon(usage, returnUrl);
}

async function redirectToSignInPageCommon(usage, returnUrl)
{
    var url = "/Accounts/SignIn";

    if (usage != null)
    {
        url += "?usage=" + usage;
    }

    if (returnUrl != null
        && returnUrl.length > 0)
    {
        const startingSymbol = (usage != null) ? "&" : "?";

        url += startingSymbol + "returnUrl=" + returnUrl;
    }

    window.location.href = url;
}