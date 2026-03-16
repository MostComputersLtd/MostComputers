using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using ZiggyCreatures.Caching.Fusion;

using static MOSTComputers.Services.ProductRegister.Configuration.ConfigureServices;
using static MOSTComputers.Services.ProductRegister.Utils.Caching.CacheKeyUtils.ForManufacturer;

namespace MOSTComputers.Services.ProductRegister.Services.Cached;

public sealed class CachedManufacturerService : IManufacturerService
{
    public CachedManufacturerService(
        [FromKeyedServices(ManufacturerServiceKey)] IManufacturerService manufacturerService,
        IFusionCache fusionCache)
    {
        _manufacturerService = manufacturerService;
        _fusionCache = fusionCache;
    }

    private static readonly TimeSpan _defaultItemAbsoluteExpiration = TimeSpan.FromMinutes(5);

    private readonly IManufacturerService _manufacturerService;
    private readonly IFusionCache _fusionCache;

    public async Task<List<Manufacturer>> GetAllAsync()
    {
        return await _fusionCache.GetOrSetAsync(GetAllKey,
            async (cancellationToken) => await _manufacturerService.GetAllAsync(),
            _defaultItemAbsoluteExpiration);
    }

    public async Task<List<Manufacturer>> GetAllWithActiveProductsAsync()
    {
        return await _fusionCache.GetOrSetAsync(GetAllWithActiveProductsKey,
            async (cancellationToken) => await _manufacturerService.GetAllWithActiveProductsAsync(),
            _defaultItemAbsoluteExpiration);
    }

    public async Task<Manufacturer?> GetByIdAsync(int id)
    {
        if (id <= 0) return null;

        return await _fusionCache.GetOrSetAsync(GetByIdKey(id),
            async (cancellationToken) => await _manufacturerService.GetByIdAsync(id),
            _defaultItemAbsoluteExpiration);
    }
}