using FluentValidation.Results;
using MOSTComputers.Models.Product.MappingUtils;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Services.XMLDataOperations.Models;
using MOSTComputers.UI.Web.RealWorkTesting.Services.Contracts;
using OneOf;

namespace MOSTComputers.UI.Web.RealWorkTesting.Services;

public class ProductXmlToProductMappingService : IProductXmlToProductMappingService
{
    private readonly IProductCharacteristicService _productCharacteristicService;
    private readonly IHttpClientFactory _httpClientFactory;

    public ProductXmlToProductMappingService(
        IProductCharacteristicService productCharacteristicService,
        IHttpClientFactory httpClientFactory)
    {
        _productCharacteristicService = productCharacteristicService;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<OneOf<Product, ValidationResult>> GetProductFromXmlDataAsync(XmlProduct product, string xmlForImages)
    {
        uint productId = (uint)product.Id;

        OneOf<List<ProductProperty>, ValidationResult> resultOfPropsMapping
            = GetPropertiesFromXmlData(product.XmlProductProperties, productId, (uint)product.Category.Id);

        List<Tuple<ProductImage, ProductImageFileNameInfo>> imagesAndImageFileNames
            = await GetImagesAndImageFileNameInfosFromXmlDataAsync(product.ShopItemImages, xmlForImages, productId);

        int? standardWarrantyTermMonths = GetWarrantyData(product.XmlProductProperties);

        if (resultOfPropsMapping.IsT1) return resultOfPropsMapping.AsT1;

        (string? partNumber1, string? partNumber2) = GetPartNumbers(product);

        return new Product()
        {
            Id = product.Id,
            Name = product.Name,
            Status = product.Status,
            StandardWarrantyTermMonths = standardWarrantyTermMonths,
            Price = product.Price,
            Currency = CurrencyEnumMapping.GetCurrencyEnumFromString(product.CurrencyCode),
            Properties = resultOfPropsMapping.AsT0,
            Images = imagesAndImageFileNames.Select(x => x.Item1)
                .ToList(),
            ImageFileNames = imagesAndImageFileNames.Select(x => x.Item2)
                .ToList(),
            CategoryID = (short?)product.Category.Id,
            Category = Map(product.Category),
            ManifacturerId = (short?)product.Manifacturer.Id,
            Manifacturer = Map(product.Manifacturer),
            SearchString = product.SearchString,
            PartNumber1 = partNumber1,
            PartNumber2 = partNumber2,
        };
    }

    private static int? GetWarrantyData(List<XmlProductProperty> xmlProductProperties)
    {
        string? standardWarrantyXmlData
            = xmlProductProperties.FirstOrDefault(prop => prop.Name == "Warranty")?.Value ?? null;

        if (standardWarrantyXmlData is null) return null;

        ReadOnlySpan<char> standardWarrantyTermMonthsData
            = standardWarrantyXmlData.AsSpan(0, standardWarrantyXmlData.IndexOf("Months") - 1);

        int? standardWarrantyTermMonths = int.Parse(standardWarrantyTermMonthsData);

        return standardWarrantyTermMonths;
    }

    private static (string? partNumber1, string? partNumber2) GetPartNumbers(XmlProduct product)
    {
        string partNumbers = product.PartNumbers;

        if (partNumbers.Length <= 0) return (null, null);

        int mediumLineIndex = partNumbers.IndexOf('/');

        string partNumber1 = partNumbers[..(mediumLineIndex - 2)];
        string partNumber2 = partNumbers[(mediumLineIndex + 2)..];

        return (partNumber1, partNumber2);
    }

    private OneOf<List<ProductProperty>, ValidationResult> GetPropertiesFromXmlData(
        List<XmlProductProperty> properties,
        uint productId,
        uint categoryId)
    {
        int productIdInt = (int)productId;

        List<ProductProperty> output = new();

        for (int i = 0; i < properties.Count; i++)
        {
            XmlProductProperty property = properties[i];

            ProductCharacteristic? productCharacteristic
                = _productCharacteristicService.GetByCategoryIdAndNameAndCharacteristicType((int)categoryId, property.Name, ProductCharacteristicTypeEnum.ProductCharacteristic);

            // RETURN AFTER TESTS ================================================================================================================

            //if (productCharacteristic is null) return new ValidationResult(new List<ValidationFailure>
            //{
            //    new(nameof(XmlProductProperty.Name), "Name is not a valid name for a characteristic")
            //});

            bool orderParseSuccess = int.TryParse(property.Order, out int order);

            ProductProperty request = new()
            {
                ProductCharacteristicId = productCharacteristic?.Id, // REMOVE "?? 0" after tests
                Characteristic = property.Name,
                DisplayOrder = orderParseSuccess ? order : null,
                Value = property.Value,
                XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList,
                ProductId = productIdInt,
            };

            output.Add(request);
        }

        return output;
    }

    private static List<ProductImageFileNameInfo> GetImageFileInfosFromXmlData(List<XmlShopItemImage> images, uint productId)
    {
        List<ProductImageFileNameInfo> output = new();

        for (int i = 0; i < images.Count; i++)
        {
            XmlShopItemImage item = images[i];

            ProductImageFileNameInfo imageFileNameInfo = new()
            {
                FileName = item.PictureUrl[(item.PictureUrl.LastIndexOf('/') + 1)..],
                DisplayOrder = i + 1,
                Active = true,
                ProductId = (int)productId
            };

            output.Add(imageFileNameInfo);
        }

        return output;
    }

    private async Task<List<Tuple<ProductImage, ProductImageFileNameInfo>>> GetImagesAndImageFileNameInfosFromXmlDataAsync(
        List<XmlShopItemImage> images,
        string xml,
        uint productId)
    {
        List<Tuple<ProductImage, ProductImageFileNameInfo>> output = new();

        using HttpClient httpClient = _httpClientFactory.CreateClient();

        for (int i = 0; i < images.Count; i++)
        {
            XmlShopItemImage item = images[i];

            ProductImageFileNameInfo imageFileNameInfo = new()
            {
                FileName = item.PictureUrl[(item.PictureUrl.LastIndexOf('/') + 1)..],
                DisplayOrder = i + 1,
                Active = true,
                ProductId = (int)productId
            };

            byte[]? imageData = null;

            HttpResponseMessage imageDataResponse = await httpClient.GetAsync(item.PictureUrl);

            if (imageDataResponse.IsSuccessStatusCode)
            {
                imageData = await imageDataResponse.Content.ReadAsByteArrayAsync();
            }

            string imageIdAsString = imageFileNameInfo.FileName[..imageFileNameInfo.FileName.IndexOf('.')];

            bool isImageIdParseSuccessful = int.TryParse(imageIdAsString, out int imageId);

            ProductImage image = new()
            {
                Id = isImageIdParseSuccessful ? imageId : 0,
                ImageData = imageData,
                ImageFileExtension = string.Concat("image/", item.PictureUrl.AsSpan(item.PictureUrl.LastIndexOf('.') + 1)),
                HtmlData = xml,
                ProductId = (int)productId,
            };

            output.Add(new(image, imageFileNameInfo));
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
