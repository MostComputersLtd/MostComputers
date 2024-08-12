using Dapper.FluentMap.Mapping;
using MOSTComputers.Models.Product.Models;

namespace MOSTComputers.Services.DAL.Mapping;

internal sealed class ProductFirstImageEntityMap : EntityMap<ProductFirstImage>
{
    public ProductFirstImageEntityMap()
    {
        Map(x => x.Id).ToColumn("ImageProductId");
        Map(x => x.HtmlData).ToColumn("HtmlData");
        Map(x => x.ImageData).ToColumn("Image");
        Map(x => x.ImageContentType).ToColumn("ImageFileExt");
        Map(x => x.DateModified).ToColumn("DateModified");
    }
}