using MOSTComputers.Services.DataAccess.Documents.Models;

namespace MOSTComputers.Services.DataAccess.Documents.DataAccess.Contracts;

public interface IOrderRepository
{

	Task<List<Order>> GetAllForUserAsync(int userId);
}
