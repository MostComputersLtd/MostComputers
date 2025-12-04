
namespace MOSTComputers.Models.Product.Models.ProductIdentifiers;
public sealed class ProductGTINCodeType : IEquatable<ProductGTINCodeType>
{
    private ProductGTINCodeType(int value, Func<string, bool> isValidFunc)
    {
        Value = value;
        _isValidFunc = isValidFunc;
    }

    public int Value { get; }

    private readonly Func<string, bool> _isValidFunc;
    public bool IsValid(string value)
    {
        return _isValidFunc(value);
    }

    // EAN-8
    public static readonly ProductGTINCodeType EAN8 = new(
        1,
        IsValidEAN8Code
    );

    // UPC (GTIN-12)
    public static readonly ProductGTINCodeType UPC = new(
        2,
        IsValidUPCCode
    );

    // EAN-13
    public static readonly ProductGTINCodeType EAN13 = new(
        3,
        IsValidEAN13Code
    );

    // JAN (Japanese Article Number)
    public static readonly ProductGTINCodeType JAN = new(
        4,
        IsValidJANCode
    );

    public static readonly IReadOnlyList<ProductGTINCodeType> AllTypes =
    [
        EAN8,
        UPC,
        EAN13,
        JAN
    ];

    public static ProductGTINCodeType? FromValue(int value)
    {
        foreach (ProductGTINCodeType type in AllTypes)
        {
            if (type.Value == value) return type;
        }

        return null;
    }

    public bool Equals(ProductGTINCodeType? other)
    {
        return other != null && Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as ProductGTINCodeType);
    }

    public static bool operator ==(ProductGTINCodeType? left, ProductGTINCodeType? right)
    {
        if (left is null) return right is null;

        return left.Equals(right);
    }

    public static bool operator !=(ProductGTINCodeType? left, ProductGTINCodeType? right)
    {
        return !(left == right);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    private static readonly char[] _validDigits =
       new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

    private static bool IsValidEAN13Code(string value)
    {
        if (string.IsNullOrEmpty(value) || value.Length != 13)
        {
            return false;
        }

        int sum = 0;

        for (int i = 0; i < 12; i++)
        {
            char character = value[i];

            if (Array.IndexOf(_validDigits, character) == -1)
            {
                return false;
            }

            int digit = (int)char.GetNumericValue(character);

            int weight = (i % 2 == 0) ? 1 : 3;

            sum += digit * weight;
        }

        char lastCharacter = value[12];

        if (Array.IndexOf(_validDigits, lastCharacter) == -1)
        {
            return false;
        }

        int lastDigit = (int)char.GetNumericValue(lastCharacter);

        int checkDigit = (10 - (sum % 10)) % 10;

        return lastDigit == checkDigit;
    }

    private static bool IsValidEAN8Code(string value)
    {
        if (string.IsNullOrEmpty(value) || value.Length != 8)
        {
            return false;
        }

        int sum = 0;

        for (int i = 0; i < 7; i++)
        {
            char c = value[i];

            if (Array.IndexOf(_validDigits, c) == -1)
            {
                return false;
            }

            int digit = (int)char.GetNumericValue(c);

            int weight = (i % 2 == 0) ? 3 : 1;

            sum += digit * weight;
        }

        char lastCharacter = value[7];

        if (Array.IndexOf(_validDigits, lastCharacter) == -1)
        {
            return false;
        }

        int lastDigit = (int)char.GetNumericValue(lastCharacter);

        int checkDigit = (10 - (sum % 10)) % 10;

        return lastDigit == checkDigit;
    }

    private static bool IsValidJANCode(string value)
    {
        if (!(value.StartsWith("45") || value.StartsWith("49")))
        {
            return false;
        }

        return IsValidEAN13Code(value);
    }

    private static bool IsValidUPCCode(string value)
    {
        if (string.IsNullOrEmpty(value) || value.Length != 12)
        {
            return false;
        }

        int sum = 0;

        for (int i = 0; i < 11; i++)
        {
            char character = value[i];

            if (Array.IndexOf(_validDigits, character) == -1)
            {
                return false;
            }

            int digit = (int)char.GetNumericValue(character);

            int weight = (i % 2 == 0) ? 3 : 1;

            sum += digit * weight;
        }

        int checkDigit = (10 - (sum % 10)) % 10;

        char lastCharacter = value[11];

        if (Array.IndexOf(_validDigits, lastCharacter) == -1)
        {
            return false;
        }

        int lastDigit = (int)char.GetNumericValue(lastCharacter);

        return lastDigit == checkDigit;
    }
}