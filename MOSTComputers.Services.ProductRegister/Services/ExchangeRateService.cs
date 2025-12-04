using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Contracts;

namespace MOSTComputers.Services.ProductRegister.Services;
internal class ExchangeRateService : IExchangeRateService
{
    public ExchangeRateService(IExchangeRateRepository exchangeRateRepository)
    {
        _exchangeRateRepository = exchangeRateRepository;
    }

    private readonly IExchangeRateRepository _exchangeRateRepository;

    public async Task<List<ExchangeRate>> GetAllAsync()
    {
        return await _exchangeRateRepository.GetAllAsync();
    }

    public async Task<ExchangeRate?> GetForCurrenciesAsync(Currency currencyFrom, Currency currencyTo)
    {
        return await _exchangeRateRepository.GetForCurrenciesAsync(currencyFrom, currencyTo);
    }
}