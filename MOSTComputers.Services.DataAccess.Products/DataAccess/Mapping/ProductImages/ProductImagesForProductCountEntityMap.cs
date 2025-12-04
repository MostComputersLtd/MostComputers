using Dapper.FluentMap.Mapping;
using MOSTComputers.Services.DataAccess.Products.Models.Responses.ProductImages;
using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils.AllImagesTable;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Mapping.ProductImages;
internal sealed class ProductImagesForProductCountDataEntityMap : EntityMap<ProductImagesForProductCountData>
{
    public ProductImagesForProductCountDataEntityMap()
    {
        Map(x => x.ProductId).ToColumn(ProductIdColumnAlias);
        Map(x => x.ImageCount).ToColumn(CountColumnName);
    }
}