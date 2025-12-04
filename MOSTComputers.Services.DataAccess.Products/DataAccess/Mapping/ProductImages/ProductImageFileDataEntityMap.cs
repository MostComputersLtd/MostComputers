using Dapper.FluentMap.Mapping;
using MOSTComputers.Models.Product.Models.ProductImages;
using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils.ImageFileNamesTable;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Mapping.ProductImages;
internal sealed class ProductImageFileDataEntityMap : EntityMap<ProductImageFileData>
{
    public ProductImageFileDataEntityMap()
    {
        Map(x => x.ProductId).ToColumn(ProductIdColumnName);
        Map(x => x.Id).ToColumn(IdColumnName);
        Map(x => x.ImageId).ToColumn(ImageIdColumnName);
        Map(x => x.FileName).ToColumn(FileNameColumnName);
        Map(x => x.DisplayOrder).ToColumn(DisplayOrderColumnName);
        Map(x => x.Active).ToColumn(ActiveColumnName);

        Map(x => x.CreateUserName).ToColumn(CreateUserNameColumnName);
        Map(x => x.CreateDate).ToColumn(CreateDateColumnName);
        Map(x => x.LastUpdateUserName).ToColumn(LastUpdateUserNameColumnName);
        Map(x => x.LastUpdateDate).ToColumn(LastUpdateDateColumnName);
    }
}