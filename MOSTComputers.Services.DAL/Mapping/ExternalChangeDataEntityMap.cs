using Dapper.FluentMap.Mapping;
using MOSTComputers.Models.Product.Models.Changes.External;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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