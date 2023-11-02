using MOSTComputers.Services.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOSTComputers.Services.XMLDataOperations.Localization;

internal static class ProductStatusMapping
{
    internal static string GetBGStatusStringFromStatusEnum(ProductStatusEnum productStatusEnum)
    {
        return productStatusEnum switch
        {
            ProductStatusEnum.Unavailable => "Unavailable",
            ProductStatusEnum.Available => "Available",
            ProductStatusEnum.Call => "Call",
            _ => "Unavailable"
        };
    }

    internal static ProductStatusEnum? GetStatusEnumFromBGStatusString(string statusString)
    {
        return statusString switch
        {
            "Unavailable" => ProductStatusEnum.Unavailable,
            "Available" => ProductStatusEnum.Available,
            "Call" => ProductStatusEnum.Call,
            _ => null
        };
    }
}