using Dapper.FluentMap.Mapping;
using MOSTComputers.Services.DataAccess.Documents.Models;

using static MOSTComputers.Services.DataAccess.Documents.Utils.TableAndColumnNameUtils.FirmsTable;

namespace MOSTComputers.Services.DataAccess.Documents.DataAccess.Mapping;

internal sealed class FirmDataEntityMap: EntityMap<FirmData>
{
    public FirmDataEntityMap()
    {
        Map(x => x.Id).ToColumn(IdColumn);
        Map(x => x.Name).ToColumn(NameColumn);
        Map(x => x.Order).ToColumn(OrderColumn);
        Map(x => x.Info).ToColumn(InfoColumn);
        Map(x => x.InvoiceNumber).ToColumn(InvoiceNumberColumn);
        Map(x => x.Address).ToColumn(AddressColumn);
        Map(x => x.MPerson).ToColumn(MPersonColumn);
        Map(x => x.TaxNumber).ToColumn(TaxNumberColumn);
        Map(x => x.Bulstat).ToColumn(BulstatColumn);
    }
}