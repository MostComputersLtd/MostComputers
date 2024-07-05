using Dapper.FluentMap.Mapping;
using MOSTComputers.Models.Product.Models;

namespace MOSTComputers.Services.DAL.Mapping;

internal sealed class ProductFirstImageEntityMap : EntityMap<ProductFirstImage>
{
    public ProductFirstImageEntityMap()
    {
        Map(x => x.Id).ToColumn("ImageProductId");
        Map(x => x.HtmlData).ToColumn("XMLData");
        Map(x => x.ImageData).ToColumn("Image");
        Map(x => x.ImageFileExtension).ToColumn("ImageFileExt");
        Map(x => x.DateModified).ToColumn("DateModified");
    }
}