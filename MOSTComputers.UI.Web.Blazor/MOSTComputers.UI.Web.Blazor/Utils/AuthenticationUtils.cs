using Microsoft.AspNetCore.Identity;
using OpenIddict.Validation.AspNetCore;

namespace MOSTComputers.UI.Web.Blazor.Utils;

internal static class AuthenticationUtils
{
    public static readonly string CookieAuthenticationScheme = IdentityConstants.ApplicationScheme;
    public const string ApiAuthenticationScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;

    public static class Policies
    {
        public const string Cookie = "Cookie";

        public const string ReadInvoices = "ReadInvoices";
        public const string ReadInvoiceResource = "ReadInvoiceResource";

        public const string ReadWarrantyCards = "ReadWarrantyCards";
        public const string ReadWarrantyCardResource = "ReadWarrantyCardResource";

        public const string ReadOrderPageComponents = "ReadOrderPageComponents";
        public const string ReadOrders = "ReadOrders";
        public const string ReadOrderResource = "ReadOrderResource";
    }

    public static class Scopes
    {
        public const string ReadInvoices = "invoices.read";
        public const string ReadWarrantyCards = "warrantyCards.read";
        public const string ReadOrders = "orders.read";

        public static readonly string[] AllScopes = [ReadInvoices, ReadWarrantyCards, ReadOrders];
    }
}
