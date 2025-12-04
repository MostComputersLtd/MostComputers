using Dapper.FluentMap.Mapping;
using MOSTComputers.Services.DataAccess.Products.Models.Responses.Promotions.GroupPromotionImages;

using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils.GroupPromotionImagesTable;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Mapping.Promotions.Groups;

internal sealed class GroupPromotionImageWithoutFileEntityMap : EntityMap<GroupPromotionImageWithoutFile>
{
    public GroupPromotionImageWithoutFileEntityMap()
    {
        Map(x => x.Id).ToColumn(IdColumnName);
        Map(x => x.PromotionId).ToColumn(PromotionIdColumnName);
        Map(x => x.ContentType).ToColumn(ContentTypeColumnName);
    }
}