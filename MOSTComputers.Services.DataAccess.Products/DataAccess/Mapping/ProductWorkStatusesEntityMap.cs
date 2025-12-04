using Dapper.FluentMap.Mapping;
using MOSTComputers.Models.Product.Models.ProductStatuses;

using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils.ProductWorkStatusesTable;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Mapping;
internal sealed class ProductWorkStatusesEntityMap : EntityMap<ProductWorkStatuses>
{
    public ProductWorkStatusesEntityMap()
    {
        Map(x => x.Id).ToColumn(IdColumnAlias);
        Map(x => x.ProductId).ToColumn(ProductIdColumnAlias);
        Map(x => x.ProductNewStatus).ToColumn(ProductNewStatusColumnName);
        Map(x => x.ProductXmlStatus).ToColumn(ProductXmlStatusColumnName);
        Map(x => x.ReadyForImageInsert).ToColumn(ReadyForImageInsertColumnName);

        Map(x => x.CreateUserName).ToColumn(CreateUserNameColumnName);
        Map(x => x.CreateDate).ToColumn(CreateDateColumnName);
        Map(x => x.LastUpdateUserName).ToColumn(LastUpdateUserNameColumnName);
        Map(x => x.LastUpdateDate).ToColumn(LastUpdateDateColumnName);
    }
}