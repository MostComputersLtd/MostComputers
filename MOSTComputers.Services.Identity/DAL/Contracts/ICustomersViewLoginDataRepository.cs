using MOSTComputers.Services.Identity.Models.Customers;

namespace MOSTComputers.Services.Identity.DAL.Contracts;
public interface ICustomersViewLoginDataRepository
{
    Task<CustomerData?> GetByIdAsync(int id);
    Task<CustomerData?> GetByUsernameAsync(string username);
    Task<CustomerLoginData?> GetLoginDataByIdAsync(int id);
    Task<CustomerLoginData?> GetLoginDataByUsernameAsync(string username);
    Task<CheckPasswordResult> IsPasswordEqualToExistingAsync(int id, string password);
    Task<CheckPasswordResult> IsPasswordEqualToExistingAsync(string username, string password);
}