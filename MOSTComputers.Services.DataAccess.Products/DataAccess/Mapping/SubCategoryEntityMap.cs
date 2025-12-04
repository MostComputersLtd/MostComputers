using Dapper.FluentMap.Mapping;
using MOSTComputers.Models.Product.Models;

using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils.SubCategoriesTable;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Mapping;
internal sealed class SubCategoryEntityMap : EntityMap<SubCategory>
{
    public SubCategoryEntityMap()
    {
        Map(x => x.Id).ToColumn(IdColumnName);
        Map(x => x.CategoryId).ToColumn(CategoryIdAlias);
        Map(x => x.Name).ToColumn(NameAlias);
        Map(x => x.DisplayOrder).ToColumn(DisplayOrderAlias);
        Map(x => x.Active).ToColumn(ActiveAlias);
    }
}