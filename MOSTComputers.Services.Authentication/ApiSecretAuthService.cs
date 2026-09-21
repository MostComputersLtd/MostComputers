using System.Security.Cryptography;
using System.Text;
using System.Transactions;
using MOSTComputers.Services.Authentication.Contracts;
using MOSTComputers.Services.Authentication.Models;
using MOSTComputers.Services.Authentication.Models.Requests.ApiSecret;
using MOSTComputers.Services.Authentication.Models.Requests.ApiSecretPermission;
using OneOf;
using OneOf.Types;
using static MOSTComputers.Services.Authentication.CommonRules;

namespace MOSTComputers.Services.Authentication;

internal sealed class ApiSecretAuthService(
    IApiSecretService apiSecretService,
    IApiSecretPermissionService apiSecretPermissionService) : IApiSecretAuthService
{
    private const string _secretCharacters =
        "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_";

    private readonly IApiSecretService _apiSecretService = apiSecretService;
    private readonly IApiSecretPermissionService _apiSecretPermissionService = apiSecretPermissionService;

    public async Task<bool> SecretHasActivePermissionAsync(long secretId, ApiSecretPermissions permission)
    {
        return await _apiSecretPermissionService.SecretHasActivePermissionAsync(secretId, permission);
    }

    public async Task<OneOf<string, SecretForUserAlreadyExistsResult, DataAccessFailure>> CreateSecretForClientAsync(int clientId)
    {
        bool clientHasActiveSecret = await _apiSecretService.ClientHasActiveSecretAsync(clientId, DateTime.Now);

        if (clientHasActiveSecret)
        {
            return new SecretForUserAlreadyExistsResult();
        }

        string secret = RandomNumberGenerator.GetString(_secretCharacters, ApiSecretLength);

        byte[] secretBytes = Encoding.UTF8.GetBytes(secret);

        byte[] secretHash = SHA256.HashData(secretBytes);

        ApiSecretCreateRequest createRequest = new()
        {
            ClientId = clientId,
            SecretHash = secretHash,
            CreatedAt = DateTime.Now,
            ExpiresAt = null,
        };

        using TransactionScope transactionScope = new(TransactionScopeAsyncFlowOption.Enabled);

        OneOf<int, DataAccessFailure> success = await _apiSecretService.InsertAsync(createRequest);

        if (success.IsT1)
        {
            return success.AsT1;
        }

        ApiSecretPermissionCreateManyRequest createPermissionsRequest = new()
        {
            SecretId = success.AsT0,
            Permissions = [ApiSecretPermissions.ReadInvoices, ApiSecretPermissions.ReadWarrantyCards, ApiSecretPermissions.ReadOrders],
        };

        Console.WriteLine($"SecretId used for permissions: {success.AsT0}");

        OneOf<Success, DataAccessFailure> createPermissionsResult
            = await _apiSecretPermissionService.InsertManyAsync(createPermissionsRequest);

        return createPermissionsResult.Match<OneOf<string, SecretForUserAlreadyExistsResult, DataAccessFailure>>(
            success =>
            {
                transactionScope.Complete();

                return secret;
            },
            dataAccessFailure => dataAccessFailure);
    }
}
