namespace MOSTComputers.UI.Web.Blazor.Utils;

internal static class AuthenticationUtils
{
    public const string ApiAuthenticationScheme = "Api";

    public static class Policies
    {
        public const string ReadInvoicesThatAllowApi = "ReadInvoicesThatAllowApi";
        public const string ReadInvoiceResource = "ReadInvoiceResource";

        public const string ReadWarrantyCardsThatAllowApi = "ReadWarrantyCardsThatAllowApi";
        public const string ReadWarrantyCardResource = "ReadWarrantyCardResource";

        public const string ReadOrders = "ReadOrders";
        public const string ReadOrdersThatAllowApi = "ReadOrdersThatAllowApi";
        public const string ReadOrderResource = "ReadOrderResource";
    }
}
