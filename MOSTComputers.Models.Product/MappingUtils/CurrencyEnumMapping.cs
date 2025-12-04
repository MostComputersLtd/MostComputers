using MOSTComputers.Models.Product.Models;

namespace MOSTComputers.Models.Product.MappingUtils;
public static class CurrencyEnumMapping
{
    public static string? GetStringFromCurrencyEnum(Currency? currencyEnum)
    {
        if (currencyEnum is null) return null;

        return currencyEnum switch
        {
            Currency.BGN => "BGN",
            Currency.EUR => "EUR",
            Currency.USD => "USD",
            _ => null
        };
    }

    public static Currency? GetCurrencyEnumFromString(string currencyString)
    {
        if (currencyString is null) return null;

        return currencyString switch
        {
            "BGN" => Currency.BGN,
            "EUR" => Currency.EUR,
            "USD" => Currency.USD,
            _ => null
        };
    }
}