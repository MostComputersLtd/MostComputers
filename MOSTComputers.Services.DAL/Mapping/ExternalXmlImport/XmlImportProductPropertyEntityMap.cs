using Dapper.FluentMap.Mapping;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.ExternalXmlImport;

namespace MOSTComputers.Services.DAL.Mapping.ExternalXmlImport;

internal sealed class XmlImportProductPropertyEntityMap : EntityMap<XmlImportProductProperty>
{
    public XmlImportProductPropertyEntityMap()
    {
        Map(x => x.ProductId).ToColumn("PropertyProductId");
        Map(x => x.ProductCharacteristicId).ToColumn("ProductKeywordID");
        Map(x => x.DisplayOrder).ToColumn("PropertyDisplayOrder");
        Map(x => x.Characteristic).ToColumn("Keyword");
        Map(x => x.Value).ToColumn("KeywordValue");
        Map(x => x.XmlPlacement).ToColumn("Discr");
        Map(x => x.XmlName).ToColumn("XmlName");
        Map(x => x.XmlDisplayOrder).ToColumn("XmlDisplayOrder");
    }
}