using System.Xml;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Utils;
public static class XmlContentValidationUtils
{
    public static bool IsNullOrValidXmlString(string? value)
    {
        return value is null || ContainsOnlyValidXmlCharacters(value);
    }

    public static bool ContainsOnlyValidXmlCharacters(ReadOnlySpan<char> value)
    {
        foreach (char character in value)
        {
            if (!XmlConvert.IsXmlChar(character))
            {
                return false;
            }
        }

        return true;
    }
}