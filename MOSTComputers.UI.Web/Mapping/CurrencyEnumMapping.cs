using MOSTComputers.Services.DAL;
using MOSTComputers.Services.DAL.Models.Requests.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOSTComputers.UI.Web.Mapping;

internal static class CurrencyEnumMapping
{
    internal static string? GetStringFromCurrencyEnum(CurrencyEnum? currencyEnum)
    {
        if (currencyEnum is null) return null;

        return currencyEnum switch
        {
            CurrencyEnum.BGN => "BGN",
            CurrencyEnum.EUR => "EUR",
            CurrencyEnum.USD => "USD",
            _ => null
        };
    }

    internal static CurrencyEnum? GetCurrencyEnumFromString(string currencyString)
    {
        if (currencyString is null) return null;

        return currencyString switch
        {
            "BGN" => CurrencyEnum.BGN,
            "EUR" => CurrencyEnum.EUR,
            "USD" => CurrencyEnum.USD,
            _ => null
        };
    }
}