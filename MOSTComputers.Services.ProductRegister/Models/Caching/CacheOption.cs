namespace MOSTComputers.Services.ProductRegister.Models.Caching;

[Serializable]
internal sealed class CacheOption<T>
{
    public T? Value { get; }
    public bool IsEmpty { get; }

    private CacheOption(T value)
    {
        Value = value;
        IsEmpty = false;
    }

    private CacheOption()
    {
        Value = default;
        IsEmpty = true;
    }

    public static CacheOption<T> FromValue(T value)
    {
        return new(value);
    }

    public static CacheOption<T> Empty()
    {
        return new();
    }
}