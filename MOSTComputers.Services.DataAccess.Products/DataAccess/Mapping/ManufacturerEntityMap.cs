using Dapper.FluentMap.Mapping;
using MOSTComputers.Models.Product.Models;

using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils.ManufacturersTable;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Mapping;
internal sealed class ManufacturerEntityMap : EntityMap<Manufacturer>
{
    public ManufacturerEntityMap()
    {
        Map(x => x.Id).ToColumn(IdColumnAlias);
        Map(x => x.RealCompanyName).ToColumn(RealCompanyNameColumnName);
        Map(x => x.BGName).ToColumn(BGNameColumnName);
        Map(x => x.DisplayOrder).ToColumn(DisplayOrderColumnAlias);
        Map(x => x.Active).ToColumn(ActiveColumnName);
    }
}