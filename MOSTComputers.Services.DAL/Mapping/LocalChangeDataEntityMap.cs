using Dapper.FluentMap.Mapping;
using MOSTComputers.Models.Product.Models.Changes.Local;

namespace MOSTComputers.Services.DAL.Mapping;

internal sealed class LocalChangeDataEntityMap : EntityMap<LocalChangeData>
{
    public LocalChangeDataEntityMap()
    {
        Map(x => x.Id).ToColumn("PK");
        Map(x => x.TableElementId).ToColumn("ID");
        Map(x => x.OperationType).ToColumn("Operation");
        Map(x => x.TableName).ToColumn("TableName");
        Map(x => x.TimeStamp).ToColumn("TimeStamp");
    }
}