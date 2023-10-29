using Dapper.FluentMap.Mapping;
using MOSTComputers.Services.DAL.Models;

namespace MOSTComputers.Services.DAL.Mapping;

internal sealed class ProductImageEntityMap : EntityMap<ProductImage>
{
    public ProductImageEntityMap()
    {
        Map(x => x.Id).ToColumn("ID");
        Map(x => x.ProductId).ToColumn("ImageProductId");
        Map(x => x.XML).ToColumn("XMLData");
        Map(x => x.ImageData).ToColumn("Image");
        Map(x => x.ImageFileExtension).ToColumn("ImageFileExt");
        Map(x => x.DateModified).ToColumn("DateModified");
    }
}