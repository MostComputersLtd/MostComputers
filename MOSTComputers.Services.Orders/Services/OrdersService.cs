using MOSTComputers.Services.DataAccess.Documents.DataAccess.Contracts;
using MOSTComputers.Services.DataAccess.Documents.Models;
using MOSTComputers.Services.DataAccess.Documents.Models.Requests.Orders;

namespace MOSTComputers.Services.Orders.Services;

internal sealed class OrdersService(IOrderRepository orderRepository) : IOrdersService
{
    private readonly IOrderRepository _orderRepository = orderRepository;

    public async Task<List<Order>> GetAllMatchingAsync(OrderSearchRequest orderSearchRequest)
    {
        if (orderSearchRequest.CustomerId < 0
            || orderSearchRequest.Status < 0)
        {
            return new();
        }

        if (orderSearchRequest.SearchByNameString != null)
        {
            orderSearchRequest.SearchByNameString = orderSearchRequest.SearchByNameString.Trim();
        }

        return await _orderRepository.GetAllMatchingAsync(orderSearchRequest);
    }

    public Task<Order?> GetByIdAsync(int id)
    {
        return _orderRepository.GetByIdAsync(id);
    }
}
