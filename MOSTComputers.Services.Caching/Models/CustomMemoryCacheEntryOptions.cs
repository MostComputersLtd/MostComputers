using Microsoft.Extensions.Primitives;

namespace MOSTComputers.Services.Caching.Models;
public class CustomMemoryCacheEntryOptions
{
    private TimeSpan? _absoluteExpirationRelativeToNow;

    private TimeSpan? _slidingExpiration;

    private long? _size;

    public DateTimeOffset? AbsoluteExpiration { get; set; }

    public TimeSpan? AbsoluteExpirationRelativeToNow
    {
        get => _absoluteExpirationRelativeToNow;

        set
        {
            if (value <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException("AbsoluteExpirationRelativeToNow", value, "The relative expiration value must be positive.");
            }

            _absoluteExpirationRelativeToNow = value;
        }
    }

    public TimeSpan? SlidingExpiration
    {
        get => _slidingExpiration;

        set
        {
            if (value <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException("SlidingExpiration", value, "The sliding expiration value must be positive.");
            }

            _slidingExpiration = value;
        }
    }

    public IList<IChangeToken> ExpirationTokens { get; } = new List<IChangeToken>();

    public CustomCacheItemPriorityEnum Priority { get; set; } = CustomCacheItemPriorityEnum.Normal;

    public long? Size
    {
        get => _size;

        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, $"{nameof(value)} must be non-negative.");
            }

            _size = value;
        }
    }
}