using HtmlAgilityPack;

namespace MOSTComputers.Services.PDF.Utils;
internal static class HtmlBasicOperationUtils
{
    internal static string GetIdForNodeInListFromIndex(string nodeIdPrefix, int index)
    {
        return nodeIdPrefix + "-" + index;
    }

    internal static bool ChangeHtmlElementInnerHtml(HtmlDocument document, string elementId, string newInnerHtml)
    {
        HtmlNode? element = document.GetElementbyId(elementId);

        if (element is null) return false;

        element.InnerHtml = newInnerHtml;

        return true;
    }

    internal static bool ChangeHtmlElementInnerHtml(HtmlNode parentNode, string elementId, string newInnerHtml)
    {
        HtmlNode? element = parentNode.ChildNodes.FirstOrDefault(x => x.Id == elementId);

        if (element is null) return false;

        element.InnerHtml = newInnerHtml;

        return true;
    }

    internal static bool ChangeHtmlElement(HtmlDocument htmlDocument, string elementId, Action<HtmlNode> changeAction)
    {
        HtmlNode? element = htmlDocument.GetElementbyId(elementId);

        if (element is null) return false;

        changeAction(element);

        return true;
    }

    internal static bool ChangeHtmlElement(HtmlNode parentNode, string elementId, Action<HtmlNode> changeAction)
    {
        HtmlNode? element = parentNode.ChildNodes.FirstOrDefault(x => x.Id == elementId);

        if (element is null) return false;

        changeAction(element);

        return true;
    }
}