using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Models.Product.Models.ProductIdentifiers;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Common;
using MOSTComputers.Services.DataAccess.Products.Configuration;
using MOSTComputers.Services.DataAccess.Products.DataAccess.ProductIdentifiers.Contracts;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.ProductIdentifiers.ProductSerialNumber;
using OneOf;
using OneOf.Types;
using System.Data;
using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils;
using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils.ProductSerialNumbersTable;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.ProductIdentifiers;
internal sealed class ProductSerialNumberRepository : IProductSerialNumberRepository
{
    public ProductSerialNumberRepository(
        [FromKeyedServices(ConfigureServices.ConnectionStringProviderServiceKey)] IConnectionStringProvider connectionStringProvider)
    {
        _connectionStringProvider = connectionStringProvider;
    }

    private readonly IConnectionStringProvider _connectionStringProvider;

    public async Task<List<IGrouping<int, ProductSerialNumber>>> GetAllForProductsAsync(List<int> productIds)
    {
        const string query =
            $"""
            SELECT {ProductIdColumnName}, {SerialNumberColumnName}
            FROM {ProductSerialNumbersTableName}
            WHERE {ProductIdColumnName} IN @productIds;
            """;

        using SqlConnection connection = new(_connectionStringProvider.ConnectionString);

        var parameters = new
        {
            productIds = productIds
        };

        IEnumerable<ProductSerialNumber> productSerialNumbers = await connection.QueryAsync<ProductSerialNumber>(
            query, parameters, commandType: CommandType.Text);

        return productSerialNumbers
            .GroupBy(productSerialNumber => productSerialNumber.ProductId)
            .ToList();
    }

    public async Task<List<ProductSerialNumber>> GetAllForProductAsync(int productId)
    {
        const string query =
            $"""
            SELECT {ProductIdColumnName}, {SerialNumberColumnName}
            FROM {ProductSerialNumbersTableName}
            WHERE {ProductIdColumnName} = @productId;
            """;

        using SqlConnection connection = new(_connectionStringProvider.ConnectionString);

        var parameters = new
        {
            productIds = productId
        };

        IEnumerable<ProductSerialNumber> productSerialNumbers = await connection.QueryAsync<ProductSerialNumber>(
            query, parameters, commandType: CommandType.Text);

        return productSerialNumbers.ToList();
    }

    public async Task<OneOf<Success, UnexpectedFailureResult>> InsertAsync(ProductSerialNumberCreateRequest createRequest)
    {
        const string insertQuery =
            $"""
            INSERT INTO {ProductSerialNumbersTableName} ({ProductIdColumnName}, {SerialNumberColumnName})
            VALUES (@productId, @SerialNumber);
            """;

        using SqlConnection connection = new(_connectionStringProvider.ConnectionString);

        var parameters = new
        {
            productId = createRequest.ProductId,
            SerialNumber = createRequest.SerialNumber
        };

        int rowsAffected = await connection.ExecuteAsync(insertQuery, parameters, commandType: CommandType.Text);

        return rowsAffected > 0 ? new Success() : new UnexpectedFailureResult();
    }

    public async Task<OneOf<Success, NotFound>> DeleteAsync(int productId, string serialNumber)
    {
        const string deleteQuery =
            $"""
            DELETE FROM {ProductSerialNumbersTableName}

            WHERE {ProductIdColumnName} = @productId
            AND {SerialNumberColumnName} = @SerialNumber
            """;

        var parameters = new
        {
            productId = productId,
            SerialNumber = serialNumber,
        };

        using SqlConnection sqlConnection = new(_connectionStringProvider.ConnectionString);

        int rowsAffected = await sqlConnection.ExecuteAsync(deleteQuery, parameters, commandType: CommandType.Text);

        return rowsAffected > 0 ? new Success() : new NotFound();
    }
}