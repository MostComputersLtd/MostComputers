using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.UI.Web.Models.Authentication;
using OneOf;

namespace MOSTComputers.UI.Web.Services.Data.Accounts.Contracts;
public interface IUserDisplayDataService
{
    Task<OneOf<List<UserDisplayData>, UnexpectedFailureResult>> GetAllUsersAsync();
    Task<OneOf<UserDisplayData?, UnexpectedFailureResult>> GetUserAsync(string userId);
    Task<OneOf<List<UserDisplayData>, UnexpectedFailureResult>> GetUsersAsync(List<string> userIds);
}