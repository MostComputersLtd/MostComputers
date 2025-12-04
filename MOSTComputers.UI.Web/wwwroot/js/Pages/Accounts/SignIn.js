function redirectToLogInPage(returnUrlInputElementId)
{
    var returnUrlInputElement = document.getElementById(returnUrlInputElementId);

    if (returnUrlInputElement == null) return;

    var returnUrl = returnUrlInputElement.value;

    var url = "/Accounts/Signin" + "?handler=RedirectToLogInPage";

    if (returnUrl != null
        && returnUrl.length > 0)
    {
        url += "&returnUrl=" + returnUrl;
    }

    $.ajax({
        type: "GET",
        url: url,
        contentType: "application/json",
        data: null,
        headers:
        {
            RequestVerificationToken:
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    })
        .done(function (result)
        {
            window.location.href = result.redirectUrl;
        });
}

async function signIn(
    usernameInputElementId,
    passwordInputElementId,
    confirmPasswordInputElementId,
    roleSelectCheckboxName,
    roleSelectElementsName,
    returnUrlInputElementId,
    usageInputElementId)
{
    const usernameInputElement = document.getElementById(usernameInputElementId);
    const passwordInputElement = document.getElementById(passwordInputElementId);
    const confirmPasswordInputElement = document.getElementById(confirmPasswordInputElementId);

    const usageInputElement = document.getElementById(usageInputElementId);
    const returnUrlInputElement = document.getElementById(returnUrlInputElementId);

    if (usageInputElement == null
        || returnUrlInputElement == null) return;

    const usage = parseInt(usageInputElement.value);

    if (isNaN(usage)) return;

    const returnUrl = returnUrlInputElement.value;

    const roleSelectCheckboxes = [...document.getElementsByName(roleSelectCheckboxName)];

    const selectedRoleValues = [];
    roleSelectCheckboxes.forEach(function (checkbox)
    {
        if (checkbox.checked)
        {
            const value = checkbox.getAttribute(roleValueAttributeName);

            selectedRoleValues.push(value);
        }
    });

    const url = "/Accounts/Signin/" + "?handler=SignIn";

    const data =
    {
        Username: usernameInputElement.value,
        Password: passwordInputElement.value,
        ConfirmPassword: confirmPasswordInputElement.value,
        Usage: usage,
        ReturnUrl: returnUrl,
        RoleValues: selectedRoleValues
    };

    const response = await fetch(url,
    {
        method: "POST",
        headers: {
            'Content-Type': 'application/json',
            RequestVerificationToken:
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
        body: JSON.stringify(data)
    });


    return await handleSignInResult(
        response, usernameInputElement, passwordInputElement, confirmPasswordInputElement, roleSelectCheckboxName, roleSelectElementsName);
}

async function handleSignInResult(
    response,
    usernameInputElement,
    passwordInputElement,
    confirmPasswordInputElement,
    roleSelectCheckboxName,
    roleSelectElementsName)
{
    clearFields(usernameInputElement, passwordInputElement, confirmPasswordInputElement, roleSelectCheckboxName, roleSelectElementsName);
    clearValidationFields();

    const responseJson = await response.json();

    if (response.status !== 200)
    {
        displayValidationMessages(responseJson);

        return response;
    }

    if (responseJson.redirectUrl == null
        || responseJson.redirectUrl.length <= 0)
    {
        return;
    }

    window.location.href = responseJson.redirectUrl;
}

function clearFields(usernameInputElement, passwordInputElement, confirmPasswordInputElement, roleSelectCheckboxName, roleSelectElementsName)
{
    usernameInputElement.value = "";
    passwordInputElement.value = "";
    confirmPasswordInputElement.value = "";

    const roleSelectElements = [...document.getElementsByName(roleSelectElementsName)];

    roleSelectElements.forEach(function (element)
    {
        element.classList.remove(roleSelectedElementClassName);
    });

    const roleSelectCheckboxes = [...document.getElementsByName(roleSelectCheckboxName)];

    roleSelectCheckboxes.forEach(function (checkbox)
    {
        checkbox.checked = false;
    });
}