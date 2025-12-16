using MOSTComputers.Services.Currencies.Contracts;

namespace MOSTComputers.Services.Currencies;
public sealed class CurrencyVATService : ICurrencyVATService
{
    public CurrencyVATService(ICurrencyVATPercentageProvider currencyVATPercentageProvider)
    {
        _currencyVATPercentageProvider = currencyVATPercentageProvider;
    }

    private readonly ICurrencyVATPercentageProvider _currencyVATPercentageProvider;

    // Currently uses line-level rounding (for all VAT operations)

    public decimal CalculateVAT(decimal price, int itemQuantity, decimal valueAddedTaxPercentage)
    {
        return CalculateLineLevelVAT(price, itemQuantity, valueAddedTaxPercentage);
    }

    public decimal CalculateVATUsingDefaultRate(decimal price, int itemQuantity)
    {
        decimal valueAddedTaxPercentage = _currencyVATPercentageProvider.GetDefaultVATPercentage();

        return CalculateLineLevelVAT(price, itemQuantity, valueAddedTaxPercentage);
    }

    public decimal CalculateVATUsingDefaultRate(decimal price, int itemQuantity, ICurrencyVATPercentageProvider currencyVATPercentageProvider)
    {
        decimal valueAddedTaxPercentage = currencyVATPercentageProvider.GetDefaultVATPercentage();

        return CalculateLineLevelVAT(price, itemQuantity, valueAddedTaxPercentage);
    }

    public decimal AddVATToPrice(decimal price, int itemQuantity, decimal valueAddedTaxPercentage)
    {
        return AddLineLevelVATToPrice(price, itemQuantity, valueAddedTaxPercentage);
    }

    public decimal AddVATToPriceUsingDefaultRate(decimal price, int itemQuantity)
    {
        decimal valueAddedTaxPercentage = _currencyVATPercentageProvider.GetDefaultVATPercentage();

        return AddVATToPrice(price, itemQuantity, valueAddedTaxPercentage);
    }

    public decimal AddVATToPriceUsingDefaultRate(decimal price, int itemQuantity, ICurrencyVATPercentageProvider currencyVATPercentageProvider)
    {
        decimal valueAddedTaxPercentage = currencyVATPercentageProvider.GetDefaultVATPercentage();

        return AddVATToPrice(price, itemQuantity, valueAddedTaxPercentage);
    }

    public decimal CalculateNETPriceFromTotalPrice(decimal priceWithVAT, decimal valueAddedTaxPercentage)
    {
        return CalculateLineLevelNETPrice(priceWithVAT, valueAddedTaxPercentage);
    }

    public decimal CalculateNETPriceFromTotalPriceUsingDefaultRate(decimal priceWithVAT)
    {
        decimal valueAddedTaxPercentage = _currencyVATPercentageProvider.GetDefaultVATPercentage();

        return CalculateNETPriceFromTotalPrice(priceWithVAT, valueAddedTaxPercentage);
    }

    public decimal CalculateNETPriceFromTotalPriceUsingDefaultRate(decimal priceWithVAT, ICurrencyVATPercentageProvider currencyVATPercentageProvider)
    {
        decimal valueAddedTaxPercentage = currencyVATPercentageProvider.GetDefaultVATPercentage();

        return CalculateNETPriceFromTotalPrice(priceWithVAT, valueAddedTaxPercentage);
    }

    // D8 (line-level rounding)

    private static decimal CalculateLineLevelVAT(decimal unitPrice, int quantity, decimal vatRate)
    {
        decimal lineNet = unitPrice * quantity;
        decimal vatLine = RoundStandard(lineNet * vatRate);

        return vatLine;
    }

    private static decimal AddLineLevelVATToPrice(decimal unitPrice, int quantity, decimal vatRate)
    {
        decimal lineNet = unitPrice * quantity;
        decimal vatLine = RoundStandard(lineNet * vatRate);

        return lineNet + vatLine;
    }

    private static decimal CalculateLineLevelNETPrice(decimal grossLine, decimal vatRate)
    {
        decimal netLine = RoundStandard(grossLine / (1 + vatRate));

        return netLine;
    }

#pragma warning disable IDE0051
    // Reason: Added to be used if change is deemed necessary

    // D7 (unit-level rounding)

    private static decimal CalculateUnitLevelVAT(decimal unitPrice, int quantity, decimal vatRate)
    {
        decimal vatPerUnit = RoundStandard(unitPrice * vatRate);

        return vatPerUnit * quantity;
    }

    private static decimal AddUnitLevelVATToPrice(decimal unitPrice, int quantity, decimal vatRate)
    {
        decimal vatPerUnit = RoundStandard(unitPrice * vatRate);

        return (unitPrice + vatPerUnit) * quantity;
    }

    private static decimal CalculateUnitLevelNETPrice(decimal grossLine, int quantity, decimal vatRate)
    {
        decimal grossPerUnit = grossLine / quantity;

        decimal vatPerUnit = RoundStandard((grossPerUnit / (1 + vatRate)) * vatRate);

        decimal netPerUnit = grossPerUnit - vatPerUnit;

        return netPerUnit * quantity;
    }

#pragma warning restore IDE0051

    private static decimal RoundStandard(decimal value)
    {
        return Math.Round(value, 2, MidpointRounding.AwayFromZero);
    }
}