using Dapper.FluentMap.Mapping;
using MOSTComputers.Models.Product.Models;
using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils.ExchangeRatesTable;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Mapping;
internal sealed class ExchangeRateEntityMap : EntityMap<ExchangeRate>
{
    public ExchangeRateEntityMap()
    {
        Map(x => x.CurrencyFrom).ToColumn(CurrencyFromIdColumnName);
        Map(x => x.CurrencyTo).ToColumn(CurrencyToIdColumnName);
        Map(x => x.Rate).ToColumn(RateColumnName);
    }
}