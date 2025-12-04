using MOSTComputers.Models.Product.Models;

namespace MOSTComputers.Services.Currencies.Models;

public class ChangeCurrencyRequest
{
    public decimal Value { get; init; }
    public Currency CurrentCurrency { get; init; }
    public Currency NewCurrency { get; init; }
}