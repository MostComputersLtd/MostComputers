using MOSTComputers.Services.HTMLAndXMLDataOperations.Models;
using System.Text;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.StaticUtils;

internal static class ProductDeserializeXmlTransformUtils
{
    private const string _smallerSignEscape = "|smallerSign|";
    private const string _greaterSignEscape = "|biggerSign|";

    private const string _propertyTagStart = "<property";
    private const string _propertyTagEnd = "</property>";

    private static readonly HashSet<string> _knownXmlEscapeKeywords = new()
    {
        "&#",
        "&lt;",
        "&gt;",
        "&apos;",
        "&quot;",
        "&amp;"
    };

    public static string TransformInputToWorkingXml(ReadOnlySpan<char> xml)
    {
        xml = EscapeAmpersandsThatArentUsedToEscapeOtherCharacters(xml);
        xml = EscapeSmallerSignInSearchStrings(xml);
        xml = EscapeSmallerSignInProperties(xml);

        return xml.ToString();
    }

    private static string EscapeAmpersandsThatArentUsedToEscapeOtherCharacters(ReadOnlySpan<char> xml)
    {
        StringBuilder output = new(xml.Length);

        for (int charIndex = 0; charIndex < xml.Length; charIndex++)
        {
            if (xml[charIndex] == '&')
            {
                bool ampersandIsAPartOfAnEscapeKeyword = false;

                foreach (string keyword in _knownXmlEscapeKeywords)
                {
                    if (charIndex + keyword.Length <= xml.Length && xml.Slice(charIndex, keyword.Length) == keyword)
                    {
                        ampersandIsAPartOfAnEscapeKeyword = true;

                        break;
                    }
                }

                if (!ampersandIsAPartOfAnEscapeKeyword)
                {
                    output.Append("&amp;");

                    continue;
                }
            }

            output.Append(xml[charIndex]);
        }

        return output.ToString();
    }

    private static string EscapeSmallerSignInSearchStrings(ReadOnlySpan<char> xmlSpan)
    {
        const string searchStringTagStart = "<searchstring>";
        const string searchStringTagEnd = "</searchstring>";

        StringBuilder output = new(xmlSpan.Length);

        int currentSearchStringIndex = 0;

        while (true)
        {
            int startIndexRaw = xmlSpan[currentSearchStringIndex..].IndexOf(searchStringTagStart);

            if (startIndexRaw == -1) break;

            startIndexRaw += currentSearchStringIndex;

            int endIndexRaw = xmlSpan[(startIndexRaw + searchStringTagStart.Length)..].IndexOf(searchStringTagEnd);

            if (endIndexRaw == -1) break;

            endIndexRaw += startIndexRaw + searchStringTagStart.Length;

            output.Append(xmlSpan[currentSearchStringIndex..startIndexRaw]);

            output.Append(searchStringTagStart);

            ReadOnlySpan<char> searchStringSpan = xmlSpan.Slice(startIndexRaw + searchStringTagStart.Length,
                endIndexRaw - startIndexRaw - searchStringTagStart.Length);

            foreach (char xmlChar in searchStringSpan)
            {
                if (xmlChar == '<')
                {
                    output.Append(_smallerSignEscape);
                }
                else if (xmlChar == '>')
                {
                    output.Append(_greaterSignEscape);
                }
                else
                {
                    output.Append(xmlChar);
                }
            }

            output.Append(searchStringTagEnd);

            currentSearchStringIndex = endIndexRaw + searchStringTagEnd.Length;
        }

        output.Append(xmlSpan[currentSearchStringIndex..]);

        return output.ToString();
    }

    //private static string EscapeSmallerSignInProperties(string xml)
    //{
    //    int currentPropertyIndex = 0;

    //    while (currentPropertyIndex != -1)
    //    {
    //        int startIndexRaw = xml.IndexOf(_propertyTagStart, currentPropertyIndex);
    //        int endIndexRaw = xml.IndexOf(_propertyTagEnd, currentPropertyIndex);

    //        if (startIndexRaw == -1 || endIndexRaw == -1) break;

    //        int propertyStartIndex = startIndexRaw + 9;
    //        int propertyEndIndex = endIndexRaw;

    //        int xmlStringChangeOffset = _smallerSignEscape.Length - 1;

    //        for (int i = propertyStartIndex; i < propertyEndIndex; i++)
    //        {
    //            if (xml[i] == '<')
    //            {
    //                xml = xml[..i] + _smallerSignEscape + xml[(i + 1)..];

    //                i += xmlStringChangeOffset;
    //                propertyEndIndex += xmlStringChangeOffset;
    //            }
    //        }

    //        currentPropertyIndex = propertyEndIndex + _propertyTagEnd.Length + 1;
    //    }

    //    return xml;
    //}

    //private static string EscapeSmallerSignInProperties(string xml)
    //{
    //    StringBuilder output = new(xml.Length);

