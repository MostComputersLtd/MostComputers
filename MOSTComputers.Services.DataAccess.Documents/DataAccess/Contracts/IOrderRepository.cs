using MOSTComputers.Services.DataAccess.Documents.Models;
using MOSTComputers.Services.DataAccess.Documents.Models.Requests.Orders;

namespace MOSTComputers.Services.DataAccess.Documents.DataAccess.Contracts;

public interface IOrderRepository
{
    Task<List<Order>> GetAllMatchingAsync(OrderSearchRequest orderSearchRequest);
    Task<Order?> GetByIdAsync(int id);
}
