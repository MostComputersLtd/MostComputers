using Dapper.FluentMap.Mapping;
using MOSTComputers.Models.Product.Models.Changes.External;

namespace MOSTComputers.Services.DAL.Mapping;

internal sealed class ExternalChangeDataEntityMap : EntityMap<ExternalChangeData>
{
    public ExternalChangeDataEntityMap()
    {
        Map(x => x.Id).ToColumn("ExternalChangePK");
        Map(x => x.TableElementId).ToColumn("ExternalChangeID");
        Map(x => x.OperationType).ToColumn("ExternalChangeOperation");
        Map(x => x.TableName).ToColumn("ExternalChangeTableName");
    }
}