    //    int currentPropertyIndex = 0;

    //    while (true)
    //    {
    //        int startIndexRaw = xml.IndexOf(_propertyTagStart, currentPropertyIndex);

    //        if (startIndexRaw == -1) break;

    //        int endIndexRaw = xml.IndexOf(_propertyTagEnd, startIndexRaw);
    //        int selfClosingIndex = xml.IndexOf("/>", startIndexRaw);

    //        bool isSelfClosing = selfClosingIndex != -1 && (endIndexRaw == -1 || selfClosingIndex < endIndexRaw);

    //        int propertyStartIndex = startIndexRaw + _propertyTagStart.Length;
    //        int propertyEndIndex = isSelfClosing ? selfClosingIndex : endIndexRaw;

    //        // Append everything up to and including the <property ...> start tag
    //        output.Append(xml, currentPropertyIndex, propertyStartIndex - currentPropertyIndex);

    //        for (int i = propertyStartIndex; i < propertyEndIndex; i++)
    //        {
    //            if (xml[i] == '<')
    //            {
    //                output.Append(_smallerSignEscape);
    //            }
    //            else
    //            {
    //                output.Append(xml[i]);
    //            }
    //        }

    //        if (isSelfClosing)
    //        {
    //            output.Append("/>");
    //            currentPropertyIndex = propertyEndIndex + 2;
    //        }
    //        else
    //        {
    //            output.Append(_propertyTagEnd);
    //            currentPropertyIndex = propertyEndIndex + _propertyTagEnd.Length;
    //        }
    //    }

    //    output.Append(xml, currentPropertyIndex, xml.Length - currentPropertyIndex);

    //    return output.ToString();
    //}

    private static string EscapeSmallerSignInProperties(ReadOnlySpan<char> xmlSpan)
    {
        const string selfClosingTagEnd = "/>";

        StringBuilder output = new(xmlSpan.Length);

        int currentPropertyIndex = 0;

        while (true)
        {
            int startIndexRaw = xmlSpan[currentPropertyIndex..].IndexOf(_propertyTagStart);

            if (startIndexRaw == -1) break;

            startIndexRaw += currentPropertyIndex;

            int endIndexRaw = xmlSpan[(startIndexRaw + _propertyTagStart.Length)..].IndexOf(_propertyTagEnd);

            int selfClosingIndex = xmlSpan[startIndexRaw..].IndexOf(selfClosingTagEnd);

            bool isSelfClosing = selfClosingIndex != -1 && (endIndexRaw == -1 || selfClosingIndex < endIndexRaw);

            if (isSelfClosing)
            {
                endIndexRaw = startIndexRaw + selfClosingIndex + selfClosingTagEnd.Length;
            }
            else
            {
                endIndexRaw += startIndexRaw + _propertyTagStart.Length;
            }

            output.Append(xmlSpan[currentPropertyIndex..startIndexRaw]);
            output.Append(xmlSpan[startIndexRaw..(startIndexRaw + _propertyTagStart.Length)]);

            ReadOnlySpan<char> propertySpan = xmlSpan[(startIndexRaw + _propertyTagStart.Length)..endIndexRaw];

            foreach (char xmlChar in propertySpan)
            {
                if (xmlChar == '<')
                {
                    output.Append(_smallerSignEscape);
                }
                else
                {
                    output.Append(xmlChar);
                }
            }

            if (isSelfClosing)
            {
                output.Append(selfClosingTagEnd);

                currentPropertyIndex = endIndexRaw;
            }
            else
            {
                output.Append(_propertyTagEnd);

                currentPropertyIndex = endIndexRaw + _propertyTagEnd.Length - _propertyTagStart.Length;
            }
        }

        output.Append(xmlSpan[currentPropertyIndex..]);

        return output.ToString();
    }

    public static bool TransformXmlDataBackToNormal(XmlObjectData xmlData)
    {
        foreach (XmlProduct xmlProduct in xmlData.Products)
        {
            xmlProduct.SearchString = xmlProduct.SearchString.Replace(_smallerSignEscape, "<");
            xmlProduct.SearchString = xmlProduct.SearchString.Replace(_greaterSignEscape, ">");

            foreach (XmlProductProperty property in xmlProduct.XmlProductProperties)
            {
                property.Name = property.Name?.Replace(_smallerSignEscape, "<") ?? string.Empty;
                property.Value = property.Value?.Replace(_smallerSignEscape, "<") ?? string.Empty;
                property.Order = property.Order?.Replace(_smallerSignEscape, "<") ?? string.Empty;
            }
        }

        return true;
    }

    public static string TransformToOriginalAfterSerialization(string xml)
    {
        return xml.Replace("&lt;", "<")
            .Replace("&gt;", ">")
            .Replace("&apos;", "'")
            .Replace("&quot;", "\"")
            .Replace("&amp;", "&");
    }
}