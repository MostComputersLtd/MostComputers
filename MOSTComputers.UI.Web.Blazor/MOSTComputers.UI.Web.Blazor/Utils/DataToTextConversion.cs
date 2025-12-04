using System.Diagnostics.CodeAnalysis;
using System.Text;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.ProductStatuses;
using MOSTComputers.UI.Web.Blazor.Models.Search.Product;

namespace MOSTComputers.UI.Web.Blazor.Utils;

internal static class DataToTextConversion
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
            Currency.USD => "$",
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

    public static string? GetStringFromProductStatusSearchOptions(ProductStatusSearchOptions productStatusEnum)
    {
        return productStatusEnum switch
        {
            ProductStatusSearchOptions.Unavailable => "Old",
            ProductStatusSearchOptions.Available => "Avl",
            ProductStatusSearchOptions.Call => "Call",
            ProductStatusSearchOptions.AvailableAndCall => "Avl & Call",
            _ => null
        };
    }

    public static string? GetStringFromProductNewStatusesSearchOptions(IEnumerable<ProductNewStatusSearchOptions> productStatusEnum)
    {
        StringBuilder stringBuilder = new();

        foreach (ProductNewStatusSearchOptions status in productStatusEnum)
        {
            string? statusString = GetStringFromProductNewStatusSearchOptions(status);

            if (statusString is null) continue;

            if (stringBuilder.Length > 0)
            {
                stringBuilder.Append(" & ");
            }

            stringBuilder.Append(statusString);
        }

        return stringBuilder.Length > 0 ? stringBuilder.ToString() : null;
    }

    public static string? GetStringFromProductNewStatusSearchOptions(ProductNewStatusSearchOptions productStatusEnum)
    {
        return productStatusEnum switch
        {
            ProductNewStatusSearchOptions.New => "New",
            ProductNewStatusSearchOptions.WorkInProgress => "Work",
            ProductNewStatusSearchOptions.ReadyForUse => "Ready",
            ProductNewStatusSearchOptions.Postponed1 => "Post1",
            ProductNewStatusSearchOptions.Postponed2 => "Post2",
            _ => null
        };
    }

    public static string? GetStringFromPromotionSearchOptions(PromotionSearchOptions promotionSearchOptions)
    {
        return promotionSearchOptions switch
        {
            PromotionSearchOptions.None => "None",
            PromotionSearchOptions.P => "PID",
            PromotionSearchOptions.R => "RID",
            PromotionSearchOptions.I => "Info",
            PromotionSearchOptions.All => "All",
            PromotionSearchOptions.Discount => "Discount",
            _ => null
        };
    }
}