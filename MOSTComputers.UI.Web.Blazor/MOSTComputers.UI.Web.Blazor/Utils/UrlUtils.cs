using System.Text;

namespace MOSTComputers.UI.Web.Blazor.Utils;

public sealed class UrlUtils
{
    public static string JoinUrlPath(
        ReadOnlySpan<char> first,
        ReadOnlySpan<char> second)
    {
        StringBuilder result = new();

        AppendPathSegment(result, first);
        AppendPathSegment(result, second);

        return result.ToString();
    }

    public static string JoinUrlPath(
        ReadOnlySpan<char> first,
        ReadOnlySpan<char> second,
        ReadOnlySpan<char> third)
    {
        StringBuilder result = new();

        AppendPathSegment(result, first);
        AppendPathSegment(result, second);
        AppendPathSegment(result, third);

        return result.ToString();
    }

    private static void AppendPathSegment(
        StringBuilder result,
        ReadOnlySpan<char> segment)
    {
        segment = segment.Trim().Trim('/');

        if (segment.IsEmpty) return;

        if (result.Length > 0)
        {
            result.Append('/');
        }

        result.Append(segment);
    }
}
