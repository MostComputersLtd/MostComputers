using Dapper.FluentMap.Mapping;
using MOSTComputers.Models.Product.Models.Counters;

using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils.SystemCountersTable;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Mapping.Counters;
internal sealed class SystemCountersEntityMap : EntityMap<SystemCounters>
{
    public SystemCountersEntityMap()
    {
        Map(x => x.OriginalChangesLastSearchedPK).ToColumn(OriginalChangesLastSearchedPKColumnName);
    }
}