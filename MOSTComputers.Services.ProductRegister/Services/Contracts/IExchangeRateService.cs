using MOSTComputers.Models.Product.Models;

namespace MOSTComputers.Services.ProductRegister.Services.Contracts;
public interface IExchangeRateService
{
    Task<List<ExchangeRate>> GetAllAsync();
    Task<ExchangeRate?> GetForCurrenciesAsync(Currency currencyFrom, Currency currencyTo);
}