using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Services.Authentication.Models.Requests.ApiToken;
using MOSTComputers.Services.DataAccess.Common;
using static MOSTComputers.Services.Authentication.DataAccess.TableAndColumnNameUtils;
using static MOSTComputers.Services.Authentication.DataAccess.TableAndColumnNameUtils.ApiTokensTable;
using MOSTComputers.Services.Authentication.Configuration;
using MOSTComputers.Services.Authentication.DataAccess.Contracts;
using MOSTComputers.Services.Authentication.Models;

namespace MOSTComputers.Services.Authentication.DataAccess;

internal sealed class ApiTokenRepository : IApiTokenRepository
{
    public ApiTokenRepository(
        [FromKeyedServices(ConfigureServices.LocalDBConnectionStringProviderServiceKey)] IConnectionStringProvider connectionStringProvider)
    {
        _connectionStringProvider = connectionStringProvider;
    }

    private readonly IConnectionStringProvider _connectionStringProvider;

    public async Task<ApiToken?> GetByHashAsync(byte[] tokenHash)
    {
        const string query =
            $"""
            SELECT TOP 1 {IdColumn},
                {SecretIdColumn},
                {ClientIdColumn},
                {TokenHashColumn},
                {CreatedAtColumn},
                {ExpiresAtColumn},
                {RevokedAtColumn},
                {LastUsedAtColumn}
            FROM {ApiTokensTableName}
            WHERE {TokenHashColumn} = @tokenHash;
            """;

        using SqlConnection sqlConnection = new (_connectionStringProvider.ConnectionString);

        var parameters = new
        {
            tokenHash = tokenHash,
        };

        ApiToken? apiToken = await sqlConnection.QuerySingleOrDefaultAsync<ApiToken>(
            query, parameters, commandType: CommandType.Text);

        return apiToken;
    }

    public async Task<bool> InsertAsync(ApiTokenCreateRequest createRequest)
    {
        const string query =
            $"""
            INSERT INTO {ApiTokensTableName} (
                {SecretIdColumn},
                {ClientIdColumn},
                {TokenHashColumn},
                {CreatedAtColumn},
                {ExpiresAtColumn})

            VALUES (@secretId, @clientId, @TokenHash, @CreatedAt, @ExpiresAt);
            """;

        using SqlConnection sqlConnection = new (_connectionStringProvider.ConnectionString);

        var parameters = new
        {
            secretId = createRequest.SecretId,
            clientId = createRequest.ClientId,
            TokenHash = createRequest.TokenHash,
            CreatedAt = createRequest.CreatedAt,
            ExpiresAt = createRequest.ExpiresAt,
        };

        int rowsAffected = await sqlConnection.ExecuteAsync(query, parameters, commandType: CommandType.Text);

        return rowsAffected > 0;
    }
}
