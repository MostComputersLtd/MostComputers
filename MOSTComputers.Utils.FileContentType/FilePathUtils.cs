namespace MOSTComputers.Utils.Files;
public static class FilePathUtils
{
    public static string CombinePathsWithSeparator(char separator, params string[] paths)
    {
        IEnumerable<string> pathsTrimmed = paths.Select(
            path => path.Trim(' ', separator));

        return string.Join(separator, pathsTrimmed);
    }
}