using Dapper.FluentMap.Mapping;
using MOSTComputers.Services.DataAccess.Documents.Models;

namespace MOSTComputers.Services.DataAccess.Documents.DataAccess.Mapping;

using static MOSTComputers.Services.DataAccess.Documents.Utils.TableAndColumnNameUtils.OrdersTable;

internal class OrderEntityMap : EntityMap<Order>
{
    public OrderEntityMap()
    {
        Map(x => x.Id).ToColumn(IdColumn);
        Map(x => x.Status).ToColumn(StatusColumn);
        Map(x => x.AutoReply).ToColumn(AutoReplyColumn);
        Map(x => x.QuoteId).ToColumn(QuoteIdColumn);
        Map(x => x.UserId).ToColumn(UserIdColumn);
        Map(x => x.OrderDate).ToColumn(OrderDateColumn);
        Map(x => x.OrderName).ToColumn(OrderNameColumn);
        Map(x => x.ReadTime).ToColumn(ReadTimeColumn);
        Map(x => x.ReadUserId).ToColumn(ReadUserIdColumn);
        Map(x => x.BusinessId).ToColumn(BIDColumn);
        Map(x => x.DealId).ToColumn(DealIdColumn);
        Map(x => x.MOST3DealId).ToColumn(MOST3DealIdColumn);
        Map(x => x.IsConfiguration).ToColumn(IsConfigurationColumn);
        Map(x => x.CfgProfit).ToColumn(CfgProfitColumn);
        Map(x => x.Currency).ToColumn(CurrencyColumn);
        Map(x => x.OriginalInternetOrderId).ToColumn(OriginalInternetOrderIdColumn);
        Map(x => x.RepliedInternetOrderId).ToColumn(RepliedInternetOrderIdColumn);
        Map(x => x.Info).ToColumn(InfoColumn);

        Map(x => x.Items).Ignore();
    }
}
