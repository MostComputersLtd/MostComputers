using System;
using System.Diagnostics.CodeAnalysis;

namespace MOSTComputers.UI.Web.Utils;
public static class StandardInputValueUtils
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
}