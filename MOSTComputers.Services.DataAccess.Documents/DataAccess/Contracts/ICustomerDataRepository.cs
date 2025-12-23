using MOSTComputers.Services.DataAccess.Documents.Models;

namespace MOSTComputers.Services.DataAccess.Documents.DataAccess.Contracts;
public interface ICustomerDataRepository
{
    Task<List<CustomerData>> GetAllAsync();
    Task<CustomerData?> GetByIdAsync(int id);
    Task<List<CustomerData>> GetByIdsAsync(List<int> ids);
}