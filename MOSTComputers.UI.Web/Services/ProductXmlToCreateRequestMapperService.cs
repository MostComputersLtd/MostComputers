using FluentValidation.Results;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Requests.Product;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Services.XMLDataOperations.Models;
using MOSTComputers.Services.XMLDataOperations.Services;
using MOSTComputers.UI.Web.Mapping;
using MOSTComputers.UI.Web.Models;
using OneOf;

namespace MOSTComputers.UI.Web.Services;

public class ProductXmlToCreateRequestMapperService
{
    public ProductXmlToCreateRequestMapperService(
        IProductCharacteristicService productCharacteristicService,
        IHttpClientFactory httpClientFactory,
        ProductDeserializeService productDeserializeService)
    {
        _productCharacteristicService = productCharacteristicService;
        _httpClient = httpClientFactory.CreateClient();
        _productDeserializeService = productDeserializeService;
    }

    private readonly IProductCharacteristicService _productCharacteristicService;
    private readonly HttpClient _httpClient;
    private readonly ProductDeserializeService _productDeserializeService;

    public async Task<OneOf<List<ProductCreateRequest>, ValidationResult, UnexpectedFailureResult, InvalidXmlResult>> GetProductCreateRequestsFromXmlAsync(string xmlText)
    {
        OneOf<XmlObjectData?, InvalidXmlResult> result = _productDeserializeService.TryDeserializeProductsXml(xmlText);

        if (result.IsT1) return result.AsT1;

        XmlObjectData? xmlObjectData = result.AsT0;

        if (xmlObjectData is null) return new UnexpectedFailureResult();

        List<ProductCreateRequest> output = new();

        ValidationResult? validationResult = null;

        foreach (XmlProduct item in xmlObjectData.Products)
        {
            OneOf<ProductCreateRequest, ValidationResult> itemMappingResult =
                await GetProductFromXmlDataAsync(item, xmlText);

            itemMappingResult.Switch(
                output.Add,
                validationRes => validationResult = validationRes);
            
            if (validationResult is not null) return validationResult;
        }

        return output;
    }

    public async Task<OneOf<List<ProductCreateRequest>, ValidationResult>> GetProductCreateRequestsFromXmlAsync(XmlObjectData xmlObjectData, string xmlTextForImages)
    {
        List<ProductCreateRequest> output = new();

        ValidationResult? validationResult = null;

        foreach (XmlProduct item in xmlObjectData.Products)
        {
            OneOf<ProductCreateRequest, ValidationResult> itemMappingResult =
                await GetProductFromXmlDataAsync(item, xmlTextForImages);

            itemMappingResult.Switch(
                output.Add,
                validationRes => validationResult = validationRes);

            if (validationResult is not null) return validationResult;
        }

        return output;
    }

    public async Task<OneOf<List<XmlProductCreateDisplay>, ValidationResult, InvalidXmlResult, UnexpectedFailureResult>> GetProductXmlDisplayFromXmlAsync(string xmlText)
    {
        OneOf<XmlObjectData?, InvalidXmlResult> result = _productDeserializeService.TryDeserializeProductsXml(xmlText);

        if (result.IsT1) return result.AsT1;

        XmlObjectData? xmlObjectData = result.AsT0;

        if (xmlObjectData is null) return new UnexpectedFailureResult();

        List<XmlProductCreateDisplay> output = new();

        foreach (XmlProduct item in xmlObjectData.Products)
        {
            ValidationResult? validationResult = null;

            OneOf<XmlProductCreateDisplay, ValidationResult> requestMappingResult = await GetDisplayProductFromXmlDataAsync(item, xmlText);

            requestMappingResult.Switch(
                output.Add,
                validationRes => validationResult = validationRes);

            if (validationResult is not null) return validationResult;
        }

        return output;
    }

