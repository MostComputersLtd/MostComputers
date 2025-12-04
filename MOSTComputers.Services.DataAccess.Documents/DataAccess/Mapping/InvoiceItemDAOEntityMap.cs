using Dapper.FluentMap.Mapping;
using MOSTComputers.Services.DataAccess.Documents.Models.DAO;
using static MOSTComputers.Services.DataAccess.Documents.Utils.TableAndColumnNameUtils.InvoiceItemsTable;

namespace MOSTComputers.Services.DataAccess.Documents.DataAccess.Mapping;
internal class InvoiceItemDAOEntityMap : EntityMap<InvoiceItemDAO>
{
    public InvoiceItemDAOEntityMap()
    {
        Map(x => x.ExportedItemId).ToColumn(ExportedItemIdColumn);
        Map(x => x.ExportId).ToColumn(ExportIdAlias);
        Map(x => x.IEID).ToColumn(IEIDColumn);
        Map(x => x.InvoiceId).ToColumn(InvoiceIdAlias);
        Map(x => x.Name).ToColumn(NameColumn);
        Map(x => x.PriceInLeva).ToColumn(PriceInLevaColumn);
        Map(x => x.Quantity).ToColumn(QuantityColumn);
        Map(x => x.DisplayOrder).ToColumn(DisplayOrderColumn);
    }
}