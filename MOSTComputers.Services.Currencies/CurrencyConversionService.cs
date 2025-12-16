using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.Currencies.Contracts;
using MOSTComputers.Services.Currencies.Models;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.Currencies;

public sealed class CurrencyConversionService : ICurrencyConversionService
{
    public CurrencyConversionService(IExchangeRateService exchangeRateService)
    {
        _exchangeRateService = exchangeRateService;
    }

    private readonly IExchangeRateService _exchangeRateService;

    public async Task<Dictionary<ChangeCurrencyRequest, OneOf<decimal, NotFound>>> ChangeCurrenciesAsync(List<ChangeCurrencyRequest> changeCurrencyRequest)
    {
        Dictionary<ChangeCurrencyRequest, OneOf<decimal, NotFound>> result = new();

        Dictionary<Tuple<Currency, Currency>, ExchangeRate?> exchangeRates = new();

        foreach (ChangeCurrencyRequest request in changeCurrencyRequest)
        {
            decimal priceInNewCurrency = request.Value;

            if (request.CurrentCurrency == request.NewCurrency)
            {
                result.Add(request, priceInNewCurrency);

                continue;
            }

            Tuple<Currency, Currency> key = new(request.CurrentCurrency, request.NewCurrency);

            bool exchangeRateExistsInDict = exchangeRates.ContainsKey(key);

            ExchangeRate? exchangeRate;

            if (exchangeRateExistsInDict)
            {
                exchangeRate = exchangeRates[key];
            }
            else
            {
                exchangeRate = await _exchangeRateService.GetForCurrenciesAsync(request.CurrentCurrency, request.NewCurrency);

                exchangeRates.Add(key, exchangeRate);
            }

            if (exchangeRate is null)
            {
                result.Add(request, new NotFound());

                continue;
            }

            priceInNewCurrency = Math.Round(priceInNewCurrency * exchangeRate.Rate, 2, MidpointRounding.AwayFromZero);

            result.Add(request, priceInNewCurrency);
        }

        return result;
    }

    public async Task<OneOf<decimal, NotFound>> ChangeCurrencyAsync(decimal value, Currency currentCurrency, Currency newCurrency)
    {
        if (currentCurrency == newCurrency) return value;

        ExchangeRate? exchangeRate = await _exchangeRateService.GetForCurrenciesAsync(currentCurrency, newCurrency);

        if (exchangeRate is null) return new NotFound();

        return ChangeCurrency(value, exchangeRate.Rate);
    }

    public decimal ChangeCurrency(decimal price, decimal rate)
    {
        decimal priceInNewCurrency = Math.Round(price * rate, 2, MidpointRounding.AwayFromZero);

        return priceInNewCurrency;
    }
}