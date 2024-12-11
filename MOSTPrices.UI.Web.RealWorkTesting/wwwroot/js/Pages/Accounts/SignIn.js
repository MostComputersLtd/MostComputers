const roleValueAttributeName = "data-selectedRoleValue";
const validationFieldAttributeName = "data-valmsg-for";
const validationSummaryAttributeName = "data-validation-summary";

const validationFieldTextClassName = "text-danger";

//const roleSelectedElementClassName = "role-element-selected";

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

function displayValidationMessages(responseJson)
{
    const validationSummary = document.querySelector('[' + validationSummaryAttributeName + ']');

    const responseJsonEntries = [...Object.entries(responseJson)];

    for (var i = 0; i < responseJsonEntries.length; i++)
    {
        [validationErrorKey, validationMessages] = responseJsonEntries[i];

        if (validationErrorKey == null) continue;

        const validationDisplayElement = document.querySelector('[' + validationFieldAttributeName + '="' + validationErrorKey + '"]');

        if (!Array.isArray(validationMessages))
        {
            const validationElement = createValidationMessageElement(validationMessages);

            if (validationDisplayElement == null)
            {
                validationSummary.appendChild(validationElement);
            }
            else
            {
                validationDisplayElement.appendChild(validationElement);
            }

            continue;
        }

        validationMessages.forEach(function (message)
        {
            const validationElement = createValidationMessageElement(message);

            if (validationDisplayElement == null)
            {
                validationSummary.appendChild(validationElement);

                return;
            }

            validationDisplayElement.appendChild(validationElement);
        });
    }
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

function clearValidationFields()
{
    const validationSummary = document.querySelector('[' + validationSummaryAttributeName + ']');

    const validationDisplayElements = [...document.querySelectorAll('[' + validationFieldAttributeName + ']')];

    validationSummary.innerHTML = "";

    validationDisplayElements.forEach(function (element)
    {
        element.innerHTML = "";
    });
}

function createValidationMessageElement(message)
{
    const validationMessageElement = document.createElement("span");

    validationMessageElement.classList.add(validationFieldTextClassName);

    validationMessageElement.textContent = message;

    return validationMessageElement;
}