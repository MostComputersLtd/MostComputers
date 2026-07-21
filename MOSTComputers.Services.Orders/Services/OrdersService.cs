using MOSTComputers.Services.DataAccess.Documents.DataAccess.Contracts;
using MOSTComputers.Services.DataAccess.Documents.Models;

namespace MOSTComputers.Services.Orders.Services;

internal sealed class OrdersService(IOrderRepository orderRepository) : IOrdersService
{
	private readonly IOrderRepository _orderRepository = orderRepository;

	public Task<List<Order>> GetAllForUserAsync(int userId)
	{
		return _orderRepository.GetAllForUserAsync(userId);
	}
}
