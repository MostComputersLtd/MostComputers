function redirectToSignInPage(returnUrlInputId)
{
    var returnUrlInput = document.getElementById(returnUrlInputId);

    if (returnUrlInput == null) return;

    var returnUrl = returnUrlInput.value;

    const url = "/Accounts/Login" + "?handler=RedirectToSignInPage" + "&returnUrl=" + returnUrl;

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