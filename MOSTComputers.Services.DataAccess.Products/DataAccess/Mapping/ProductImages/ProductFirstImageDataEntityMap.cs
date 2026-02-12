using Dapper.FluentMap.Mapping;
using MOSTComputers.Services.DataAccess.Products.Models.DAOs.ProductImage;

using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils.FirstImagesTable;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Mapping.ProductImages;

internal sealed class ProductFirstImageDataEntityMap : EntityMap<ProductFirstImageDataDAO>
{
    public ProductFirstImageDataEntityMap()
    {
        Map(x => x.Id).ToColumn(IdColumnAlias);
        Map(x => x.ImageContentType).ToColumn(ImageContentTypeColumnName);
        Map(x => x.DateModified).ToColumn(DateModifiedColumnName);
        Map(x => x.HtmlData).ToColumn(DescriptionColumnAlias);
    }
}