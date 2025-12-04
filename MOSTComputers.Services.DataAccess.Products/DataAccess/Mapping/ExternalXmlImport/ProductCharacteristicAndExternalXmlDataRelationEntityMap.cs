using Dapper.FluentMap.Mapping;
using MOSTComputers.Models.Product.Models.ExternalXmlImport;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Mapping.ExternalXmlImport;
internal sealed class ProductCharacteristicAndExternalXmlDataRelationEntityMap : EntityMap<ProductCharacteristicAndExternalXmlDataRelation>
{
    public ProductCharacteristicAndExternalXmlDataRelationEntityMap()
    {
        Map(x => x.Id).ToColumn("Id");
        Map(x => x.CategoryId).ToColumn("TID");
        Map(x => x.ProductCharacteristicId).ToColumn("ProducKeywordID");
        Map(x => x.XmlName).ToColumn("XmlName");
        Map(x => x.XmlDisplayOrder).ToColumn("XmlDisplayOrder");
    }
}