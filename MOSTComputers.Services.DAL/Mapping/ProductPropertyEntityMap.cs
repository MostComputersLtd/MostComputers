using Dapper.FluentMap.Mapping;
using MOSTComputers.Services.DAL.Models;

namespace MOSTComputers.Services.DAL.Mapping;

internal sealed class ProductPropertyEntityMap : EntityMap<ProductProperty>
{
    public ProductPropertyEntityMap()
    {
        Map(x => x.ProductId).ToColumn("PropertyProductId");
        Map(x => x.ProductCharacteristicId).ToColumn("ProductKeywordID");
        Map(x => x.DisplayOrder).ToColumn("PropertyDisplayOrder");
        Map(x => x.Characteristic).ToColumn("Keyword");
        Map(x => x.Value).ToColumn("KeywordValue");
        Map(x => x.XmlPlacement).ToColumn("Discr");
    }
}