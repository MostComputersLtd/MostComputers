using Dapper.FluentMap.Mapping;
using MOSTComputers.Services.DAL.Models;

namespace MOSTComputers.Services.DAL.Mapping;

internal sealed class ProductImageFileNameInfoEntityMap : EntityMap<ProductImageFileNameInfo>
{
    public ProductImageFileNameInfoEntityMap()
    {
        Map(x => x.ProductId).ToColumn("CSTID");
        Map(x => x.DisplayOrder).ToColumn("ImgNo");
        Map(x => x.FileName).ToColumn("ImgFileName");
    }
}