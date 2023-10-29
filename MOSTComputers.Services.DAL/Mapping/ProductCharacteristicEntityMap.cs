using Dapper.FluentMap.Mapping;
using MOSTComputers.Services.DAL.Models;

namespace MOSTComputers.Services.Mapping;

internal sealed class ProductCharacteristicEntityMap : EntityMap<ProductCharacteristic>
{
    internal ProductCharacteristicEntityMap()
    {
        Map(x => x.Id).ToColumn("ProductKeywordID");
        Map(x => x.CategoryId).ToColumn("TID");
        Map(x => x.Name).ToColumn("Name");
        Map(x => x.Meaning).ToColumn("KeywordMeaning");
        Map(x => x.DisplayOrder).ToColumn("S");
        Map(x => x.Active).ToColumn("Active");
        Map(x => x.PKUserId).ToColumn("PKUserID");
        Map(x => x.LastUpdate).ToColumn("LastUpdate");
        Map(x => x.KWPrCh).ToColumn("KWPrCh");
    }
}