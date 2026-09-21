using Dapper;
using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Services.DataAccess.Common;
using MOSTComputers.Services.DataAccess.Documents.Models;
using MOSTComputers.Services.DataAccess.Documents.Configuration;
using static MOSTComputers.Services.DataAccess.Documents.Utils.TableAndColumnNameUtils;
using static MOSTComputers.Services.DataAccess.Documents.Utils.TableAndColumnNameUtils.OrdersTable;
using static MOSTComputers.Services.DataAccess.Documents.Utils.SqlGenerateUtils;
using Microsoft.Data.SqlClient;
using System.Data;
using MOSTComputers.Services.DataAccess.Documents.DataAccess.Contracts;
using MOSTComputers.Services.DataAccess.Documents.Models.Requests.Orders;

namespace MOSTComputers.Services.DataAccess.Documents.DataAccess;

internal sealed class OrderRepository(
    [FromKeyedServices(ConfigureServices.DocumentsDataAccessServiceKey)] IConnectionStringProvider connectionStringProvider)
    : IOrderRepository
{
    private readonly IConnectionStringProvider _connectionStringProvider = connectionStringProvider;

    const string _selectQueryBody =
        $"""
        SELECT orders.{IdColumn},
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
            {OrderItemsTable.QuantityColumn},
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
            {OrderItemsTable.InternalInfoColumn}

        FROM {OrdersTableName} orders
        LEFT JOIN {OrderItemsTableName} items
        ON orders.{IdColumn} = items.{OrderItemsTable.OrderIdColumn}
        """;

    private const string _orderNameParameterBase = "NameParameter";
    private const string _orderStatusParameterBase = "StatusParameter";
    private const string _orderBusinessIdParameterBase = "BusinessIdParameter";
    private const string _orderSearchStartTimeIdParameterBase = "SearchStartTimeParameter";

    public async Task<List<Order>> GetAllMatchingAsync(OrderSearchRequest orderSearchRequest)
    {
        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        (string filter, DynamicParameters parameters) = GetQueryFilterFromRequest(orderSearchRequest);

        string query =
            $"""
            {_selectQueryBody}
            WHERE {filter}
            """;

        List<Order> orders = new();

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
            splitOn: $"{OrderItemsTable.IdColumn}",
            commandType: CommandType.Text);

        return orders;
    }

    public async Task<Order?> GetByIdAsync(int id)
    {
        const string query =
            $"""
            {_selectQueryBody}
            WHERE orders.{IdColumn} = @id;
            """;

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        var parameters = new
        {
            id = id,
        };

        Order? existingOrder = null;

        await dbConnection.QueryAsync<Order, OrderItem, Order>(
            query,
            (order, orderItem) =>
            {
                existingOrder ??= order;

                if (orderItem != null)
                {
                    existingOrder.Items.Add(orderItem);
                }

                return order;
            },
            parameters,
            splitOn: $"{OrderItemsTable.IdColumn}",
            commandType: CommandType.Text);

        return existingOrder;
    }

    private static (string filter, DynamicParameters parameters) GetQueryFilterFromRequest(OrderSearchRequest orderSearchRequest)
    {
        DynamicParameters parameters = new();

        List<string> whereClauses = new();

        if (orderSearchRequest.SearchByNameString != null)
        {
            parameters.Add(_orderNameParameterBase, orderSearchRequest.SearchByNameString, dbType: DbType.String);
            whereClauses.Add($"CHARINDEX(@{_orderNameParameterBase}, {OrderNameColumn}) > 0\n");
        }

        if (orderSearchRequest.Status != null)
        {
            parameters.Add(_orderStatusParameterBase, orderSearchRequest.Status, dbType: DbType.Byte);
            whereClauses.Add($"{StatusColumn} = @{_orderStatusParameterBase}\n");
        }

        if (orderSearchRequest.CustomerId != null)
        {
            parameters.Add(_orderBusinessIdParameterBase, orderSearchRequest.CustomerId, dbType: DbType.Int32);
            whereClauses.Add($"{BIDColumn} = @{_orderBusinessIdParameterBase}\n");
        }

        if (orderSearchRequest.SearchStartDateTime != null)
        {
            parameters.Add(_orderSearchStartTimeIdParameterBase, orderSearchRequest.SearchStartDateTime, dbType: DbType.DateTime);
            whereClauses.Add($"{OrderDateColumn} >= @{_orderSearchStartTimeIdParameterBase}\n");
        }

        string filter = JoinConditionalStatements(whereClauses, ConditionalStatementJoinType.And);

        return (filter, parameters);
    }
}
