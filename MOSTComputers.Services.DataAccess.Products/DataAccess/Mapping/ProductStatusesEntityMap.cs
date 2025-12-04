using Dapper.FluentMap.Mapping;
using MOSTComputers.Models.Product.Models.ProductStatuses;

using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils.ProductStatusesTable;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Mapping;
internal sealed class ProductStatusesEntityMap : EntityMap<ProductStatuses>
{
    public ProductStatusesEntityMap()
    {
        Map(x => x.ProductId).ToColumn(ProductIdColumnName);
        Map(x => x.IsProcessed).ToColumn(IsProcessedColumnName);
        Map(x => x.NeedsToBeUpdated).ToColumn(NeedsToBeUpdatedColumnName);
    }
}