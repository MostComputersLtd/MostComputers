using FluentValidation.Results;
using MOSTComputers.Services.DAL;
using MOSTComputers.Services.DAL.Models;
using MOSTComputers.Services.DAL.Models.Requests.Product;
using MOSTComputers.Services.DAL.Models.Responses;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Services.XMLDataOperations.Models;
using MOSTComputers.Services.XMLDataOperations.Services;
using MOSTComputers.UI.Web.Mapping;
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

    public async Task<OneOf<List<ProductCreateRequest>, ValidationResult, UnexpectedFailureResult>> GetProductCreateRequestsFromXmlTextAsync(string xmlText)
    {
        XmlObjectData? xmlObjectData = _productDeserializeService.DeserializeProductsXml(xmlText);

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

    private async Task<OneOf<ProductCreateRequest, ValidationResult>> GetProductFromXmlDataAsync(XmlProduct product, string xml)
    {
        OneOf<List<CurrentProductPropertyCreateRequest>, ValidationResult> resultOfPropsMapping
            = GetPropertyCreateRequestsFromXmlData(product.xmlProductProperties, (uint)product.Category.Id);

        List<CurrentProductImageCreateRequest> images = await GetImagesFromXmlDataAsync(product.ShopItemImages, xml);

        if (resultOfPropsMapping.IsT1) return resultOfPropsMapping.AsT1;

        return new ProductCreateRequest()
        {
            Id = product.Id,
            SearchString = product.SearchString,
            Status = product.Status,
            Price1 = product.Price,
            Currency = CurrencyEnumMapping.GetCurrencyEnumFromString(product.CurrencyCode),
            Properties = resultOfPropsMapping.AsT0,
            ImageFileNames = GetImageFileInfoCreateRequestsFromXmlData(product.ShopItemImages),
            Images = images,
            CategoryID = (short?)product.Category.Id,
            ManifacturerId = (short?)product.Manifacturer.Id,
        };
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
}