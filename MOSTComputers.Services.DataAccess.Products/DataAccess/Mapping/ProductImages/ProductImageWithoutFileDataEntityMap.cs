using Dapper.FluentMap.Mapping;
using MOSTComputers.Models.Product.Models.ProductImages;
using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils.AllImagesTable;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Mapping.ProductImages;
internal sealed class ProductImageWithoutFileDataEntityMap : EntityMap<ProductImageData>
{
    public ProductImageWithoutFileDataEntityMap()
    {
        Map(x => x.Id).ToColumn(IdColumnAlias, caseSensitive: false);
        Map(x => x.ProductId).ToColumn(ProductIdColumnAlias);
        Map(x => x.ImageContentType).ToColumn(ImageContentTypeColumnName);
        Map(x => x.HtmlData).ToColumn(DescriptionColumnAlias);
        Map(x => x.DateModified).ToColumn(DateModifiedColumnName);
    }
}