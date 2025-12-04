using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.ProductImages;
using MOSTComputers.Models.Product.Models.Promotions;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.Legacy;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Xml.Legacy.Contracts;

using static MOSTComputers.Models.Product.MappingUtils.CurrencyEnumMapping;
using static MOSTComputers.Models.Product.MappingUtils.ProductStatusMapping;
using static MOSTComputers.Utils.Files.FilePathUtils;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Xml.Legacy;
internal sealed class LegacyProductToXmlProductMappingService : ILegacyProductToXmlProductMappingService
{
    public LegacyXmlProduct MapProductDataToXmlProduct(
        Product product,
        List<ProductProperty>? productProperties = null,
        List<LegacyXmlProductImage>? xmlProductImages = null,
        IEnumerable<Promotion>? productPromotions = null,
        SubCategory? productSubCategory = null,
        List<LegacySearchStringPartInfo>? searchStringPartInfos = null)
    {
        return new()
        {
            Id = product.Id,
            UId = 0,
            Name = product.Name ?? string.Empty,
            StatusString = GetBGStatusStringFromStatusEnum(product.Status) ?? string.Empty,
            HasPromotion = product.PromotionPid > 0,
            GeneralDescription = product.PriceListDescription ?? string.Empty,
            Price = product.Price ?? 0,
            Code = product.Name ?? string.Empty,
            CurrencyCode = GetStringFromCurrencyEnum(product.Currency) ?? string.Empty,
            MainPictureUrl = xmlProductImages?.FirstOrDefault()?.PictureUrl ?? string.Empty,
            SearchString = product.SearchString ?? string.Empty,
            Category = product.Category is not null ? Map(product.Category) : new(),
            SubCategory = productSubCategory is not null ? Map(productSubCategory) : null,
            SearchStringParts = searchStringPartInfos?.ToList(),
            Images = xmlProductImages ?? new(),
            Promotions = productPromotions is not null ? GetXmlPromotionsFromPromotions(productPromotions) : null,
            PartNumbers = $"{product.PartNumber1} / {product.PartNumber2}",
            Manufacturer = product.Manufacturer is not null ? Map(product.Manufacturer) : new(),
            Properties = productProperties is not null ? GetXmlPropertiesFromProductProperties(productProperties) : new(),
        };
    }

    private static LegacyXmlCategory Map(Category category)
    {
        return new()
        {
            Id = category.Id,
            Name = category.Description ?? string.Empty,
            ParentCategoryId = category.ParentCategoryId,
        };
    }

    private static LegacyXmlSubCategory Map(SubCategory subCategory)
    {
        return new()
        {
            Id = subCategory.Id,
            Name = subCategory.Name ?? string.Empty,
        };
    }

    private static LegacyXmlManufacturer Map(Manufacturer manufacturer)
    {
        return new()
        {
            IdAsString = manufacturer.Id.ToString(),
            Name = manufacturer.RealCompanyName ?? string.Empty,
        };
    }

    private static List<LegacyXmlPromotion> GetXmlPromotionsFromPromotions(IEnumerable<Promotion> promotions)
    {
        List<LegacyXmlPromotion> output = new();

        foreach (Promotion promotion in promotions)
        {
            LegacyXmlPromotion xmlPromotion = new()
            {
                Type = GetPromotionTypeAsXmlDisplayString(promotion.Type),
                ValidFrom = promotion.StartDate,
                ValidTo = promotion.ExpirationDate,
                PromotionUSD = promotion.DiscountUSD,
                PromotionEUR = promotion.DiscountEUR,
            };

            output.Add(xmlPromotion);
        }

        return output;
    }

    private static string? GetPromotionTypeAsXmlDisplayString(short? promotionType)
    {
        if (promotionType is null) return null;

        return promotionType switch
        {
            1 => "P",
            2 => "Info",
            _ => null,
        };
    }

    private static List<LegacyXmlProductProperty> GetXmlPropertiesFromProductProperties(List<ProductProperty> productProperties)
    {
        List<LegacyXmlProductProperty> output = new();

        foreach (ProductProperty property in productProperties)
        {
            LegacyXmlProductProperty xmlProductProperty = new()
            {
                Name = property.Characteristic ?? string.Empty,
                Value = property.Value ?? string.Empty,
                Order = property.DisplayOrder?.ToString() ?? string.Empty
            };

            output.Add(xmlProductProperty);
        }

        return output;
    }
}