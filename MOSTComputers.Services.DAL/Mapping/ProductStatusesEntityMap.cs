using Dapper.FluentMap.Mapping;
using MOSTComputers.Models.Product.Models.ProductStatuses;

namespace MOSTComputers.Services.DAL.Mapping;

internal sealed class ProductStatusesEntityMap : EntityMap<ProductStatuses>
{
    public ProductStatusesEntityMap()
    {
        Map(x => x.ProductId).ToColumn("CSTID");
        Map(x => x.IsProcessed).ToColumn("IsProcessed");
        Map(x => x.NeedsToBeUpdated).ToColumn("NeedsToBeUpdated");
    }
}