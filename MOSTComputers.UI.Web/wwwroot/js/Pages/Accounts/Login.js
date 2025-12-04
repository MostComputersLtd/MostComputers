async function login(
    usernameInputElementId,
    passwordInputElementId,
    returnUrlInputElementId)
{
    const usernameInputElement = document.getElementById(usernameInputElementId);
    const passwordInputElement = document.getElementById(passwordInputElementId);

    const returnUrlInputElement = document.getElementById(returnUrlInputElementId);

    if (returnUrlInputElement == null) return;

    const returnUrl = returnUrlInputElement.value;

    const url = "/Accounts/Login/" + "?handler=LogIn";

    const data =
    {
        Username: usernameInputElement.value,
        Password: passwordInputElement.value,
        ReturnUrl: returnUrl
    };

    const response = await fetch(url,
    {
        method: "POST",
        headers:
        {
            'Content-Type': 'application/json',
            RequestVerificationToken:
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
        body: JSON.stringify(data)
    });


    return await handleLoginResult(
        response, usernameInputElement, passwordInputElement);
}

async function handleLoginResult(
    response,
    usernameInputElement,
    passwordInputElement)
{
    clearFields(usernameInputElement, passwordInputElement);
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

function clearFields(usernameInputElement, passwordInputElement)
{
    usernameInputElement.value = "";
    passwordInputElement.value = "";
}