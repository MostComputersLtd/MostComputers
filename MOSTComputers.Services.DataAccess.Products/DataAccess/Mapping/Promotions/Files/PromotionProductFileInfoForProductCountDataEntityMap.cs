using Dapper.FluentMap.Mapping;
using MOSTComputers.Services.DataAccess.Products.Models.Responses.Promotions.PromotionProductFileInfos;
using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils.PromotionProductFilesTable;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Mapping.Promotions.Files;
internal sealed class PromotionProductFileInfoForProductCountDataEntityMap : EntityMap<PromotionProductFileInfoForProductCountData>
{
    public PromotionProductFileInfoForProductCountDataEntityMap()
    {
        Map(x => x.ProductId).ToColumn(ProductIdColumnName);
        Map(x => x.Count).ToColumn(CountColumnName);
    }
}