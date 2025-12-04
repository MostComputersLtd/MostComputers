using Dapper.FluentMap.Mapping;
using MOSTComputers.Models.Product.Models.Promotions.Groups;

using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils.GroupPromotionImageFileDatasTable;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Mapping.Promotions.Groups;
internal sealed class GroupPromotionImageFileDataEntityMap : EntityMap<GroupPromotionImageFileData>
{
    public GroupPromotionImageFileDataEntityMap()
    {
        Map(p => p.Id).ToColumn(IdColumnName);
        Map(p => p.PromotionId).ToColumn(PromotionIdColumnName);
        Map(p => p.ImageId).ToColumn(ImageIdColumnName);
        Map(p => p.FileName).ToColumn(FileNameColumnName);
    }
}