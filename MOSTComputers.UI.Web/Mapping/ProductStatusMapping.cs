using MOSTComputers.Models.Product.Models;

namespace MOSTComputers.UI.Web.Mapping;

internal static class ProductStatusMapping
{
    internal static string? GetStatusStringFromStatusEnum(ProductStatusEnum productStatusEnum)
    {
        return productStatusEnum switch
        {
            ProductStatusEnum.Unavailable => "Unavailable",
            ProductStatusEnum.Available => "Available",
            ProductStatusEnum.Call => "Call",
            _ => "Unavailable"
        };
    }

    internal static ProductStatusEnum? GetStatusEnumFromStatusString(string statusString)
    {
        return statusString switch
        {
            "Unavailable" => ProductStatusEnum.Unavailable,
            "Available" => ProductStatusEnum.Available,
            "Call" => ProductStatusEnum.Call,
            _ => null
        };
    }

    internal static string? GetBGStatusStringFromStatusEnum(ProductStatusEnum? productStatusEnum)
    {
        if (productStatusEnum is null) return null;

        return productStatusEnum switch
        {
            ProductStatusEnum.Unavailable => "Не е в наличност",
            ProductStatusEnum.Available => "В наличност",
            ProductStatusEnum.Call => "Обадете се",
            _ => "Unavailable"
        };
    }

    internal static ProductStatusEnum? GetStatusEnumFromBGStatusString(string statusString)
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