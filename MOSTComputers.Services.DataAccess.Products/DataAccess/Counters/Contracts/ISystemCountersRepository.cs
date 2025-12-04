using MOSTComputers.Models.Product.Models.Counters;
using MOSTComputers.Models.Product.Models.Validation;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Counters.Contracts;
public interface ISystemCountersRepository
{
    Task<SystemCounters?> GetSystemCountersAsync();
    Task<OneOf<Success, UnexpectedFailureResult>> UpdateAsync(SystemCounters systemCounters);
}