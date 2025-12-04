using Dapper.FluentMap.Mapping;
using MOSTComputers.Services.DataAccess.Products.Models.DAOs.ProductIdentifiers;

using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils.ProductGTINCodesTable;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Mapping.ProductIdentifiers;
internal sealed class ProductGTINCodeDAOEntityMap : EntityMap<ProductGTINCodeDAO>
{
    public ProductGTINCodeDAOEntityMap()
    {
        Map(x => x.ProductId).ToColumn(ProductIdColumnName);
        Map(x => x.CodeType).ToColumn(CodeTypeColumnName);
        Map(x => x.CodeTypeAsText).ToColumn(CodeTypeAsTextColumnName);
        Map(x => x.Value).ToColumn(ValueColumnName);

        Map(x => x.CreateUserName).ToColumn(CreateUserNameColumnName);
        Map(x => x.CreateDate).ToColumn(CreateDateColumnName);
        Map(x => x.LastUpdateUserName).ToColumn(LastUpdateUserNameColumnName);
        Map(x => x.LastUpdateDate).ToColumn(LastUpdateDateColumnName);
    }
}