using MOSTComputers.Services.HTMLAndXMLDataOperations.Models;
using System.Text;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.StaticUtils;

internal static class ProductDeserializeXmlTransformUtils
{
    private const string _smallerSignEscape = "|smallerSign|";
    private const string _biggerSignEscape = "|biggerSign|";

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

    public static string TransformInputToWorkingXml(string xml)
    {
        xml = EscapeAmpersandsThatArentUsedToEscapeOtherCharacters(xml);
        xml = EscapeSmallerSignInSearchStrings(xml);
        xml = EscapeSmallerSignInProperties(xml);

        return xml;
    }

    //private static string EscapeAmpersandsThatArentUsedToEscapeOtherCharacters(string xml)
    //{
    //    for (int charIndex = 0; charIndex < xml.Length; charIndex++)
    //    {
    //        if (xml[charIndex] == '&')
    //        {
    //            bool ampersandIsAPartOfAnEscapeKeyword = false;

    //            for (int k = 0; k < _knownXmlEscapeKeywords.Count; k++)
    //            {
    //                if (xml.IndexOf(_knownXmlEscapeKeywords[k], charIndex) == charIndex)
    //                {
    //                    ampersandIsAPartOfAnEscapeKeyword = true;

    //                    break;
    //                }
    //            }

    //            if (ampersandIsAPartOfAnEscapeKeyword) continue;

    //            xml = xml[..charIndex] + "&amp;" + xml[(charIndex + 1)..];

    //            charIndex += 4;
    //        }
    //    }

    //    return xml;
    //}

