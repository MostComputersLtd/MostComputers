using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Promotions;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.New.ProductData;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Xml.New.Contracts;

using static MOSTComputers.Models.Product.MappingUtils.CurrencyEnumMapping;
using static MOSTComputers.Models.Product.MappingUtils.ProductStatusMapping;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Xml.New;
internal sealed class ProductToXmlProductMappingService : IProductToXmlProductMappingService
{
    private const string _pidPromotionTypeInXml = "P";
    private const string _ridPromotionTypeInXml = "R";

    private static readonly Dictionary<int, string> _promotionImageIdToInfoStringInXml = new()
    {
        { 1, "New" },
        { 2, "Price Down" },
        { 3, "Price Up" },
        { 4, "Promotion" },
        { 5, "Coming Soon" },
        { 6, "Special Offer" },
        { 7, "Gift" },
        { 8, "Best Offer" },
        { 9, "Only Today" },
        { 10, "Limited" },
        { 11, "On Sale" },
        { 12, "Voucher" },
    };

    public XmlProduct MapProductDataToXmlProduct(
        Product product,
        List<ProductProperty>? productProperties = null,
        List<XmlProductImage>? xmlProductImages = null,
        IEnumerable<Promotion>? productPromotions = null,
        SubCategory? productSubCategory = null,
        List<XmlSearchStringPartInfo>? searchStringPartInfos = null,
        string? promotionPictureUrl = null)
    {
        List<XmlPromotion>? xmlPromotions = productPromotions is not null ? GetXmlPromotionsFromPromotions(product, productPromotions, promotionPictureUrl) : null;

        List<XmlProductProperty> xmlProductProperties = productProperties is not null ? GetXmlPropertiesFromProductProperties(productProperties) : new();

        XmlProduct xmlProduct = new()
        {
            Id = product.Id,
            Name = product.Name ?? string.Empty,
            StatusString = GetBGStatusStringFromStatusEnum(product.Status) ?? string.Empty,
            GeneralDescription = product.PriceListDescription ?? string.Empty,
            Price = product.Price ?? 0,
            CurrencyCode = GetCurrencyCodeFromCurrency(product.Currency),
            SearchString = product.SearchString ?? string.Empty,
            Category = product.Category is not null ? Map(product.Category) : new(),
            SubCategory = productSubCategory is not null ? Map(productSubCategory) : null,
            WarrantyInMonths = product.StandardWarrantyTermMonths,
            SearchStringParts = searchStringPartInfos,
            Images = xmlProductImages ?? new(),
            Promotions = xmlPromotions,
            PartNumber1 = product.PartNumber1,
            PartNumber2 = product.PartNumber2,
            Manufacturer = product.Manufacturer is not null ? Map(product.Manufacturer) : new(),
            Properties = xmlProductProperties,
        };

        return xmlProduct;
    }

    public XmlProduct MapProductDataToXmlProduct(
        Product product,
        List<ProductProperty>? productProperties = null,
        List<XmlProductImage>? xmlProductImages = null,
        List<XmlPromotion>? xmlProductPromotions = null,
        XmlGroupPromotion? xmlGroupPromotion = null,
        SubCategory? productSubCategory = null,
        List<XmlSearchStringPartInfo>? searchStringPartInfos = null)
    {
        List<XmlProductProperty> xmlProductProperties = productProperties is not null ? GetXmlPropertiesFromProductProperties(productProperties) : new();

        XmlProduct xmlProduct = new()
        {
            Id = product.Id,
            Name = product.Name ?? string.Empty,
            StatusString = GetBGStatusStringFromStatusEnum(product.Status) ?? string.Empty,
            GeneralDescription = product.PriceListDescription ?? string.Empty,
            Price = product.Price ?? 0,
            CurrencyCode = GetCurrencyCodeFromCurrency(product.Currency),
            SearchString = product.SearchString ?? string.Empty,
            Category = product.Category is not null ? Map(product.Category) : new(),
            SubCategory = productSubCategory is not null ? Map(productSubCategory) : null,
            SearchStringParts = searchStringPartInfos,
            WarrantyInMonths = product.StandardWarrantyTermMonths,
            Images = xmlProductImages ?? new(),
            Promotions = xmlProductPromotions,
            GroupPromotion = xmlGroupPromotion,
            PartNumber1 = product.PartNumber1,
            PartNumber2 = product.PartNumber2,
            Manufacturer = product.Manufacturer is not null ? Map(product.Manufacturer) : new(),
            Properties = xmlProductProperties,
        };

        return xmlProduct;
    }

    public string GetCurrencyCodeFromCurrency(Currency? currency)
    {
        return GetStringFromCurrencyEnum(currency) ?? string.Empty;
    }

    private static XmlCategory Map(Category category)
    {
        return new()
        {
            Id = category.Id,
            Name = category.Description ?? string.Empty,
            ParentCategoryId = category.ParentCategoryId,
        };
    }

    private static XmlSubCategory Map(SubCategory subCategory)
    {
        return new()
        {
            Id = subCategory.Id,
            Name = subCategory.Name ?? string.Empty,
        };
    }

    private static XmlManufacturer Map(Manufacturer manufacturer)
    {
        return new()
        {
            IdAsString = manufacturer.Id.ToString(),
            Name = manufacturer.RealCompanyName ?? string.Empty,
        };
    }
    
    private static List<XmlPromotion> GetXmlPromotionsFromPromotions(Product product, IEnumerable<Promotion> promotions, string? promotionPictureUrl = null)
    {
        List<XmlPromotion> output = new();

        int? promotionPictureId = product.AlertPictureId;

        if (promotionPictureId is null || promotionPictureId <= 0)
        {
            promotionPictureId = product.PromotionPictureId;
        }

        bool isInfoPromotionActive = (product.AlertExpireDate is null || product.AlertExpireDate >= DateTime.Now);

        if (promotionPictureId > 0 && isInfoPromotionActive)
        {
            XmlPromotion xmlPromotion = new()
            {
                Type = "Info",
                ValidFrom = null,
                ValidTo = product.AlertExpireDate,
                Info = GetInfoPromotionDataStringFromPictureId(promotionPictureId),
                PictureUrl = promotionPictureUrl
            };

            output.Add(xmlPromotion);
        }

        foreach (Promotion promotion in promotions)
        {
            XmlPromotion xmlPromotion = new()
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
            1 => _pidPromotionTypeInXml,
            2 => _ridPromotionTypeInXml,
            _ => null,
        };
    }

    private static string? GetInfoPromotionDataStringFromPictureId(int? promotionPictureId)
    {
        if (promotionPictureId is null) return null;

        bool exists = _promotionImageIdToInfoStringInXml.TryGetValue(promotionPictureId.Value, out string? infoString);

        return exists ? infoString : null;
    }

    private static List<XmlProductProperty> GetXmlPropertiesFromProductProperties(List<ProductProperty> productProperties)
    {
        List<XmlProductProperty> output = new();

        foreach (ProductProperty property in productProperties)
        {
            XmlProductProperty xmlProductProperty = new()
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