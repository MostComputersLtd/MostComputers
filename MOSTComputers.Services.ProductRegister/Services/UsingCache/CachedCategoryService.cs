using FluentValidation;
using OneOf.Types;
using OneOf;
using FluentValidation.Results;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.ProductRegister.Models.Requests.Category;
using MOSTComputers.Services.Caching.Services.Contracts;
using static MOSTComputers.Services.ProductRegister.StaticUtilities.CacheKeyUtils.Category;

namespace MOSTComputers.Services.ProductRegister.Services.UsingCache;

internal sealed class CachedCategoryService : ICategoryService
{
    public CachedCategoryService(
        ICategoryService categoryService,
        ICache<string> cache)
    {
        _categoryService = categoryService;
        _cache = cache;
    }

    private readonly ICategoryService _categoryService;
    private readonly ICache<string> _cache;

    public IEnumerable<Category> GetAll()
    {
        return _cache.GetOrAdd(GetAllKey, _categoryService.GetAll);
    }

    public Category? GetById(uint id)
    {
        return _cache.GetOrAdd(GetByIdKey((int)id),
            () => _categoryService.GetById(id));
    }

    public OneOf<uint, ValidationResult, UnexpectedFailureResult> Insert(ServiceCategoryCreateRequest createRequest,
        IValidator<ServiceCategoryCreateRequest>? validator = null)
    {
        OneOf<uint, ValidationResult, UnexpectedFailureResult> result = _categoryService.Insert(createRequest, validator);

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

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> Update(ServiceCategoryUpdateRequest updateRequest,
        IValidator<ServiceCategoryUpdateRequest>? validator = null)
    {
        OneOf<Success, ValidationResult, UnexpectedFailureResult> result = _categoryService.Update(updateRequest, validator);

        bool isSuccessResult = result.Match(
            _ => true,
            _ => false,
            _ => false);

        if (isSuccessResult)
        {
            _cache.Evict(GetByIdKey(updateRequest.Id));

            _cache.Evict(GetAllKey);
        }

        return result;
    }

    public bool Delete(uint id)
    {
        bool success = _categoryService.Delete(id);

        if (success)
        {
            _cache.Evict(GetByIdKey((int)id));

            _cache.Evict(GetAllKey);
        }

        return success;
    }
}