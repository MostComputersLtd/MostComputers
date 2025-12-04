using Dapper.FluentMap.Mapping;
using MOSTComputers.Models.Product.Models;

using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils.ProductCharacteristicsTable;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Mapping;
internal sealed class ProductCharacteristicEntityMap : EntityMap<ProductCharacteristic>
{
    internal ProductCharacteristicEntityMap()
    {
        Map(x => x.Id).ToColumn(IdColumnName);
        Map(x => x.CategoryId).ToColumn(CategoryIdColumnName);
        Map(x => x.Name).ToColumn(NameColumnName);
        Map(x => x.Meaning).ToColumn(MeaningColumnName);
        Map(x => x.DisplayOrder).ToColumn(DisplayOrderColumnName);
        Map(x => x.Active).ToColumn(ActiveColumnName);
        Map(x => x.PKUserId).ToColumn(PKUserIdColumnName);
        Map(x => x.LastUpdate).ToColumn(LastUpdateColumnName);
        Map(x => x.KWPrCh).ToColumn(KWPrChColumnName);
    }
}