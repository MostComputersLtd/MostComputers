using Dapper.FluentMap.Mapping;
using MOSTComputers.Services.DataAccess.Documents.Models;

namespace MOSTComputers.Services.DataAccess.Documents.DataAccess.Mapping;

using static MOSTComputers.Services.DataAccess.Documents.Utils.TableAndColumnNameUtils.OrderItemsTable;

internal class OrderItemEntityMap : EntityMap<OrderItem>
{
    public OrderItemEntityMap()
    {
        Map(x => x.Id).ToColumn(IdColumn);
        Map(x => x.OrderId).ToColumn(OrderIdColumnAlias);
        Map(x => x.ProductId).ToColumn(ProductIdColumn);
        Map(x => x.Quantity).ToColumn(QuantityColumn);
        Map(x => x.Price).ToColumn(PriceColumn);
        Map(x => x.AdditionalWarranty).ToColumn(AdditionalWarrantyColumn);
        Map(x => x.FDDC).ToColumn(FDDCColumn);
        Map(x => x.PriceGroup).ToColumn(PriceGroupColumn);
        Map(x => x.ProfitPercent).ToColumn(ProfitPercentColumn);
        Map(x => x.PromotionPId).ToColumn(PromotionPIDColumn);
        Map(x => x.PromotionRId).ToColumn(PromotionRIDColumn);
        Map(x => x.PromotionPAmount).ToColumn(PromotionPAmountColumn);
        Map(x => x.PromotionRAmount).ToColumn(PromotionRAmountColumn);
        Map(x => x.STInfo).ToColumn(STInfoColumn);
        Map(x => x.STInfoW).ToColumn(STInfoWColumn);
        Map(x => x.ExternalInfo).ToColumn(ExternalInfoColumn);
        Map(x => x.InternalInfo).ToColumn(InternalInfoColumn);
    }
}
