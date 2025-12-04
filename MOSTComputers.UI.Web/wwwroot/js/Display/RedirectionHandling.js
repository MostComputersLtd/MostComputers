function redirectIfResponseIsRedirected(response)
{
    if (!(response instanceof Response)) return;

    if (!response.redirected) return;

    const url = response.url;

    window.location.href = url;
}