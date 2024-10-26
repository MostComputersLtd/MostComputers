using Dapper.FluentMap.Mapping;
using MOSTComputers.Models.Product.Models;

namespace MOSTComputers.Services.DAL.Mapping;

public sealed class CategoryEntityMap : EntityMap<Category>
{
    public CategoryEntityMap()
    {
        Map(x => x.Id).ToColumn("CategoryID");
        Map(x => x.Description).ToColumn("Description");
        Map(x => x.IsLeaf).ToColumn("IsLeaf");
        Map(x => x.DisplayOrder).ToColumn("S");
        Map(x => x.RowGuid).ToColumn("rowguid");
        Map(x => x.ProductsUpdateCounter).ToColumn("ProductsUpdateCounter");
        Map(x => x.ParentCategoryId).ToColumn("ParentId");
    }
}