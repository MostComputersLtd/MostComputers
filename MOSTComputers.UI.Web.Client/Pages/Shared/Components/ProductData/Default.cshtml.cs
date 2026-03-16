using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.SearchStringOrigin.Models;

namespace MOSTComputers.UI.Web.Client.Pages.Shared.Components.ProductData;

public sealed class DefaultModel
{
    public readonly struct DisplayPrice
    {
        public required decimal Amount { get; init; }
        public required Currency Currency { get; init; }
    }

    public sealed class ProductPriceData
    {
        public required DisplayPrice DisplayPrice { get; init; }
        public DisplayPrice? SecondaryDisplayPrice { get; init; }
    }

    public sealed class ProductPropertyDisplayData
    {
        public required ProductCharacteristicDisplayData ProductCharacteristic { get; init; }
        public string? Name { get; init; }
        public string? Value { get; init; }
        public int? DisplayOrder { get; init; }
    }

    public sealed class ProductCharacteristicDisplayData
    {
        public int Id { get; init; }
        public int? CategoryId { get; init; }
        public bool? Active { get; init; }
        public ProductCharacteristicType? KWPrCh { get; init; }
    }

    public sealed class ProductImageDisplayData
    {
        public required string ImageSrc { get; init; }
    }

    public Product? Product { get; set; }
    public ProductPriceData? PriceData { get; set; }

    public List<ProductPropertyDisplayData>? ProductProperties { get; set; }
    public List<ProductImageDisplayData>? ProductImages { get; set; }
    public List<SearchStringPartOriginData>? ProductSearchStringParts { get; set; }
    public bool ShowProductIdData { get; set; } = true;
    public List<Models.Product.Models.Promotions.Promotion>? ProductPromotions { get; set; }

    public const string CarouselItemTemplatePartialPath = "Components/ProductData/CarouselItemPartial";
    public const string CarouselItemIndicatorTemplatePartialPath = "Components/ProductData/CarouselIndicatorItemPartial";

    public static ProductPropertyDisplayData Map(ProductProperty productProperty, ProductCharacteristic productCharacteristic)
    {
        return new ProductPropertyDisplayData
        {
            ProductCharacteristic = new ProductCharacteristicDisplayData
            {
                Id = productCharacteristic.Id,
                CategoryId = productCharacteristic.CategoryId,
                Active = productCharacteristic.Active,
                KWPrCh = productCharacteristic.KWPrCh
            },
            Name = productProperty.Characteristic,
            Value = productProperty.Value,
            DisplayOrder = productProperty.DisplayOrder
        };
    }
}