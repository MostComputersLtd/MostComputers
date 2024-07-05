using Dapper.FluentMap.Mapping;
using MOSTComputers.Models.Product.Models.ProductStatuses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOSTComputers.Services.DAL.Mapping;
internal sealed class ProductWorkStatusesEntityMap : EntityMap<ProductWorkStatuses>
{
    public ProductWorkStatusesEntityMap()
    {
        Map(x => x.Id).ToColumn("ProductWorkStatusId");
        Map(x => x.ProductId).ToColumn("ProductWorkStatusProductId");
        Map(x => x.ProductNewStatus).ToColumn("ProductNewStatus");
        Map(x => x.ProductXmlStatus).ToColumn("ProductXmlReadyStatus");
        Map(x => x.ReadyForImageInsert).ToColumn("ReadyForImageInsertStatus");
    }
}