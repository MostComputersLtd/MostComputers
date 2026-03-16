using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using ZiggyCreatures.Caching.Fusion;

using static MOSTComputers.Services.ProductRegister.Configuration.ConfigureServices;
using static MOSTComputers.Services.ProductRegister.Utils.Caching.CacheKeyUtils.ForCategory;

namespace MOSTComputers.Services.ProductRegister.Services.Cached;

public sealed class CachedCategoryService : ICategoryService
{
    public CachedCategoryService(
        [FromKeyedServices(CategoryServiceKey)] ICategoryService categoryService,
        IFusionCache fusionCache)
    {
        _categoryService = categoryService;
        _fusionCache = fusionCache;
    }

    private static readonly TimeSpan _defaultItemAbsoluteExpiration = TimeSpan.FromMinutes(5);

    private readonly ICategoryService _categoryService;
    private readonly IFusionCache _fusionCache;

    public async Task<List<Category>> GetAllAsync()
    {
        return await _fusionCache.GetOrSetAsync(GetAllKey, 
            async (cancellationToken) => await _categoryService.GetAllAsync(),
            _defaultItemAbsoluteExpiration);
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        return await _fusionCache.GetOrSetAsync(GetByIdKey(id),
            async (cancellationToken) => await _categoryService.GetByIdAsync(id),
            _defaultItemAbsoluteExpiration);
    }
}