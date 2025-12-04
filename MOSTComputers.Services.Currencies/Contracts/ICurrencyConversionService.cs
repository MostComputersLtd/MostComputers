using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.Currencies.Models;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.Currencies.Contracts;
public interface ICurrencyConversionService
{
    Task<Dictionary<ChangeCurrencyRequest, OneOf<decimal, NotFound>>> ChangeCurrenciesAsync(List<ChangeCurrencyRequest> changeCurrencyRequest);
    Task<OneOf<decimal, NotFound>> ChangeCurrencyAsync(decimal value, Currency currentCurrency, Currency newCurrency);
}