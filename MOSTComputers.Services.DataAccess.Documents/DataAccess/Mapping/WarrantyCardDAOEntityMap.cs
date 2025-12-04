using Dapper.FluentMap.Mapping;
using MOSTComputers.Services.DataAccess.Documents.Models.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static MOSTComputers.Services.DataAccess.Documents.Utils.TableAndColumnNameUtils.WarrantyCardsTable;

namespace MOSTComputers.Services.DataAccess.Documents.DataAccess.Mapping;
internal sealed class WarrantyCardDAOEntityMap : EntityMap<WarrantyCardDAO>
{
    public WarrantyCardDAOEntityMap()
    {
        Map(x => x.ExportId).ToColumn(ExportIdColumn);
        Map(x => x.ExportDate).ToColumn(ExportDateColumn);
        Map(x => x.ExportUserId).ToColumn(ExportUserIdColumn);
        Map(x => x.ExportUser).ToColumn(ExportUserColumn);
        Map(x => x.OrderId).ToColumn(OrderIdColumn);
        Map(x => x.CustomerBID).ToColumn(CustomerBIDColumn);
        Map(x => x.CustomerName).ToColumn(CustomerNameColumn);
        Map(x => x.WarrantyCardDate).ToColumn(WarrantyCardDateColumn);
        Map(x => x.WarrantyCardTerm).ToColumn(WarrantyCardTermColumn);

        Map(x => x.WarrantyCardItems).Ignore();
    }
}