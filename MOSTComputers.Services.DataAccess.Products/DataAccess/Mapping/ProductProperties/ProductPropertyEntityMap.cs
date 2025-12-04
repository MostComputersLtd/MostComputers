using Dapper.FluentMap.Mapping;
using MOSTComputers.Models.Product.Models;
using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils.PropertiesTable;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Mapping.ProductProperties;
internal sealed class ProductPropertyEntityMap : EntityMap<ProductProperty>
{
    public ProductPropertyEntityMap()
    {
        Map(x => x.ProductId).ToColumn(ProductIdColumnAlias);
        Map(x => x.ProductCharacteristicId).ToColumn(ProductCharacteristicIdColumnName);
        Map(x => x.DisplayOrder).ToColumn(DisplayOrderColumnAlias);
        Map(x => x.Characteristic).ToColumn(CharacteristicColumnName);
        Map(x => x.Value).ToColumn(ValueColumnName);
        Map(x => x.XmlPlacement).ToColumn(XmlPlacementColumnName);
    }
}