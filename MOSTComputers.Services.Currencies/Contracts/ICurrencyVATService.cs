namespace MOSTComputers.Services.Currencies.Contracts;

public interface ICurrencyVATService
{
    decimal AddVATToPrice(decimal price, int itemQuantity, decimal valueAddedTaxPercentage);
    decimal AddVATToPriceUsingDefaultRate(decimal price, int itemQuantity);
    decimal AddVATToPriceUsingDefaultRate(decimal price, int itemQuantity, ICurrencyVATPercentageProvider currencyVATPercentageProvider);
    decimal CalculateNETPriceFromTotalPrice(decimal priceWithVAT, decimal valueAddedTaxPercentage);
    decimal CalculateNETPriceFromTotalPriceUsingDefaultRate(decimal priceWithVAT);
    decimal CalculateNETPriceFromTotalPriceUsingDefaultRate(decimal priceWithVAT, ICurrencyVATPercentageProvider currencyVATPercentageProvider);
    decimal CalculateVAT(decimal price, int itemQuantity, decimal valueAddedTaxPercentage);
    decimal CalculateVATUsingDefaultRate(decimal price, int itemQuantity);
    decimal CalculateVATUsingDefaultRate(decimal price, int itemQuantity, ICurrencyVATPercentageProvider currencyVATPercentageProvider);
}