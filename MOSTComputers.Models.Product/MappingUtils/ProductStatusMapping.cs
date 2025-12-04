using MOSTComputers.Models.Product.Models;

namespace MOSTComputers.Models.Product.MappingUtils;
public static class ProductStatusMapping
{
    public static string? GetStatusStringFromStatusEnum(ProductStatus productStatusEnum)
    {
        return productStatusEnum switch
        {
            ProductStatus.Unavailable => "Old",
            ProductStatus.Available => "Avl",
            ProductStatus.Call => "Call",
            _ => null
        };
    }

    public static ProductStatus? GetStatusEnumFromStatusString(string statusString)
    {
        return statusString switch
        {
            "Old" => ProductStatus.Unavailable,
            "Avl" => ProductStatus.Available,
            "Call" => ProductStatus.Call,
            _ => null
        };
    }

    public static string? GetBGStatusStringFromStatusEnum(ProductStatus? productStatusEnum)
    {
        if (productStatusEnum is null) return null;

        return productStatusEnum switch
        {
            ProductStatus.Unavailable => "Не е в наличност",
            ProductStatus.Available => "В наличност",
            ProductStatus.Call => "Обадете се",
            _ => null
        };
    }

    public static ProductStatus? GetStatusEnumFromBGStatusString(string statusString)
    {
        return statusString switch
        {
            "Не е в наличност" => ProductStatus.Unavailable,
            "В наличност" => ProductStatus.Available,
            "Обадете се" => ProductStatus.Call,
            _ => null
        };
    }
}