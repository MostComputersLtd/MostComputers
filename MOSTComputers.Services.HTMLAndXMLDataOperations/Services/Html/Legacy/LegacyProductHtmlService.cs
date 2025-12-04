using HtmlAgilityPack;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Html.Legacy;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Html.Legacy.Contracts;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Html.Legacy;
internal sealed class LegacyProductHtmlService : ILegacyProductHtmlService
{
    private const string _bulletSymbolUnencoded = "&bull;";

    public LegacyHtmlProduct ParseProductHtml(string html)
    {
        HtmlDocument doc = new();

        doc.LoadHtml(html);

        LegacyHtmlProduct product = new()
        {
            Properties = new()
        };

        List<HtmlNode>? paragraphs = doc.DocumentNode.SelectNodes("//p")?.ToList();

        if (paragraphs is null || paragraphs.Count == 0)
        {
            return product;
        }

        HtmlNode firstParagraph = paragraphs.First();

        HtmlNode? firstParagraphBold = firstParagraph.SelectSingleNode(".//b");

        if (firstParagraphBold != null)
        {
            string firstParagraphBoldText = firstParagraphBold.InnerText.Trim();

            string[] parts = firstParagraphBoldText.Split("Product ID:", StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length > 0)
            {
                product.Name = parts[0].Trim();
            }
            if (parts.Length > 1 && int.TryParse(parts[1].Trim(), out int id))
            {
                product.Id = id;
            }
        }

        for (int i = 1; i < paragraphs.Count - 1; i++)
        {
            HtmlNode paragraph = paragraphs[i];

            string innerText = paragraph.InnerText.TrimStart('•', ' ', '\u00A0').Trim(); // handle bullet and spaces

            if (innerText.StartsWith(_bulletSymbolUnencoded))
            {
                innerText = innerText[_bulletSymbolUnencoded.Length..];
            }

            int colonIndex = innerText.IndexOf(':');

            if (colonIndex > 0)
            {
                string key = innerText[..colonIndex].Trim();

                string value = innerText[(colonIndex + 1)..].Trim();

                LegacyHtmlProductProperty item = new()
                {
                    Name = key,
                    Value = value
                };

                product.Properties.Add(item);
            }
        }

        HtmlNode lastParagraph = paragraphs.Last();

        HtmlNode? lastParagraphLink = lastParagraph.SelectSingleNode(".//a");

        if (lastParagraphLink is not null)
        {
            product.VendorUrl = lastParagraphLink.GetAttributeValue<string?>("href", null);

        }

        string lastParagraphText = lastParagraph.InnerHtml.Trim();

        string[] vendorUrlAndTitleText = lastParagraphText.Split("<br>");

        product.VendorTitle = vendorUrlAndTitleText.LastOrDefault();

        return product;
    }
}