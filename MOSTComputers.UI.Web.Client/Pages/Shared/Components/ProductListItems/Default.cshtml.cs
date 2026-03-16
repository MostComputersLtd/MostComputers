using MOSTComputers.Models.Product.Models;

namespace MOSTComputers.UI.Web.Client.Pages.Shared.Components.ProductListItems;

public class ProductListItemsModel
{
    public class PriceDisplayData
    {
        public required decimal Amount { get; set; }
        public required Currency Currency { get; set; }
    }

    public class ProductDisplayData
    {
        public required Product Product { get; set; }
        public bool NeedsPromotionalBanner { get; set; } = false;
        public string? ProductImageUrl { get; set; }
        public PriceDisplayData? PriceDisplayData { get; set; }
        public PriceDisplayData? SecondaryPriceDisplayData { get; set; }
    }

    public required List<ProductDisplayData> Products { get; set; }
}