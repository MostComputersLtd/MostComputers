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