namespace MOSTComputers.Services.Currencies.Contracts;

public interface ICurrencyVATPercentageProvider
{
    decimal GetDefaultVATPercentage();
}