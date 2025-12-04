using System.Diagnostics.CodeAnalysis;

namespace MOSTComputers.UI.Web.Utils;

internal static class FilePathUtils
{
    private const char _pathSlashCharacter = '/';
    private const string _pathSlashAsString = "/";

    [return: NotNullIfNotNull(nameof(url))]
    internal static string? AddSlashAtTheStart(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)
            || url[0] == _pathSlashCharacter)
        {
            return url;
        }

        return _pathSlashAsString + url;
    }

    internal static string? GetFileExtensionWithoutDot(string fullFileName)
    {
        string? fileExtension = Path.GetExtension(fullFileName);

        if (string.IsNullOrWhiteSpace(fileExtension)) return null;

        return fileExtension[1..];
    }
}