using MOSTComputers.Models.Product.Models.ExternalXmlImport;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.Legacy;
using MOSTComputers.UI.Web.Models;

namespace MOSTComputers.UI.Web.Pages.Shared.ProductEditor.ProductXmlData;
public class XmlPropertiesPartialModel
{
    public required ModalData ModalData { get; init; }
    public required string ImagesCompareTableContainerElementId { get; init; }
    public LegacyXmlProduct? XmlProduct { get; init; }
    public List<ProductCharacteristicAndExternalXmlDataRelation>? PropertyRelations { get; init; }
}