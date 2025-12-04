using Dapper.FluentMap.Mapping;
using MOSTComputers.Models.Product.Models.Changes.Local;

using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils.LocalChangesTable;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Mapping;
internal sealed class LocalChangeDataEntityMap : EntityMap<LocalChangeData>
{
    public LocalChangeDataEntityMap()
    {
        Map(x => x.Id).ToColumn(IdColumnAlias);
        Map(x => x.TableElementId).ToColumn(TableElementIdColumnAlias);
        Map(x => x.OperationType).ToColumn(OperationTypeColumnAlias);
        Map(x => x.TableName).ToColumn(TableNameColumnAlias);
        Map(x => x.TimeStamp).ToColumn(TimeStampColumnAlias);
    }
}