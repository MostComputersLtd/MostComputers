using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Html.Legacy;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Html.Legacy.Contracts;
public interface ILegacyProductHtmlService
{
    LegacyHtmlProduct ParseProductHtml(string html);
}