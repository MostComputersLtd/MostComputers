namespace MOSTComputers.Services.ProductRegister.StaticUtilities;

internal static class SearchStringSearchUtils
{
    internal static bool SearchStringContainsParts(this string searchString, string searchStringParts)
    {
        string[] parts = searchStringParts.Split(' ');

        foreach (string part in parts)
        {
            if (!searchString.Contains(part))
            {
                return false;
            }
        }

        return true;
    }
}