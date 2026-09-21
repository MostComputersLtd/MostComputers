using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Services.Authentication.Configuration;
using MOSTComputers.Services.Authentication.DataAccess.Contracts;
using MOSTComputers.Services.Authentication.Models;
using MOSTComputers.Services.Authentication.Models.Requests.ApiSecretPermission;
using MOSTComputers.Services.DataAccess.Common;
using static MOSTComputers.Services.Authentication.DataAccess.TableAndColumnNameUtils;
using static MOSTComputers.Services.Authentication.DataAccess.TableAndColumnNameUtils.ApiSecretPermissionsTable;

namespace MOSTComputers.Services.Authentication.DataAccess;

internal sealed class ApiSecretPermissionRepository : IApiSecretPermissionRepository
{
    public ApiSecretPermissionRepository(
        [FromKeyedServices(ConfigureServices.LocalDBConnectionStringProviderServiceKey)] IConnectionStringProvider connectionStringProvider)
    {
        _connectionStringProvider = connectionStringProvider;
    }

    private readonly IConnectionStringProvider _connectionStringProvider;

    public async Task<bool> SecretHasActivePermissionAsync(long secretId, ApiSecretPermissions permission)
    {
        const string query =
            $"""
            SELECT CASE WHEN EXISTS (
                SELECT 1 FROM {ApiSecretsTableName}
                WHERE {SecretIdColumn} = @secretId
                AND {ValueColumn} = @Value
                THEN 1
                ELSE 0
            END
            """;

        using SqlConnection sqlConnection = new(_connectionStringProvider.ConnectionString);

        var parameters = new
        {
            secretId = secretId,
            Value = (int)permission,
        };

        int exists = await sqlConnection.QuerySingleOrDefaultAsync<int>(
            query, parameters, commandType: CommandType.Text);

        return exists == 1;
    }

    public async Task<bool> InsertManyAsync(ApiSecretPermissionCreateManyRequest createRequest)
    {
        const string queryBase =
            $"""
            INSERT INTO {ApiSecretPermissionsTableName} (
                {SecretIdColumn},
                {ValueColumn})
            """;

        List<string> insertParameters = new();

        var parameters = new DynamicParameters();

        parameters.Add("secretId", createRequest.SecretId, DbType.Int64);

        for (int i = 0; i < createRequest.Permissions.Length; i++)
        {
            ApiSecretPermissions permission = createRequest.Permissions[i];

            string queryValues = $"(@secretId, @Permission{i})";

            parameters.Add($"Permission{i}", permission);

            insertParameters.Add(queryValues);
        }

        string values = string.Join(", ", insertParameters);

        string query =
            $"""
            {queryBase}
            VALUES {values}
            """;

            Console.WriteLine(query);

        using SqlConnection sqlConnection = new(_connectionStringProvider.ConnectionString);

        int rowsAffected = await sqlConnection.ExecuteAsync(query, parameters, commandType: CommandType.Text);

        return rowsAffected > 0;
    }
}
