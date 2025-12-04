using Dapper.FluentMap.Mapping;
using MOSTComputers.Models.Product.Models.ExternalXmlImport;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Mapping.ExternalXmlImport;
internal sealed class ProductCharacteristicAndImageHtmlRelationEntityMap : EntityMap<ProductCharacteristicAndImageHtmlRelation>
{
    public ProductCharacteristicAndImageHtmlRelationEntityMap()
    {
        Map(x => x.Id).ToColumn("Id");
        Map(x => x.CategoryId).ToColumn("TID");
        Map(x => x.ProductCharacteristicId).ToColumn("ProducKeywordID");
        Map(x => x.HtmlName).ToColumn("HtmlName");
    }
}