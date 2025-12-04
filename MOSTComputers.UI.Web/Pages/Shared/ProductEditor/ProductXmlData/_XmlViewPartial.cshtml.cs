using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml;
using MOSTComputers.UI.Web.Models;
using OneOf;

namespace MOSTComputers.UI.Web.Pages.Shared.ProductEditor.ProductXmlData;
public class XmlViewPartialModel
{
    public required ModalData ModalData { get; init; }
    public required Product Product { get; init; }
    public required OneOf<string, InvalidXmlResult> ProductXml { get; init; }
}