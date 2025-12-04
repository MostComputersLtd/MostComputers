using MOSTComputers.Services.DataAccess.Documents.Models;
using MOSTComputers.Services.DataAccess.Documents.Models.Requests.WarrantyCard;

namespace MOSTComputers.UI.Web.Blazor.Services.Xml.Contracts;
internal interface IWarrantyCardToXmlService
{
    Task GetXmlForWarrantyCardsAsync(Stream outputStream, IEnumerable<WarrantyCard> warrantyCards);
    Task GetXmlForWarrantyCardsAsync(Stream outputStream, WarrantyCardSearchRequest warrantyCardSearchRequest);
}