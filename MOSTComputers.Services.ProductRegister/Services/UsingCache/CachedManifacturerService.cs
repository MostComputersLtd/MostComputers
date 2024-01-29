using FluentValidation;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
using OneOf.Types;
using OneOf;
using FluentValidation.Results;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Requests.Manifacturer;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;
using MOSTComputers.Services.Caching.Services.Contracts;
using static MOSTComputers.Services.ProductRegister.StaticUtilities.CacheKeyUtils.Manifacturer;

namespace MOSTComputers.Services.ProductRegister.Services.UsingCache;

internal sealed class CachedManifacturerService : IManifacturerService
{
    public CachedManifacturerService(
        IManifacturerService manifacturerService,
        ICache<string> cache)
    {
        _manifacturerService = manifacturerService;
        _cache = cache;
    }

    private readonly IManifacturerService _manifacturerService;
    private readonly ICache<string> _cache;

    public IEnumerable<Manifacturer> GetAll()
    {
        return _cache.GetOrAdd(GetAllKey, _manifacturerService.GetAll);
    }

    public Manifacturer? GetById(uint id)
    {
        return _cache.GetOrAdd(GetByIdKey((int)id), () => _manifacturerService.GetById(id));
    }

    public OneOf<uint, ValidationResult, UnexpectedFailureResult> Insert(ManifacturerCreateRequest createRequest,
        IValidator<ManifacturerCreateRequest>? validator = null)
    {

        OneOf<uint, ValidationResult, UnexpectedFailureResult> result = _manifacturerService.Insert(createRequest, validator);

        uint? idFromResult = result.Match<uint?>(
            id => id,
            _ => null,
            _ => null);

        if (idFromResult is not null)
        {
            _cache.Evict(GetByIdKey((int)idFromResult));

            _cache.Evict(GetAllKey);
        }

        return result;
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> Update(ManifacturerUpdateRequest updateRequest,
        IValidator<ManifacturerUpdateRequest>? validator = null)
    {
        OneOf<Success, ValidationResult, UnexpectedFailureResult> result = _manifacturerService.Update(updateRequest, validator);

        bool successOfResult = result.Match(
            success => true,
            _ => false,
            _ => false);

        if (successOfResult)
        {
            _cache.Evict(GetByIdKey(updateRequest.Id));

            _cache.Evict(GetAllKey);
        }

        return result;
    }

    public bool Delete(uint id)
    {
        bool success = _manifacturerService.Delete(id);

        if (success)
        {
            _cache.Evict(GetByIdKey((int)id));

            _cache.Evict(GetAllKey);
        }

        return success;
    }
}