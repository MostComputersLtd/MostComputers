const loginPageUrl = "account/login";
const accessDeniedUrl = "account/accessDenied"

export async function fetchWithAuthRedirect(url, options)
{
    const response = await fetch(url, options);

    const didNotRedirect = handleAuthRedirect(response);

    return didNotRedirect ? response : null;
}

export function handleAuthRedirect(response) {

    if (response.status === 401) {

        const returnUrl = window.location.pathname + window.location.search;

        const url = getLoginPageUrlWithReturnUrl(loginPageUrl, returnUrl);

        window.location.href = url;

        return false;
    }
    else if (response.status === 403) {

        window.location.href = accessDeniedUrl;

        return false;
    }

    return true;
}

export async function fetchWithAuthRedirectAndReturnUrl(url, options, loginReturnUrl)
{
    const response = await fetch(url, options);

    if (response.status === 401) {

        const url = getLoginPageUrlWithReturnUrl(loginPageUrl, loginReturnUrl);

        window.location.href = url;

        return;
    }
    else if (response.status === 403) {

        window.location.href = accessDeniedUrl;

        return;
    }

    return response;
}

function getLoginPageUrlWithReturnUrl(loginPageUrl, returnUrl) {


    return `${loginPageUrl}?returnUrl=${encodeURIComponent(returnUrl)}`;
}
