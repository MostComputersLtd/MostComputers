using FluentValidation.Results;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Requests.Product;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Services.XMLDataOperations.Models;
using MOSTComputers.Services.XMLDataOperations.Services.Contracts;
using OneOf;
using MOSTComputers.Models.Product.MappingUtils;
using MOSTComputers.UI.Web.RealWorkTesting.Services.Contracts;

namespace MOSTComputers.UI.Web.RealWorkTesting.Services;

public class ProductXmlToCreateRequestMappingService : IProductXmlToCreateRequestMappingService
{
    public ProductXmlToCreateRequestMappingService(
        IProductCharacteristicService productCharacteristicService,
        IHttpClientFactory httpClientFactory,
        IProductDeserializeService productDeserializeService,
        IProductToXmlProductMappingService productToXmlProductMappingService)
    {
        _productCharacteristicService = productCharacteristicService;
        _httpClient = httpClientFactory.CreateClient();
        _productDeserializeService = productDeserializeService;
        _productToXmlProductMappingService = productToXmlProductMappingService;
    }

    private readonly IProductCharacteristicService _productCharacteristicService;
    private readonly HttpClient _httpClient;
    private readonly IProductDeserializeService _productDeserializeService;
    private readonly IProductToXmlProductMappingService _productToXmlProductMappingService;

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



    private async Task<OneOf<ProductCreateRequest, ValidationResult>> GetProductFromXmlDataAsync(XmlProduct product, string xml)
    {
        OneOf<List<CurrentProductPropertyCreateRequest>, ValidationResult> resultOfPropsMapping
            = GetPropertyCreateRequestsFromXmlData(product.XmlProductProperties, (uint)product.Category.Id);

        List<CurrentProductImageCreateRequest> images = await GetImagesFromXmlDataAsync(product.ShopItemImages, xml);

        int? standardWarrantyTermMonths = GetWarrantyData(product.XmlProductProperties);

        if (resultOfPropsMapping.IsT1) return resultOfPropsMapping.AsT1;

        (string? partNumber1, string? partNumber2) = GetPartNumbers(product);

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
            PartNumber1 = partNumber1,
            PartNumber2 = partNumber2,
        };
    }

    static int? GetWarrantyData(List<XmlProductProperty> xmlProductProperties)
    {
        string? standardWarrantyXmlData = xmlProductProperties.FirstOrDefault(prop => prop.Name == "Warranty")?.Value ?? null;

        if (standardWarrantyXmlData is null) return null;

        ReadOnlySpan<char> standartWarrantyTermMonthsData = standardWarrantyXmlData.AsSpan(0, standardWarrantyXmlData.IndexOf("Months") - 1);

        int? standardWarrantyTermMonths = int.Parse(standartWarrantyTermMonthsData);

        return standardWarrantyTermMonths;
    }

    private static (string? partNumber1, string? partNumber2) GetPartNumbers(XmlProduct product)
    {
        string partNumbers = product.PartNumbers;

        if (partNumbers.Length <= 0) return (null, null);

        int mediumLineIndex = partNumbers.IndexOf("/");

        string partNumber1 = partNumbers[..(mediumLineIndex - 2)];
        string partNumber2 = partNumbers[(mediumLineIndex + 2)..];

        return (partNumber1, partNumber2);
    }

    private OneOf<List<CurrentProductPropertyCreateRequest>, ValidationResult> GetPropertyCreateRequestsFromXmlData(
        List<XmlProductProperty> properties,
        uint categoryId)
    {
        List<CurrentProductPropertyCreateRequest> output = new();

        for (int i = 0; i < properties.Count; i++)
        {
            XmlProductProperty property = properties[i];

            ProductCharacteristic? productCharacteristic = _productCharacteristicService.GetByCategoryIdAndName((int)categoryId, property.Name);

            // RETURN AFTER TESTS ================================================================================================================

            //if (productCharacteristic is null) return new ValidationResult(new List<ValidationFailure>
            //{
            //    new(nameof(XmlProductProperty.Name), "Name is not a valid name for a characteristic")
            //});

            CurrentProductPropertyCreateRequest request = new()
            {
                ProductCharacteristicId = productCharacteristic?.Id, // REMOVE "?? 0" after tests
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
                HtmlData = xml,
            };

            output.Add(imageFileNameInfo);
        }

        return output;
    }

    public XmlObjectData GetXmlDataFromProducts(List<Product> products)
    {
        List<XmlProduct> xmlProducts = new();

        foreach (var product in products)
        {
            XmlProduct xmlProduct = _productToXmlProductMappingService.MapToXmlProduct(product);

            xmlProducts.Add(xmlProduct);
        }

        XmlObjectData output = new()
        {
            DateOfExport = DateTime.Today,
            Products = xmlProducts,
            TotalNumberOfItems = products.Count,
        };

        return output;
    }
}