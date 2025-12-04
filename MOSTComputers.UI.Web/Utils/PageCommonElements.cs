using System.Security.Claims;

namespace MOSTComputers.UI.Web.Utils;
internal static class PageCommonElements
{
    internal static string? GetUserId(ClaimsPrincipal user)
    {
        return user.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    internal static string? GetUserName(ClaimsPrincipal user)
    {
        return user.FindFirstValue(ClaimTypes.Name);
    }

    internal static string GetOnlyPagePartOfUrl(string returnUrl)
    {
        int urlPageHandlerIndex = returnUrl.IndexOf("?handler=");

        if (urlPageHandlerIndex > 0)
        {
            return returnUrl[..urlPageHandlerIndex];
        }

        return returnUrl;
    }
}