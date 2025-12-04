using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

namespace MOSTComputers.UI.Web.Blazor.Client.Localization;

public class CultureService
{
    public EventCallback<CultureInfo> OnCultureChanged;

    public CultureInfo CurrentCulture => CultureInfo.CurrentCulture;
    public CultureInfo CurrentUICulture => CultureInfo.CurrentUICulture;

    public async Task SetCultureAsync(CultureInfo culture)
    {
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;

        await OnCultureChanged.InvokeAsync(culture);
    }
}