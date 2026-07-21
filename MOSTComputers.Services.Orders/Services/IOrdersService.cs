using MOSTComputers.Services.DataAccess.Documents.Models;

namespace MOSTComputers.Services.Orders.Services;

public interface IOrdersService
{
	Task<List<Order>> GetAllForUserAsync(int userId);
}
