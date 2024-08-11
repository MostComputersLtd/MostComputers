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
using static MOSTComputers.Services.ProductRegister.StaticUtilities.ProductDataCloningUtils;

namespace MOSTComputers.Services.ProductRegister.Services.UsingCache;

internal sealed class CachedCategoryService : ICategoryService
{
    public CachedCategoryService(
        CategoryService categoryService,
        ICache<string> cache)
    {
        _categoryService = categoryService;
        _cache = cache;
    }

    private readonly CategoryService _categoryService;
    private readonly ICache<string> _cache;

    public IEnumerable<Category> GetAll()
    {
        IEnumerable<Category> categories = _cache.GetOrAdd(GetAllKey, _categoryService.GetAll);

        return CloneAll(categories);
    }

    public Category? GetById(int id)
    {
        if (id <= 0) return null;

        Category? category = _cache.GetOrAdd(GetByIdKey(id),
            () => _categoryService.GetById(id));

        if (category is null) return null;

        return Clone(category);
    }

    public OneOf<int, ValidationResult, UnexpectedFailureResult> Insert(ServiceCategoryCreateRequest createRequest,
        IValidator<ServiceCategoryCreateRequest>? validator = null)
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> result = _categoryService.Insert(createRequest, validator);

        int? idFromResult = result.Match<int?>(
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

    public bool Delete(int id)
    {
        if (id <= 0) return false;

        bool success = _categoryService.Delete(id);

        if (success)
        {
            _cache.Evict(GetByIdKey(id));

            _cache.Evict(GetAllKey);
        }

        return success;
    }
}