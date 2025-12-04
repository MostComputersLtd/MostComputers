using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.ProductStatuses;
using System.Diagnostics.CodeAnalysis;

namespace MOSTComputers.UI.Web.Blazor.Client.Utils;

public static class DataToTextConversion
{
    internal const string LocalDateTimeInputDateTimeFormat = "yyyy-MM-ddTHH:mm";
    internal const string DateInputDateTimeFormat = "yyyy-MM-dd";

    [return: NotNullIfNotNull(nameof(dateTime))]
    public static string? GetDateTimeAsStringForLocalDateTimeInput(DateTime? dateTime)
    {
        if (dateTime is null) return null;

        return dateTime.Value.ToString(LocalDateTimeInputDateTimeFormat);
    }

    [return: NotNullIfNotNull(nameof(dateTime))]
    public static string? GetDateTimeAsStringForDateInput(DateTime? dateTime)
    {
        if (dateTime is null) return null;

        return dateTime.Value.ToString(DateInputDateTimeFormat);
    }

    [return: NotNullIfNotNull(nameof(date))]
    public static string? GetDateTimeAsStringForDateInput(DateOnly? date)
    {
        if (date is null) return null;

        return date.Value.ToString(DateInputDateTimeFormat);
    }

    public static string? GetPriceSuffixStringFromCurrency(Currency currencyEnum)
    {
        return currencyEnum switch
        {
            Currency.BGN => "лв.",
            Currency.EUR => "€",
            Currency.USD => "USD",
            _ => null
        };
    }

    public static string? GetStringFromProductStatus(ProductStatus productStatusEnum)
    {
        return productStatusEnum switch
        {
            ProductStatus.Unavailable => "Old",
            ProductStatus.Available => "Avl",
            ProductStatus.Call => "Call",
            _ => null
        };
    }

    public static string? GetStringFromProductNewStatus(ProductNewStatus productStatusEnum)
    {
        return productStatusEnum switch
        {
            ProductNewStatus.New => "New",
            ProductNewStatus.WorkInProgress => "Work",
            ProductNewStatus.ReadyForUse => "Ready",
            ProductNewStatus.Postponed1 => "Post1",
            ProductNewStatus.Postponed2 => "Post2",
            _ => null
        };
    }
}