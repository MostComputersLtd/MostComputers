using System.Diagnostics.CodeAnalysis;

namespace MOSTComputers.Utils.Files;

public static class FileExtensionUtils
{
    public const char DotSymbol = '.';

    public static string? GetExtensionWithDotFromExtensionOrFileName(string extensionOrFileName)
    {
        if (string.IsNullOrWhiteSpace(extensionOrFileName))
        {
            return null;
        }

        if (extensionOrFileName.StartsWith(DotSymbol))
        {
            return extensionOrFileName;
        }

        int indexOfDotInExtensionOrFileName = extensionOrFileName.LastIndexOf(DotSymbol);

        if (indexOfDotInExtensionOrFileName < 0)
        {
            return DotSymbol + extensionOrFileName;
        }

        return extensionOrFileName[indexOfDotInExtensionOrFileName..];
    }
}