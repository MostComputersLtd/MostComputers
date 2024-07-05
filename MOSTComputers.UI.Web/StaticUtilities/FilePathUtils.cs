namespace MOSTComputers.UI.Web.StaticUtilities;

internal static class FilePathUtils
{
    internal static string? AddSlashAtTheStart(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)
            || url[0] == '/')
        {
            return url;
        }

        return "/" + url;
    }

    internal static string? RemoveSlashAtTheStart(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)
            || url[0] != '/')
        {
            return url;
        }

        return url[1..];
    }
}