namespace MOSTComputers.Models.Product.Models;
public sealed class ExchangeRate
{
    public Currency CurrencyFrom { get; init; }
    public Currency CurrencyTo { get; init; }
    public decimal Rate { get; init; }
}