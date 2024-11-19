namespace MOSTComputers.Services.ProductRegister.Models.Grouping;

internal class Grouping<TKey, TElement> : List<TElement>, IGrouping<TKey, TElement>
{
    internal Grouping(TKey key) : base() => Key = key;
    internal Grouping(TKey key, int capacity) : base(capacity) => Key = key;
    internal Grouping(TKey key, IEnumerable<TElement> collection)
        : base(collection) => Key = key;
    public TKey Key { get; }
}