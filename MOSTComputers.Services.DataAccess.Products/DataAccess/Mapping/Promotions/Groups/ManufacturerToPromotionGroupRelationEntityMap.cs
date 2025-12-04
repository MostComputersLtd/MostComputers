using Dapper.FluentMap.Mapping;
using MOSTComputers.Models.Product.Models.Promotions.Groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils.ManufacturerToPromotionGroupRelationsTable;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Mapping.Promotions.Groups;
internal sealed class ManufacturerToPromotionGroupRelationEntityMap : EntityMap<ManufacturerToPromotionGroupRelation>
{
    public ManufacturerToPromotionGroupRelationEntityMap()
    {
        Map(x => x.ManufacturerId).ToColumn(ManufacturerIdColumnName);
        Map(x => x.PromotionGroupId).ToColumn(PromotionGroupIdColumnName);
    }
}