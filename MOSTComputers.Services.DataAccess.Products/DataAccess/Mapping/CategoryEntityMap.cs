using Dapper.FluentMap.Mapping;
using MOSTComputers.Models.Product.Models;

using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils.CategoriesTable;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Mapping;
internal sealed class CategoryEntityMap : EntityMap<Category>
{
    public CategoryEntityMap()
    {
        Map(x => x.Id).ToColumn(IdColumnName);
        Map(x => x.Description).ToColumn(DescriptionColumnName);
        Map(x => x.IsLeaf).ToColumn(IsLeafColumnName);
        Map(x => x.DisplayOrder).ToColumn(DisplayOrderColumnName);
        Map(x => x.RowGuid).ToColumn(RowGuidColumnName);
        Map(x => x.ProductsUpdateCounter).ToColumn(ProductsUpdateCounterColumnName);
        Map(x => x.ParentCategoryId).ToColumn(ParentCategoryIdColumnName);
    }
}