using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using MOSTComputers.Services.Caching.Models;

namespace MOSTComputers.Services.Caching.Mapping;
internal static class LazyCacheMappingUtils
{
    internal static MemoryCacheEntryOptions Map(CustomMemoryCacheEntryOptions customOptions)
    {
        MemoryCacheEntryOptions output = new()
        {
            AbsoluteExpiration = customOptions.AbsoluteExpiration,
            AbsoluteExpirationRelativeToNow = customOptions.AbsoluteExpirationRelativeToNow,
            SlidingExpiration = customOptions.SlidingExpiration,
            Priority = Map(customOptions.Priority),
            Size = customOptions.Size,
        };

        foreach (IChangeToken changeToken in customOptions.ExpirationTokens)
        {
            output.AddExpirationToken(changeToken);
        }

        return output;
    }

    internal static CacheItemPriority Map(CustomCacheItemPriorityEnum priority)
    {
        return priority switch
        {
            CustomCacheItemPriorityEnum.Low => CacheItemPriority.Low,
            CustomCacheItemPriorityEnum.Normal => CacheItemPriority.Normal,
            CustomCacheItemPriorityEnum.High => CacheItemPriority.High,
            CustomCacheItemPriorityEnum.NeverRemove => CacheItemPriority.NeverRemove,
            _ => throw new NotImplementedException()
        };
    }
}