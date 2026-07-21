using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Services.Orders.Services;

namespace MOSTComputers.Services.Orders.Configuration;

public static class ConfigureServices
{
	public static IServiceCollection AddOrderServices(this IServiceCollection services)
	{
		services.AddScoped<IOrdersService, OrdersService>();

		return services;
	}
}
