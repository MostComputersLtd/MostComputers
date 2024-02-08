using Dapper.FluentMap.Mapping;
using MOSTComputers.Models.Product.Models.Changes.Local;

namespace MOSTComputers.Services.DAL.Mapping;

internal sealed class LocalChangeDataEntityMap : EntityMap<LocalChangeData>
{
    public LocalChangeDataEntityMap()
    {
        Map(x => x.Id).ToColumn("LocalChangePK");
        Map(x => x.TableElementId).ToColumn("LocalChangeID");
        Map(x => x.OperationType).ToColumn("LocalChangeOperation");
        Map(x => x.TableName).ToColumn("LocalChangeTableName");
        Map(x => x.TimeStamp).ToColumn("LocalChangeTimeStamp");
    }
}