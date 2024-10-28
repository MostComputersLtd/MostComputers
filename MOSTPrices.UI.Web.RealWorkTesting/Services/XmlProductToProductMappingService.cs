using FluentValidation.Results;
using MOSTComputers.Models.Product.MappingUtils;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models;
using MOSTComputers.UI.Web.RealWorkTesting.Services.Contracts;
using OneOf;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Contracts;
using OneOf.Types;

namespace MOSTComputers.UI.Web.RealWorkTesting.Services;

public class XmlProductToProductMappingService : IXmlProductToProductMappingService
{
    public XmlProductToProductMappingService(
        IProductCharacteristicService productCharacteristicService,
        IHttpClientFactory httpClientFactory,
        IProductHtmlService productHtmlService)
    {
        _productCharacteristicService = productCharacteristicService;
        _httpClientFactory = httpClientFactory;
        _productHtmlService = productHtmlService;
    }

    private readonly IProductCharacteristicService _productCharacteristicService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IProductHtmlService _productHtmlService;

    public async Task<OneOf<Product, ValidationResult, InvalidXmlResult>> GetProductFromXmlDataAsync(XmlProduct product)
    {
        int productId = product.Id;

        OneOf<List<ProductProperty>, ValidationResult> resultOfPropsMapping
            = GetPropertiesFromXmlData(product.XmlProductProperties, productId, product.Category.Id);

        List<Tuple<ProductImage, ProductImageFileNameInfo>> imagesAndImageFileNames
            = await GetImagesAndImageFileNameInfosFromXmlDataAsync(product.ShopItemImages, productId);

        int? standardWarrantyTermMonths = GetWarrantyData(product.XmlProductProperties);

        if (resultOfPropsMapping.IsT1) return resultOfPropsMapping.AsT1;

        OneOf<Manifacturer, ValidationResult> manifacturerResult = GetManifacturerFromXmlData(product.Manifacturer);

        if (manifacturerResult.IsT1) return manifacturerResult.AsT1;


        (string? partNumber1, string? partNumber2) = GetPartNumbers(product);

        Product output = new()
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
            CategoryId = (short?)product.Category.Id,
            Category = Map(product.Category),
            ManifacturerId = (short?)product.Manifacturer.Id,
            Manifacturer = manifacturerResult.AsT0,
            SearchString = product.SearchString,
            PartNumber1 = partNumber1,
            PartNumber2 = partNumber2,
        };

        OneOf<Success, InvalidXmlResult> alterImageHtmlDataResult = TryAlterImageHtmlData(output);

        return alterImageHtmlDataResult.Match<OneOf<Product, ValidationResult, InvalidXmlResult>>(
            success => output,
            invalidXmlResult => invalidXmlResult);
    }

    private static int? GetWarrantyData(List<XmlProductProperty> xmlProductProperties)
    {
        string? standardWarrantyXmlData
            = xmlProductProperties.FirstOrDefault(prop => prop.Name == "Warranty")?.Value ?? null;

        if (standardWarrantyXmlData is null) return null;

        int monthsWordStartIndex = standardWarrantyXmlData.IndexOf("Months", StringComparison.CurrentCultureIgnoreCase);

        if (monthsWordStartIndex < 0)
        {
            monthsWordStartIndex = standardWarrantyXmlData.IndexOf("Месеца", StringComparison.CurrentCultureIgnoreCase);
        }

        if (monthsWordStartIndex < 0) return null;

        ReadOnlySpan<char> standardWarrantyTermMonthsData
            = standardWarrantyXmlData.AsSpan(0, monthsWordStartIndex - 1);

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
        int productId,
        int categoryId)
    {
        List<ProductProperty> output = new();

        for (int i = 0; i < properties.Count; i++)
        {
            XmlProductProperty property = properties[i];

            ProductCharacteristic? productCharacteristic = _productCharacteristicService.GetByCategoryIdAndNameAndCharacteristicType(
                categoryId, property.Name, ProductCharacteristicTypeEnum.ProductCharacteristic);

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
                ProductId = productId,
            };

            output.Add(request);
        }

        return output;
    }

    private async Task<List<Tuple<ProductImage, ProductImageFileNameInfo>>> GetImagesAndImageFileNameInfosFromXmlDataAsync(
        List<XmlShopItemImage> images,
        int productId)
    {
        List<Tuple<ProductImage, ProductImageFileNameInfo>> output = new();

        using HttpClient httpClient = _httpClientFactory.CreateClient();

        for (int i = 0; i < images.Count; i++)
        {
            XmlShopItemImage item = images[i];

            if (string.IsNullOrWhiteSpace(item.PictureUrl)) continue;

            int indexOfLastSlashInImageUrl = item.PictureUrl.LastIndexOf('/');

            string imageFileName = item.PictureUrl[(indexOfLastSlashInImageUrl + 1)..];

            ProductImageFileNameInfo imageFileNameInfo = new()
            {
                FileName = imageFileName,
                DisplayOrder = i + 1,
                Active = true,
                ProductId = productId
            };

            byte[]? imageData = null;

            HttpResponseMessage imageDataResponse = await httpClient.GetAsync(item.PictureUrl);

            if (imageDataResponse.IsSuccessStatusCode)
            {
                imageData = await imageDataResponse.Content.ReadAsByteArrayAsync();
            }

            string imageIdAsString = imageFileNameInfo.FileName[..imageFileNameInfo.FileName.IndexOf('.')];

            bool isImageIdParseSuccessful = int.TryParse(imageIdAsString, out int imageId);

            string fileExtensionWithDot = Path.GetExtension(item.PictureUrl);

            ProductImage image = new()
            {
                Id = isImageIdParseSuccessful ? imageId : 0,
                ImageData = imageData,
                ImageContentType = string.Concat("image/", fileExtensionWithDot[1..]),
                ProductId = productId,
            };

            output.Add(new(image, imageFileNameInfo));
        }

        return output;
    }

    private OneOf<Success, InvalidXmlResult> TryAlterImageHtmlData(Product product)
    {
        if (product.Images is null
            || product.Images.Count <= 0)
        {
            return new Success();
        }

        OneOf<string, InvalidXmlResult> gethtmlDataFromProductResult = _productHtmlService.TryGetHtmlFromProduct(product);

        return gethtmlDataFromProductResult.Match<OneOf<Success, InvalidXmlResult>>(
            htmlDataFromProduct =>
            {
                foreach (ProductImage image in product.Images)
                {
                    image.HtmlData = htmlDataFromProduct;
                }

                return new Success();
            },
            invalidXmlResult => invalidXmlResult);
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

    private static OneOf<Manifacturer, ValidationResult> GetManifacturerFromXmlData(XmlManifacturer manifacturer)
    {
        if (manifacturer.Id is null)
        {
            ValidationResult validationResult = new();

            validationResult.Errors.Add(new(nameof(XmlManifacturer.Id), "Id cannot be null."));

            return validationResult;
        }

        return new Manifacturer()
        {
            Id = manifacturer.Id.Value,
            RealCompanyName = manifacturer.Name,
        };
    }
}
