using Dapper.FluentMap.Mapping;
using MOSTComputers.Models.Product.Models.Promotions.Files;
using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils.PromotionFilesTable;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Mapping.Promotions.Files;
internal sealed class PromotionFileInfoEntityMap : EntityMap<PromotionFileInfo>
{
    public PromotionFileInfoEntityMap()
    {
        Map(x => x.Id).ToColumn(IdColumnAlias);
        Map(x => x.Name).ToColumn(NameColumnName);
        Map(x => x.Active).ToColumn(ActiveColumnAlias);
        Map(x => x.ValidFrom).ToColumn(ValidFromDateColumnAlias);
        Map(x => x.ValidTo).ToColumn(ValidToDateColumnAlias);
        Map(x => x.FileName).ToColumn(FileNameColumnName);
        Map(x => x.Description).ToColumn(DescriptionColumnName);
        Map(x => x.RelatedProductsString).ToColumn(RelatedProductsColumnName);
        Map(x => x.CreateUserName).ToColumn(CreateUserNameColumnAlias);
        Map(x => x.CreateDate).ToColumn(CreateDateColumnAlias);
        Map(x => x.LastUpdateUserName).ToColumn(LastUpdateUserNameColumnAlias);
        Map(x => x.LastUpdateDate).ToColumn(LastUpdateDateColumnAlias);
    }
}