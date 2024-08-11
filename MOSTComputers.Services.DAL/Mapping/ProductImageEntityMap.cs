using Dapper.FluentMap.Mapping;
using MOSTComputers.Models.Product.Models;

namespace MOSTComputers.Services.DAL.Mapping;

internal sealed class ProductImageEntityMap : EntityMap<ProductImage>
{
    public ProductImageEntityMap()
    {
        Map(x => x.Id).ToColumn("ImagePrime", caseSensitive: false);
        Map(x => x.ProductId).ToColumn("ImageProductId");
        Map(x => x.HtmlData).ToColumn("HtmlData");
        Map(x => x.ImageData).ToColumn("Image");
        Map(x => x.ImageContentType).ToColumn("ImageFileExt");
        Map(x => x.DateModified).ToColumn("DateModified");
    }
}