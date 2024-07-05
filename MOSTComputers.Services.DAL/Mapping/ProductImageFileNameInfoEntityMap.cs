using Dapper.FluentMap.Mapping;
using MOSTComputers.Models.Product.Models;

namespace MOSTComputers.Services.DAL.Mapping;

internal sealed class ProductImageFileNameInfoEntityMap : EntityMap<ProductImageFileNameInfo>
{
    public ProductImageFileNameInfoEntityMap()
    {
        Map(x => x.ProductId).ToColumn("CSTID");
        Map(x => x.DisplayOrder).ToColumn("S");
        Map(x => x.ImageNumber).ToColumn("ImageNumber");
        Map(x => x.FileName).ToColumn("ImgFileName");
        Map(x => x.Active).ToColumn("Active");
    }
}