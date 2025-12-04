using Dapper.FluentMap.Mapping;
using MOSTComputers.Services.DataAccess.Documents.Models.DAO;
using static MOSTComputers.Services.DataAccess.Documents.Utils.TableAndColumnNameUtils.WarrantyCardItemsTable;

namespace MOSTComputers.Services.DataAccess.Documents.DataAccess.Mapping;
internal class WarrantyCardItemDAOEntityMap : EntityMap<WarrantyCardItemDAO>
{
    public WarrantyCardItemDAOEntityMap()
    {
        Map(x => x.ExportedItemId).ToColumn(ExportedItemIdColumn);
        Map(x => x.ExportId).ToColumn(ExportIdAlias);
        Map(x => x.OrderId).ToColumn(OrderIdAlias);
        Map(x => x.ProductId).ToColumn(ProductIdColumn);
        Map(x => x.ProductName).ToColumn(ProductNameColumn);
        Map(x => x.PriceInLeva).ToColumn(PriceInLevaColumn);
        Map(x => x.Quantity).ToColumn(QuantityColumn);
        Map(x => x.SerialNumber).ToColumn(SerialNumberColumn);
        Map(x => x.WarrantyCardItemTermInMonths).ToColumn(WarrantyCardItemTermInMonthsAlias);
        Map(x => x.DisplayOrder).ToColumn(DisplayOrderColumn);
    }
}