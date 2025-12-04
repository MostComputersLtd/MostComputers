using Dapper.FluentMap.Mapping;
using MOSTComputers.Models.Product.Models.ProductIdentifiers;
using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils.ProductSerialNumbersTable;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Mapping.ProductIdentifiers;
internal sealed class ProductSerialNumberEntityMap : EntityMap<ProductSerialNumber>
{
    public ProductSerialNumberEntityMap()
    {
        Map(x => x.ProductId).ToColumn(ProductIdColumnName);
        Map(x => x.SerialNumber).ToColumn(SerialNumberColumnName);
    }
}