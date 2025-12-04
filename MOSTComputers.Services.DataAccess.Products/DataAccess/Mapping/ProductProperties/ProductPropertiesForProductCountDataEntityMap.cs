using Dapper.FluentMap.Mapping;
using MOSTComputers.Services.DataAccess.Products.Models.Responses.ProductProperties;
using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils.PropertiesTable;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Mapping.ProductProperties;
internal sealed class ProductPropertiesForProductCountDataEntityMap : EntityMap<ProductPropertiesForProductCountData>
{
    public ProductPropertiesForProductCountDataEntityMap()
    {
        Map(x => x.ProductId).ToColumn(ProductIdColumnAlias);
        Map(x => x.PropertyCount).ToColumn(CountColumnName);
    }
}