using System.Globalization;

namespace MOSTComputers.Services.PDF.Utils;
internal static class CurrencyBasicOperationUtils
{
    internal static string GetCurrencyStringFromPrice(
        decimal price,
        string currency,
        bool addWhiteSpaceBetweenPriceAndCurrency,
        CultureInfo? cultureInfo = null)
    {
        CultureInfo currentCulture = cultureInfo ?? CultureInfo.InvariantCulture;

        NumberFormatInfo numberFormatInfo = (NumberFormatInfo)currentCulture.NumberFormat.Clone();

        numberFormatInfo.CurrencySymbol = string.Empty;
        numberFormatInfo.CurrencyNegativePattern = 1;

        string output = price.ToString("C", numberFormatInfo);

        if (addWhiteSpaceBetweenPriceAndCurrency)
        {
            output += " ";
        }

        return output + currency;
    }

    internal static decimal RoundCurrency(decimal value)
    {
        return Math.Round(value, 2, MidpointRounding.AwayFromZero);
    }
}