using MOSTComputers.Services.Currencies.Contracts;

namespace MOSTComputers.Services.Currencies;
public sealed class CurrencyVATPercentageProvider : ICurrencyVATPercentageProvider
{
    private const decimal _defaultBulgarianVATPercentage = 0.2M;

    public decimal GetDefaultVATPercentage()
    {
        return _defaultBulgarianVATPercentage;
    }
}