using MOSTComputers.Models.Product.Models;

namespace MOSTComputers.Models.Product.MappingUtils;

public static class ProductStatusMapping
{
    public static string? GetStatusStringFromStatusEnum(ProductStatusEnum productStatusEnum)
    {
        return productStatusEnum switch
        {
            ProductStatusEnum.Unavailable => "Unavailable",
            ProductStatusEnum.Available => "Available",
            ProductStatusEnum.Call => "Call",
            _ => null
        };
    }

    public static ProductStatusEnum? GetStatusEnumFromStatusString(string statusString)
    {
        return statusString switch
        {
            "Unavailable" => ProductStatusEnum.Unavailable,
            "Available" => ProductStatusEnum.Available,
            "Call" => ProductStatusEnum.Call,
            _ => null
        };
    }

    public static string? GetBGStatusStringFromStatusEnum(ProductStatusEnum? productStatusEnum)
    {
        if (productStatusEnum is null) return null;

        return productStatusEnum switch
        {
            ProductStatusEnum.Unavailable => "Не е в наличност",
            ProductStatusEnum.Available => "В наличност",
            ProductStatusEnum.Call => "Обадете се",
            _ => null
        };
    }

    public static ProductStatusEnum? GetStatusEnumFromBGStatusString(string statusString)
    {
        return statusString switch
        {
            "Не е в наличност" => ProductStatusEnum.Unavailable,
            "В наличност" => ProductStatusEnum.Available,
            "Обадете се" => ProductStatusEnum.Call,
            _ => null
        };
    }
}