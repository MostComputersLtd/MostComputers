using MOSTComputers.UI.Web.Models.Authentication;
using System.Numerics;

namespace MOSTComputers.UI.Web.Models.ProductEditor.ProductProperties;

public sealed class ProductPropertyEditorPropertyGroup : IEquatable<ProductPropertyEditorPropertyGroup>, IEqualityOperators<ProductPropertyEditorPropertyGroup, ProductPropertyEditorPropertyGroup, bool>
{
    private ProductPropertyEditorPropertyGroup(int value, string elementIdPrefix, string tableName)
    {
        Value = value;
        ElementIdPrefix = elementIdPrefix;
        ParentElementId = tableName;
    }

    public int Value { get; }
    public string ElementIdPrefix { get; }
    public string ParentElementId { get; }

    public static readonly ProductPropertyEditorPropertyGroup Properties = new(0, "productProperty_", "propertyEditorTableBody");
    public static readonly ProductPropertyEditorPropertyGroup Links = new(1, "productLink_", "propertyEditorLinksTableBody");

    public static ProductPropertyEditorPropertyGroup? GetFromValue(int value)
    {
        return value switch
        {
            0 => Properties,
            1 => Links,
            _ => null,
        };
    }

    public bool Equals(ProductPropertyEditorPropertyGroup? other)
    {
        if (other is null) return false;

        return Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as UserRoles);
    }

    public override int GetHashCode()
    {
        return Value;
    }

    public static bool operator ==(ProductPropertyEditorPropertyGroup? left, ProductPropertyEditorPropertyGroup? right)
    {
        if (left is null) return right is null;

        return left.Equals(right);
    }

    public static bool operator !=(ProductPropertyEditorPropertyGroup? left, ProductPropertyEditorPropertyGroup? right)
    {
        return !(left == right);
    }
}