    private static string EscapeAmpersandsThatArentUsedToEscapeOtherCharacters(string xml)
    {
        StringBuilder output = new(xml.Length);

        for (int charIndex = 0; charIndex < xml.Length; charIndex++)
        {
            if (xml[charIndex] == '&')
            {
                bool ampersandIsAPartOfAnEscapeKeyword = false;

                foreach (string keyword in _knownXmlEscapeKeywords)
                {
                    if (charIndex + keyword.Length <= xml.Length && xml.Substring(charIndex, keyword.Length) == keyword)
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

    //private static string EscapeSmallerSignInSearchStrings(string xml)
    //{
    //    int currentSearchStringIndex = 0;

    //    const string searchStringTagStart = "<searchstring>";
    //    const string searchStringTagEnd = "</searchstring>";

    //    while (currentSearchStringIndex != -1)
    //    {
    //        int startIndexRaw = xml.IndexOf(searchStringTagStart, currentSearchStringIndex);
    //        int endIndexRaw = xml.IndexOf(searchStringTagEnd, currentSearchStringIndex);

    //        if (startIndexRaw == -1 || endIndexRaw == -1) break;

    //        int searchStringStartIndex = startIndexRaw + searchStringTagStart.Length;
    //        int searchStringEndIndex = endIndexRaw - 1;

    //        int xmlStringChangeOffsetForSmallerSign = _smallerSignEscape.Length - 1;
    //        int xmlStringChangeOffsetForBiggerSign = _biggerSignEscape.Length - 1;

    //        for (int i = searchStringStartIndex; i < searchStringEndIndex; i++)
    //        {
    //            if (xml[i] == '<')
    //            {
    //                xml = xml[..i] + _smallerSignEscape + xml[(i + 1)..];

    //                i += xmlStringChangeOffsetForSmallerSign;
    //                searchStringEndIndex += xmlStringChangeOffsetForSmallerSign;
    //            }
    //            else if (xml[i] == '>')
    //            {
    //                xml = xml[..i] + _biggerSignEscape + xml[(i + 1)..];

    //                i += xmlStringChangeOffsetForBiggerSign;
    //                searchStringEndIndex += xmlStringChangeOffsetForBiggerSign;
    //            }
    //        }

    //        currentSearchStringIndex = searchStringEndIndex + 19;
    //    }

    //    return xml;
    //}

    private static string EscapeSmallerSignInSearchStrings(string xml)
    {
        const string searchStringTagStart = "<searchstring>";
        const string searchStringTagEnd = "</searchstring>";

        StringBuilder output = new(xml.Length);

        int currentSearchStringIndex = 0;

        while (true)
        {
            int startIndexRaw = xml.IndexOf(searchStringTagStart, currentSearchStringIndex);
            int endIndexRaw = xml.IndexOf(searchStringTagEnd, currentSearchStringIndex);

            if (startIndexRaw == -1 || endIndexRaw == -1) break;

            int searchStringStartIndex = startIndexRaw + searchStringTagStart.Length;
            int searchStringEndIndex = endIndexRaw;

            output.Append(xml, currentSearchStringIndex, searchStringStartIndex - currentSearchStringIndex);

            for (int i = searchStringStartIndex; i < searchStringEndIndex; i++)
            {
                if (xml[i] == '<')
                {
                    output.Append(_smallerSignEscape);
                }
                else if (xml[i] == '>')
                {
                    output.Append(_biggerSignEscape);
                }
                else
                {
                    output.Append(xml[i]);
                }
            }

            output.Append(searchStringTagEnd);

            currentSearchStringIndex = searchStringEndIndex + searchStringTagEnd.Length;
        }

        output.Append(xml, currentSearchStringIndex, xml.Length - currentSearchStringIndex);

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

    private static string EscapeSmallerSignInProperties(string xml)
    {
        StringBuilder output = new(xml.Length);

        int currentPropertyIndex = 0;

        while (true)
        {
            int startIndexRaw = xml.IndexOf(_propertyTagStart, currentPropertyIndex);

            if (startIndexRaw == -1) break;

            int endIndexRaw = xml.IndexOf(_propertyTagEnd, startIndexRaw);
            int selfClosingIndex = xml.IndexOf("/>", startIndexRaw);

            bool isSelfClosing = selfClosingIndex != -1 && (endIndexRaw == -1 || selfClosingIndex < endIndexRaw);

            int propertyStartIndex = startIndexRaw + _propertyTagStart.Length;
            int propertyEndIndex = isSelfClosing ? selfClosingIndex : endIndexRaw;

            // Append everything up to and including the <property ...> start tag
            output.Append(xml, currentPropertyIndex, propertyStartIndex - currentPropertyIndex);

            for (int i = propertyStartIndex; i < propertyEndIndex; i++)
            {
                if (xml[i] == '<')
                {
                    output.Append(_smallerSignEscape);
                }
                else
                {
                    output.Append(xml[i]);
                }
            }

            if (isSelfClosing)
            {
                output.Append("/>");
                currentPropertyIndex = propertyEndIndex + 2;
            }
            else
            {
                output.Append(_propertyTagEnd);
                currentPropertyIndex = propertyEndIndex + _propertyTagEnd.Length;
            }
        }

        output.Append(xml, currentPropertyIndex, xml.Length - currentPropertyIndex);

        return output.ToString();
    }

    private static string GetXmlWithEscapedCharacterWithinCertainTags(
        string xml, string xmlElementName, char charToEscape, string strToEscapeWith)
    {
        const string xmlEndTagWithoutValue = "/>";

        int currentIndex = 0;

        string startTag = $"<{xmlElementName}";
        string endTag = $"</{xmlElementName}>";

        while (currentIndex != -1)
        {
            int startIndexRaw = xml.IndexOf(startTag, currentIndex);
            int endIndexRaw = xml.IndexOf(endTag, currentIndex);

            if (startIndexRaw == -1) break;

            int propertyStartIndex = startIndexRaw + startTag.Length;

            if (endIndexRaw == -1)
            {
                string substringFromPropStart = xml[propertyStartIndex..];

                if (substringFromPropStart.TrimStart().StartsWith(xmlEndTagWithoutValue))
                {
                    currentIndex = propertyStartIndex + xmlEndTagWithoutValue.Length;

                    continue;
                }

                break;
            }

            int propertyEndIndex = endIndexRaw;

            string substringBetweenTags = xml[propertyStartIndex..propertyEndIndex];

            int indexOfEndForTagWithoutValue = substringBetweenTags.IndexOf(xmlEndTagWithoutValue);

            if (indexOfEndForTagWithoutValue > -1)
            {
                propertyEndIndex = propertyStartIndex + indexOfEndForTagWithoutValue;
            }

            int escapeLengthChange = strToEscapeWith.Length - 1;

            for (int i = propertyStartIndex; i < propertyEndIndex; i++)
            {
                if (xml[i] == charToEscape)
                {
                    xml = xml[..i] + strToEscapeWith + xml[(i + 1)..];

                    i += escapeLengthChange;
                    propertyEndIndex += escapeLengthChange;
                }
            }

            currentIndex = propertyEndIndex + endTag.Length;
        }

        return xml;
    }

    public static bool TransformXmlDataBackToNormal(XmlObjectData xmlData)
    {
        foreach (XmlProduct item in xmlData.Products)
        {
            item.SearchString = item.SearchString.Replace(_smallerSignEscape, "<");
            item.SearchString = item.SearchString.Replace(_biggerSignEscape, ">");

            foreach (var property in item.XmlProductProperties)
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