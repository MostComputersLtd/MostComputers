using Dapper.FluentMap.Mapping;
using MOSTComputers.Services.DataAccess.Products.Models.DAOs.PromotionProductFileInfo;

using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils.PromotionProductFilesTable;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Mapping.Promotions.Files;
internal sealed class PromotionProductFileInfoDAOEntityMap : EntityMap<PromotionProductFileInfoDAO>
{
    public PromotionProductFileInfoDAOEntityMap()
    {
        Map(x => x.Id).ToColumn(IdColumnName);
        Map(x => x.ProductId).ToColumn(ProductIdColumnName);
        Map(x => x.PromotionFileInfoId).ToColumn(PromotionFileIdColumnAlias);
        Map(x => x.ValidFrom).ToColumn(ValidFromDateColumnAlias);
        Map(x => x.ValidTo).ToColumn(ValidToDateColumnAlias);
        Map(x => x.Active).ToColumn(ActiveColumnAlias);
        Map(x => x.ProductImageId).ToColumn(ImagesAllIdColumnName);
        Map(x => x.CreateUserName).ToColumn(CreateUserNameColumnAlias);
        Map(x => x.CreateDate).ToColumn(CreateDateColumnAlias);
        Map(x => x.LastUpdateUserName).ToColumn(LastUpdateUserNameColumnAlias);
        Map(x => x.LastUpdateDate).ToColumn(LastUpdateDateColumnAlias);

        Map(x => x.PromotionFileInfo).Ignore();
    }
}