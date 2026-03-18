using Dapper.FluentMap.Mapping;
using MOSTComputers.Models.Product.Models;
using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils.ProductDocumentsTable;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Mapping;
internal sealed class ProductDocumentEntityMap : EntityMap<ProductDocument>
{
    public ProductDocumentEntityMap()
    {
        Map(x => x.Id).ToColumn(IdColumnName);
        Map(x => x.ProductId).ToColumn(ProductIdColumnName);
        Map(x => x.FileName).ToColumn(FileNameColumnName);
        Map(x => x.Description).ToColumn(DescriptionColumnName);
    }
}