    public XmlProductCreateDisplay GetProductXmlDisplayFromProductData(XmlProduct xmlProduct, ProductCreateRequest createRequest)
    {
        XmlProductCreateDisplay output = new()
        {
            Id = xmlProduct.Id,
            Name = createRequest.Name,
            AdditionalWarrantyPrice = createRequest.AdditionalWarrantyPrice,
            AdditionalWarrantyTermMonths = createRequest.AdditionalWarrantyTermMonths,
            StandardWarrantyPrice = createRequest.StandardWarrantyPrice,
            StandardWarrantyTermMonths = createRequest.StandardWarrantyTermMonths,
            DisplayOrder = createRequest.DisplayOrder,
            Status = createRequest.Status,
            PlShow = createRequest.PlShow,
            Price1 = createRequest.Price1,
            DisplayPrice = createRequest.DisplayPrice,
            Price3 = createRequest.Price3,
            Currency = createRequest.Currency,
            RowGuid = createRequest.RowGuid,
            Promotionid = createRequest.Promotionid,
            PromRid = createRequest.PromRid,
            PromotionPictureId = createRequest.PromotionPictureId,
            PromotionExpireDate = createRequest.PromotionExpireDate,
            AlertPictureId = createRequest.AlertPictureId,
            AlertExpireDate = createRequest.AlertExpireDate,
            PriceListDescription = createRequest.PriceListDescription,
            PartNumber1 = createRequest.PartNumber1,
            PartNumber2 = createRequest.PartNumber2,
            SearchString = createRequest.SearchString,

            Properties = new(),
            Images = createRequest.Images,
            ImageFileNames = createRequest.ImageFileNames,

            Category = Map(xmlProduct.Category),
            Manifacturer = Map(xmlProduct.Manifacturer),
            SubCategoryId = createRequest.SubCategoryId
        };

        for (int i = 0; i < createRequest.Properties!.Count; i++)
        {
            var prop = createRequest.Properties[i];

            var xmlProp = xmlProduct.XmlProductProperties[i];

            output.Properties.Add(new()
            {
                ProductCharacteristicId = prop.ProductCharacteristicId,
                Name = xmlProp.Name,
                Value = prop.Value,
                DisplayOrder = prop.DisplayOrder,
                XmlPlacement = prop.XmlPlacement,
            });
        }

        return output;
    }


    private async Task<OneOf<ProductCreateRequest, ValidationResult>> GetProductFromXmlDataAsync(XmlProduct product, string xml)
    {
        OneOf<List<CurrentProductPropertyCreateRequest>, ValidationResult> resultOfPropsMapping
            = GetPropertyCreateRequestsFromXmlData(product.XmlProductProperties, (uint)product.Category.Id);

        List<CurrentProductImageCreateRequest> images = await GetImagesFromXmlDataAsync(product.ShopItemImages, xml);

        int? standardWarrantyTermMonths = GetWarrantyData(product.XmlProductProperties);

        if (resultOfPropsMapping.IsT1) return resultOfPropsMapping.AsT1;

        return new ProductCreateRequest()
        {
            Name = product.Name,
            Status = product.Status,
            StandardWarrantyTermMonths = standardWarrantyTermMonths,
            DisplayPrice = product.Price,
            Currency = CurrencyEnumMapping.GetCurrencyEnumFromString(product.CurrencyCode),
            Properties = resultOfPropsMapping.AsT0,
            ImageFileNames = GetImageFileInfoCreateRequestsFromXmlData(product.ShopItemImages),
            Images = images,
            CategoryID = (short?)product.Category.Id,
            ManifacturerId = (short?)product.Manifacturer.Id,
            SearchString = product.SearchString,
        };
    }

    private async Task<OneOf<XmlProductCreateDisplay, ValidationResult>> GetDisplayProductFromXmlDataAsync(XmlProduct product, string xml)
    {
        List<CurrentProductImageCreateRequest> images = await GetImagesFromXmlDataAsync(product.ShopItemImages, xml);

        int? standardWarrantyTermMonths = GetWarrantyData(product.XmlProductProperties);

        XmlProductCreateDisplay output = new()
        {
            Id = product.Id,
            Name = product.Name,
            Status = product.Status,
            DisplayPrice = product.Price,
            Currency = CurrencyEnumMapping.GetCurrencyEnumFromString(product.CurrencyCode),
            Properties = new(),
            ImageFileNames = GetImageFileInfoCreateRequestsFromXmlData(product.ShopItemImages),
            Images = images,
            Category = Map(product.Category),
            Manifacturer = Map(product.Manifacturer),
            SearchString = product.SearchString,
            StandardWarrantyTermMonths = standardWarrantyTermMonths
        };

        for (int i = 0; i < product.XmlProductProperties!.Count; i++)
        {
            var xmlProp = product.XmlProductProperties[i];

            ProductCharacteristic? productCharacteristic = _productCharacteristicService.GetByCategoryIdAndName((uint)product.Category.Id, xmlProp.Name);

            // RETURN AFTER TESTS ================================================================================================================

            //if (productCharacteristic is null) return new ValidationResult(new List<ValidationFailure>
            //{
            //    new(nameof(XmlProductProperty.Name), "Name is not a valid name for a characteristic")
            //});

            output.Properties.Add(new()
            {
                ProductCharacteristicId = productCharacteristic?.Id,
                Name = xmlProp.Name,
                Meaning = productCharacteristic?.Meaning,
                Value = xmlProp.Value,
                DisplayOrder = int.Parse(xmlProp.Order),
                XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList,
            });
        }

        return output;

    }

