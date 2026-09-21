using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Services.Authentication.Configuration;
using MOSTComputers.Services.Authentication.DataAccess.Contracts;
using MOSTComputers.Services.Authentication.Models;
using MOSTComputers.Services.Authentication.Models.Requests.ApiSecret;
using MOSTComputers.Services.DataAccess.Common;
using OneOf;
using static MOSTComputers.Services.Authentication.DataAccess.TableAndColumnNameUtils;
using static MOSTComputers.Services.Authentication.DataAccess.TableAndColumnNameUtils.ApiSecretsTable;

namespace MOSTComputers.Services.Authentication.DataAccess;

internal sealed class ApiSecretRepository : IApiSecretRepository
{
    public ApiSecretRepository(
        [FromKeyedServices(ConfigureServices.LocalDBConnectionStringProviderServiceKey)] IConnectionStringProvider connectionStringProvider)
    {
        _connectionStringProvider = connectionStringProvider;
    }

    private readonly IConnectionStringProvider _connectionStringProvider;

    public async Task<bool> ClientHasActiveSecretAsync(int clientId, DateTime now)
    {
        const string query =
            $"""
            SELECT CASE
            WHEN EXISTS (
                SELECT 1 FROM {ApiSecretsTableName}
                WHERE {ClientIdColumn} = @clientId
                AND {RevokedAtColumn} IS NULL
                AND ({ExpiresAtColumn} IS NULL OR {ExpiresAtColumn} > @Now))
                THEN 1
                ELSE 0
            END
            """;

        using SqlConnection sqlConnection = new(_connectionStringProvider.ConnectionString);

        var parameters = new
        {
            clientId = clientId,
            Now = now,
        };

        int exists = await sqlConnection.QuerySingleOrDefaultAsync<int>(
            query, parameters, commandType: CommandType.Text);

        return exists == 1;
    }

    public async Task<ApiSecret?> GetByHashAsync(byte[] secretHash)
    {
        const string query =
            $"""
            SELECT TOP 1 {IdColumn},
                {ClientIdColumn},
                {SecretHashColumn},
                {CreatedAtColumn},
                {ExpiresAtColumn},
                {RevokedAtColumn},
                {LastUsedAtColumn}
            FROM {ApiSecretsTableName}
            WHERE {SecretHashColumn} = @secretHash;
            """;

        using SqlConnection sqlConnection = new(_connectionStringProvider.ConnectionString);

        var parameters = new
        {
            secretHash = secretHash,
        };

        ApiSecret? apiSecret = await sqlConnection.QuerySingleOrDefaultAsync<ApiSecret>(
            query, parameters, commandType: CommandType.Text);

        return apiSecret;
    }

    public async Task<OneOf<int, DataAccessFailure>> InsertAsync(ApiSecretCreateRequest createRequest)
    {
        const string query =
            $"""
            DECLARE @InsertedIdTable TABLE (Id INT);

            INSERT INTO {ApiSecretsTableName} (
                {ClientIdColumn},
                {SecretHashColumn},
                {CreatedAtColumn},
                {ExpiresAtColumn})
            OUTPUT INSERTED.Id INTO @InsertedIdTable
            VALUES (@clientId, @SecretHash, @CreatedAt, @ExpiresAt);

            SELECT TOP 1 Id FROM @InsertedIdTable;
            """;

        using SqlConnection sqlConnection = new(_connectionStringProvider.ConnectionString);

        var parameters = new
        {
            clientId = createRequest.ClientId,
            SecretHash = createRequest.SecretHash,
            CreatedAt = createRequest.CreatedAt,
            ExpiresAt = createRequest.ExpiresAt,
        };

        int id = await sqlConnection.QuerySingleOrDefaultAsync<int>(query, parameters, commandType: CommandType.Text);

        return id > 0 ? id : new DataAccessFailure();
    }
}
