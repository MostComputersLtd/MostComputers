using Dapper.FluentMap.Mapping;
using MOSTComputers.Services.DAL.Models;

namespace MOSTComputers.Services.DAL.Mapping;

internal class ProductFirstImageEntityMap : EntityMap<ProductFirstImage>
{
    public ProductFirstImageEntityMap()
    {
        Map(x => x.Id).ToColumn("ID");
        Map(x => x.ProductId).Ignore();
        Map(x => x.XML).ToColumn("Description");
        Map(x => x.ImageData).ToColumn("Image");
        Map(x => x.ImageFileExtension).ToColumn("ImageFileExt");
        Map(x => x.DateModified).ToColumn("DateModified");
    }
}