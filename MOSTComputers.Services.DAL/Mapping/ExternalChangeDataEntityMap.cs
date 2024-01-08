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
        Map(x => x.Id).ToColumn("PK");
        Map(x => x.TableElementId).ToColumn("ID");
        Map(x => x.OperationType).ToColumn("Operation");
        Map(x => x.TableName).ToColumn("TableName");
    }
}