    static int? GetWarrantyData(List<XmlProductProperty> xmlProductProperties)
    {
        string? standardWarrantyXmlData = xmlProductProperties.FirstOrDefault(prop => prop.Name == "Warranty")?.Value ?? null;

        if (standardWarrantyXmlData is null) return null;

        ReadOnlySpan<char> standartWarrantyTermMonthsData = standardWarrantyXmlData.AsSpan(0, standardWarrantyXmlData.IndexOf("Months") - 1);

        int? standardWarrantyTermMonths = int.Parse(standartWarrantyTermMonthsData);

        return standardWarrantyTermMonths;
    }

    private OneOf<List<CurrentProductPropertyCreateRequest>, ValidationResult> GetPropertyCreateRequestsFromXmlData(
        List<XmlProductProperty> properties,
        uint categoryId)
    {
        List<CurrentProductPropertyCreateRequest> output = new();

        for (int i = 0; i < properties.Count; i++)
        {
            XmlProductProperty property = properties[i];

            ProductCharacteristic? productCharacteristic = _productCharacteristicService.GetByCategoryIdAndName(categoryId, property.Name);

            // RETURN AFTER TESTS ================================================================================================================

            //if (productCharacteristic is null) return new ValidationResult(new List<ValidationFailure>
            //{
            //    new(nameof(XmlProductProperty.Name), "Name is not a valid name for a characteristic")
            //});

            CurrentProductPropertyCreateRequest request = new()
            {
                ProductCharacteristicId = productCharacteristic?.Id ?? 0, // REMOVE "?? 0" after tests
                DisplayOrder = int.Parse(property.Order),
                Value = property.Value,
                XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList
            };

            output.Add(request);
        }

        return output;
    }

    private static List<CurrentProductImageFileNameInfoCreateRequest> GetImageFileInfoCreateRequestsFromXmlData(List<XmlShopItemImage> images)
    {
        List<CurrentProductImageFileNameInfoCreateRequest> output = new();

        for (int i = 0; i < images.Count; i++)
        {
            XmlShopItemImage item = images[i];

            CurrentProductImageFileNameInfoCreateRequest imageFileNameInfo = new()
            {
                FileName = item.PictureUrl[(item.PictureUrl.LastIndexOf('/') + 1)..],
                DisplayOrder = i + 1,
            };

            output.Add(imageFileNameInfo);
        }

        return output;
    }

    private async Task<List<CurrentProductImageCreateRequest>> GetImagesFromXmlDataAsync(List<XmlShopItemImage> images, string xml)
    {
        List<CurrentProductImageCreateRequest> output = new();

        for (int i = 0; i < images.Count; i++)
        {
            XmlShopItemImage item = images[i];

            byte[] imageData = await _httpClient.GetByteArrayAsync(item.PictureUrl);

            CurrentProductImageCreateRequest imageFileNameInfo = new()
            {
                ImageData = imageData,
                ImageFileExtension = string.Concat("image/", item.PictureUrl.AsSpan(item.PictureUrl.LastIndexOf('.') + 1)),
                XML = xml,
            };

            output.Add(imageFileNameInfo);
        }

        return output;
    }

    private static Category Map(XmlShopItemCategory category)
    {
        return new()
        {
            Id = category.Id,
            Description = category.Name,
            ParentCategoryId = category.ParentCategoryId,
        };
    }

    private static Manifacturer Map(XmlManifacturer manifacturer)
    {
        return new()
        {
            Id = manifacturer.Id,
            RealCompanyName = manifacturer.Name,
        };
    }
}