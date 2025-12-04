using Dapper.FluentMap.Mapping;
using MOSTComputers.Models.Product.Models.Promotions.Groups;

using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils.PromotionGroupsTable;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Mapping.Promotions.Groups;
internal sealed class PromotionGroupEntityMap : EntityMap<PromotionGroup>
{
    public PromotionGroupEntityMap()
    {
        Map(x => x.Id).ToColumn(IdColumnName);
        Map(x => x.Name).ToColumn(NameColumnName);
        Map(x => x.DisplayOrder).ToColumn(DisplayOrderColumnName);
        Map(x => x.Header).ToColumn(HeaderColumnName);
        Map(x => x.LogoImage).ToColumn(LogoColumnName);
        Map(x => x.LogoContentType).ToColumn(LogoContentTypeColumnName);
        Map(x => x.IsDefault).ToColumn(IsDefaultColumnName);
        Map(x => x.ShowEmptyForLogged).ToColumn(ShowEmptyForLoggedColumnName);
        Map(x => x.ShowEmptyForNonLogged).ToColumn(ShowEmptyForNonLoggedColumnName);
    }
}