using Dapper.FluentMap.Mapping;
using MOSTComputers.Models.Product.Models.FailureData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOSTComputers.Services.DAL.Mapping;

internal sealed class FailedPropertyNameOfProductEntityMap : EntityMap<FailedPropertyNameOfProduct>
{
    public FailedPropertyNameOfProductEntityMap()
    {
        Map(x => x.ProductId).ToColumn("CSTID");
        Map(x => x.PropertyName).ToColumn("PropertyName");
    }
}