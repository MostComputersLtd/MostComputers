using MOSTComputers.Services.Authentication.Contracts;
using MOSTComputers.Services.Authentication.DataAccess.Contracts;
using MOSTComputers.Services.Authentication.Models;
using MOSTComputers.Services.Authentication.Models.Requests.ApiSecret;
using OneOf;

namespace MOSTComputers.Services.Authentication;

internal sealed class ApiSecretService(IApiSecretRepository apiSecretRepository) : IApiSecretService
{
    private readonly IApiSecretRepository _apiSecretRepository = apiSecretRepository;

    public Task<bool> ClientHasActiveSecretAsync(int clientId, DateTime now)
    {
        return _apiSecretRepository.ClientHasActiveSecretAsync(clientId, now);
    }

    public Task<ApiSecret?> GetByHashAsync(byte[] secretHash)
    {
        return _apiSecretRepository.GetByHashAsync(secretHash);
    }

    public Task<OneOf<int, DataAccessFailure>> InsertAsync(ApiSecretCreateRequest createRequest)
    {
        return _apiSecretRepository.InsertAsync(createRequest);
    } 
}
