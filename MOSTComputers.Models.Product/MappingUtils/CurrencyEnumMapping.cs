using MOSTComputers.Models.Product.Models;

namespace MOSTComputers.Models.Product.MappingUtils;

public static class CurrencyEnumMapping
{
    public static string? GetStringFromCurrencyEnum(CurrencyEnum? currencyEnum)
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

    public static CurrencyEnum? GetCurrencyEnumFromString(string currencyString)
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