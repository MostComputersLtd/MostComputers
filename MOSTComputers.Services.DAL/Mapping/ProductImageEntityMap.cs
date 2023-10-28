using Dapper.FluentMap.Mapping;
using MOSTComputers.Services.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOSTComputers.Services.DAL.Mapping;

internal class ProductImageEntityMap : EntityMap<ProductImage>
{
    public ProductImageEntityMap()
    {
        Map(x => x.Id).ToColumn("ID");
        Map(x => x.ProductId).ToColumn("CSTID");
        Map(x => x.XML).ToColumn("Description");
        Map(x => x.ImageData).ToColumn("Image");
        Map(x => x.ImageFileExtension).ToColumn("ImageFileExt");
        Map(x => x.DateModified).ToColumn("DateModified");
    }
}