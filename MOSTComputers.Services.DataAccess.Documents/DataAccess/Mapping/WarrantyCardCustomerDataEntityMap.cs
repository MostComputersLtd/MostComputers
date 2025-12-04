using Dapper.FluentMap.Mapping;
using MOSTComputers.Services.DataAccess.Documents.Models;

using static MOSTComputers.Services.DataAccess.Documents.Utils.TableAndColumnNameUtils.WarrantyCardsTable;

namespace MOSTComputers.Services.DataAccess.Documents.DataAccess.Mapping;

internal sealed class WarrantyCardCustomerDataEntityMap : EntityMap<WarrantyCardCustomerData>
{
    public WarrantyCardCustomerDataEntityMap()
    {
        Map(x => x.CustomerName).ToColumn(CustomerNameColumn);
        Map(x => x.CustomerBID).ToColumn(CustomerBIDColumn);
    }
}