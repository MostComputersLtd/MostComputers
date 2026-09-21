using MOSTComputers.Services.DataAccess.Documents.Models;
using MOSTComputers.Services.DataAccess.Documents.Models.Requests.Orders;

namespace MOSTComputers.Services.Orders.Services;

public interface IOrdersService
{
    Task<List<Order>> GetAllMatchingAsync(OrderSearchRequest orderSearchRequest);
    Task<Order?> GetByIdAsync(int id);
}
