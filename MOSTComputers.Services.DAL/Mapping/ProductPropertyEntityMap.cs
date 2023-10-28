using Dapper.FluentMap.Mapping;
using MOSTComputers.Services.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOSTComputers.Services.DAL.Mapping;

internal class ProductPropertyEntityMap : EntityMap<ProductProperty>
{
    public ProductPropertyEntityMap()
    {
        Map(x => x.ProductId).ToColumn("CSTID");
        Map(x => x.ProductCharacteristicId).ToColumn("ProductKeywordID");
        Map(x => x.DisplayOrder).ToColumn("S");
        Map(x => x.Characteristic).ToColumn("Keyword");
        Map(x => x.Value).ToColumn("KeywordValue");
        Map(x => x.XmlPlacement).ToColumn("Discr");
    }
}