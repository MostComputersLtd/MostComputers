using Dapper;
using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Services.DataAccess.Common;
using MOSTComputers.Services.DataAccess.Documents.Models;
using MOSTComputers.Services.DataAccess.Documents.Configuration;
using static MOSTComputers.Services.DataAccess.Documents.Utils.TableAndColumnNameUtils;
using static MOSTComputers.Services.DataAccess.Documents.Utils.TableAndColumnNameUtils.OrdersTable;
using Microsoft.Data.SqlClient;
using System.Data;
using MOSTComputers.Services.DataAccess.Documents.DataAccess.Contracts;

namespace MOSTComputers.Services.DataAccess.Documents.DataAccess;

internal sealed class OrderRepository(
	[FromKeyedServices(ConfigureServices.DocumentsDataAccessServiceKey)] IConnectionStringProvider connectionStringProvider)
	: IOrderRepository
{
	private readonly IConnectionStringProvider _connectionStringProvider = connectionStringProvider;

	const string _selectQueryBody =
		$"""
		SELECT {IdColumn},
			{StatusColumn},
			{AutoReplyColumn},
			{QuoteIdColumn},
			{UserIdColumn},
			{OrderDateColumn},
			{OrderNameColumn},
			{ReadTimeColumn},
			{ReadUserIdColumn},
			{BIDColumn},
			{DealIdColumn},
			{MOST3DealIdColumn},
			{IsConfigurationColumn},
			{CfgProfitColumn},
			{CurrencyColumn},
			{OriginalInternetOrderIdColumn},
			{RepliedInternetOrderIdColumn},
			{InfoColumn},
			{OrderItemsTable.IdColumn},
			items.{OrderItemsTable.OrderIdColumn} AS {OrderItemsTable.OrderIdColumnAlias},
			{OrderItemsTable.ProductIdColumn},
			{OrderItemsTable.PriceColumn},
			{OrderItemsTable.AdditionalWarrantyColumn},
			{OrderItemsTable.FDDCColumn},
			{OrderItemsTable.PriceGroupColumn},
			{OrderItemsTable.ProfitPercentColumn},
			{OrderItemsTable.PromotionPIDColumn},
			{OrderItemsTable.PromotionRIDColumn},
			{OrderItemsTable.PromotionPAmountColumn},
			{OrderItemsTable.PromotionRAmountColumn},
			{OrderItemsTable.STInfoColumn},
			{OrderItemsTable.STInfoWColumn},
			{OrderItemsTable.ExternalInfoColumn},
			{OrderItemsTable.InternalInfoColumn},

		FROM {OrdersTableName} orders
		LEFT JOIN {OrderItemsTableName} items
		ON orders.{IdColumn} = items.{OrderItemsTable.OrderIdColumn}
		""";

	public async Task<List<Order>> GetAllForUserAsync(int userId)
	{
		const string query =
			$"""
			{_selectQueryBody}
			WHERE {UserIdColumn} = @userId;
			""";

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

		List<Order> orders = new();

		var parameters = new
		{
			userId = userId,
		};

        IEnumerable<Order> output = await dbConnection.QueryAsync<Order, OrderItem, Order>(
				query,
				(order, orderItem) =>
				{
					Order? existingOrder = orders.Find(x => x.Id == order.Id);

					if (existingOrder == null)
					{
						orders.Add(order);

						existingOrder = order;
					}

					if (orderItem != null)
					{
						existingOrder.Items.Add(orderItem);
					}

					return order;
				},
				parameters,
				commandType: CommandType.Text);

		return orders;
	}
}
