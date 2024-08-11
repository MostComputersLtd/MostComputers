using FluentValidation;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
using OneOf.Types;
using OneOf;
using FluentValidation.Results;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Requests.Manifacturer;
using MOSTComputers.Services.Caching.Services.Contracts;
using static MOSTComputers.Services.ProductRegister.StaticUtilities.CacheKeyUtils.Manifacturer;
using static MOSTComputers.Services.ProductRegister.StaticUtilities.ProductDataCloningUtils;

namespace MOSTComputers.Services.ProductRegister.Services.UsingCache;

internal sealed class CachedManifacturerService : IManifacturerService
{
    public CachedManifacturerService(
        ManifacturerService manifacturerService,
        ICache<string> cache)
    {
        _manifacturerService = manifacturerService;
        _cache = cache;
    }

    private readonly ManifacturerService _manifacturerService;
    private readonly ICache<string> _cache;

    public IEnumerable<Manifacturer> GetAll()
    {
        IEnumerable<Manifacturer> manifacturers = _cache.GetOrAdd(GetAllKey, _manifacturerService.GetAll);

        return CloneAll(manifacturers);
    }

    public Manifacturer? GetById(int id)
    {
        if (id <= 0) return null;

        Manifacturer? manifacturer = _cache.GetOrAdd(GetByIdKey((int)id),
            () => _manifacturerService.GetById(id));

        if (manifacturer is null) return null;

        return Clone(manifacturer);
    }

    public OneOf<int, ValidationResult, UnexpectedFailureResult> Insert(ManifacturerCreateRequest createRequest,
        IValidator<ManifacturerCreateRequest>? validator = null)
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> result = _manifacturerService.Insert(createRequest, validator);

        int? idFromResult = result.Match<int?>(
            id => id,
            _ => null,
            _ => null);

        if (idFromResult is not null)
        {
            _cache.Evict(GetByIdKey(idFromResult.Value));

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

    public bool Delete(int id)
    {
        if (id <= 0) return false;

        bool success = _manifacturerService.Delete(id);

        if (success)
        {
            _cache.Evict(GetByIdKey(id));

            _cache.Evict(GetAllKey);
        }

        return success;
    }
}