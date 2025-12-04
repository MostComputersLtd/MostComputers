using Dapper.FluentMap.Mapping;
using MOSTComputers.Models.Product.Models.Promotions.Groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils.GroupPromotionImagesTable;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Mapping.Promotions.Groups;
internal sealed class GroupPromotionImageEntityMap : EntityMap<GroupPromotionImage>
{
    public GroupPromotionImageEntityMap()
    {
        Map(x => x.Id).ToColumn(IdColumnName);
        Map(x => x.PromotionId).ToColumn(PromotionIdColumnName);
        Map(x => x.ContentType).ToColumn(ContentTypeColumnName);
        Map(x => x.Image).ToColumn(ImageColumnName);
    }
}