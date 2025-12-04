using MOSTComputers.Models.Product.Models;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Contracts;
public interface IExchangeRateRepository
{
    Task<List<ExchangeRate>> GetAllAsync();
    Task<ExchangeRate?> GetForCurrenciesAsync(Currency currencyFrom, Currency currencyTo);
}