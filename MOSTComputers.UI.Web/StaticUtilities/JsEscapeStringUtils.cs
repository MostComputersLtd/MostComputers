using System.Web;

namespace MOSTComputers.UI.Web.StaticUtilities;

public static class JsEscapeStringUtils
{
    public static string? JavascriptEncode(this string value)
    {
        return HttpUtility.JavaScriptStringEncode(value);
    }
}