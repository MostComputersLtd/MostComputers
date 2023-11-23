using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.Rendering;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Requests.Product;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Services.XMLDataOperations.Models;
using MOSTComputers.Services.XMLDataOperations.Services;
using MOSTComputers.UI.Web.Mapping;
using MOSTComputers.UI.Web.Models;
using OneOf;
using System.Xml;

namespace MOSTComputers.UI.Web.Services;

public class ProductXmlToCreateRequestMapperService
{
    public ProductXmlToCreateRequestMapperService(
        IProductCharacteristicService productCharacteristicService,
        IHttpClientFactory httpClientFactory,
        ProductDeserializeService productDeserializeService,
        IFailedPropertyNameOfProductService failedPropertyNameOfProductService)
    {
        _productCharacteristicService = productCharacteristicService;
        _httpClient = httpClientFactory.CreateClient();
        _productDeserializeService = productDeserializeService;
        _failedPropertyNameOfProductService = failedPropertyNameOfProductService;
    }

    private readonly IProductCharacteristicService _productCharacteristicService;
    private readonly HttpClient _httpClient;
    private readonly ProductDeserializeService _productDeserializeService;
    private readonly IFailedPropertyNameOfProductService _failedPropertyNameOfProductService;

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

        OneOf<List<XmlProductCreateDisplay>, ValidationResult> getDisplayProductsResult = await GetDisplayProductsFromXmlDataAsync(xmlObjectData.Products, xmlText);

        return getDisplayProductsResult.Match<OneOf<List<XmlProductCreateDisplay>, ValidationResult, InvalidXmlResult, UnexpectedFailureResult>>(
            items => items,
            validationResult => validationResult);
    }

    public async Task<OneOf<List<XmlProductCreateDisplay>, ValidationResult>> GetProductXmlDisplayFromXmlAsync(XmlObjectData xmlObjectData, string xmlTextForImages)
    {
        return await GetDisplayProductsFromXmlDataAsync(xmlObjectData.Products, xmlTextForImages);
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

    public ProductCreateRequest GetProductCreateRequestFromProductXmlDisplay(XmlProductCreateDisplay display)
    {
        ProductCreateRequest output = new()
        {
            Name = display.Name,
            AdditionalWarrantyPrice = display.AdditionalWarrantyPrice,
            AdditionalWarrantyTermMonths = display.AdditionalWarrantyTermMonths,
            StandardWarrantyPrice = display.StandardWarrantyPrice,
            StandardWarrantyTermMonths = display.StandardWarrantyTermMonths,
            DisplayOrder = display.DisplayOrder,
            Status = display.Status,
            PlShow = display.PlShow,
            Price1 = display.Price1,
            DisplayPrice = display.DisplayPrice,
            Price3 = display.Price3,
            Currency = display.Currency,
            RowGuid = display.RowGuid,
            Promotionid = display.Promotionid,
            PromRid = display.PromRid,
            PromotionPictureId = display.PromotionPictureId,
            PromotionExpireDate = display.PromotionExpireDate,
            AlertPictureId = display.AlertPictureId,
            AlertExpireDate = display.AlertExpireDate,
            PriceListDescription = display.PriceListDescription,
            PartNumber1 = display.PartNumber1,
            PartNumber2 = display.PartNumber2,
            SearchString = display.SearchString,

            Properties = new(),
            Images = display.Images,
            ImageFileNames = display.ImageFileNames,

            CategoryID = display.Category?.Id,
            ManifacturerId = (short?)display.Manifacturer?.Id,
            SubCategoryId = display.SubCategoryId
        };

        for (int i = 0; i < display.Properties!.Count; i++)
        {
            var prop = display.Properties[i];

            output.Properties.Add(new()
            {
                ProductCharacteristicId = prop.ProductCharacteristicId,
                Value = prop.Value,
                DisplayOrder = prop.DisplayOrder,
                XmlPlacement = prop.XmlPlacement,
            });
        }

        return output;
    }

    public List<XmlProductCreateDisplay> GetProductXmlDisplayFromProductData(List<Tuple<XmlProduct, ProductCreateRequest>> xmlProductsAndRequests)
    {
        List<XmlProductCreateDisplay> output = new();

        Dictionary<uint, List<XmlProductCreateDisplay>> neededCategoriesAndRequestsToBeUpdatedForThem = new();

        foreach (Tuple<XmlProduct, ProductCreateRequest> tuple in xmlProductsAndRequests)
        {
            (XmlProduct xmlProduct, ProductCreateRequest createRequest) = tuple;

            XmlProductCreateDisplay item = new()
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

            bool shouldGatherAdditionalCharacteristicsData = false;

            for (int i = 0; i < createRequest.Properties!.Count; i++)
            {
                var prop = createRequest.Properties[i];

                var xmlProp = xmlProduct.XmlProductProperties[i];

                item.Properties.Add(new()
                {
                    ProductCharacteristicId = prop.ProductCharacteristicId,
                    Name = xmlProp.Name,
                    Value = prop.Value,
                    DisplayOrder = prop.DisplayOrder,
                    XmlPlacement = prop.XmlPlacement,
                });

                if (prop.ProductCharacteristicId == 0)
                {
                    _failedPropertyNameOfProductService.Insert(new() { ProductId = (uint)item.Id, PropertyName = xmlProp.Name });

                    shouldGatherAdditionalCharacteristicsData = true;
                }
            }

            if (!shouldGatherAdditionalCharacteristicsData
                || createRequest.CategoryID is null
                || createRequest.CategoryID <= 0) continue;

            uint value = (uint)createRequest.CategoryID.Value;

            if (!neededCategoriesAndRequestsToBeUpdatedForThem.ContainsKey(value))
            {
                neededCategoriesAndRequestsToBeUpdatedForThem.Add(value, new() { item });

                continue;
            }

            neededCategoriesAndRequestsToBeUpdatedForThem[value].Add(item);
        }

        IEnumerable<uint> categoryIds = neededCategoriesAndRequestsToBeUpdatedForThem
            .Select(x => x.Key);

        IEnumerable<Tuple<uint, IEnumerable<ProductCharacteristic>>> characteristicsForNeededCategories = _productCharacteristicService.GetAllForSelectionOfCategoryIds(categoryIds)
            .Select(x => Tuple.Create(x.Key, x.AsEnumerable()));

        foreach (var grouping in characteristicsForNeededCategories)
        {
            foreach (XmlProductCreateDisplay productRequest in neededCategoriesAndRequestsToBeUpdatedForThem[grouping.Item1])
            {
                productRequest.Characteristics = grouping.Item2.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() });
            }
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

    private async Task<OneOf<List<XmlProductCreateDisplay>, ValidationResult>> GetDisplayProductsFromXmlDataAsync(IEnumerable<XmlProduct> products, string xml)
    {
        List<XmlProductCreateDisplay> output = new();

        Dictionary<uint, List<XmlProductCreateDisplay>> neededCategoriesAndRequestsToBeUpdatedForThem = new();

        foreach (XmlProduct product in products)
        {
            if (product.Category is null
                || product.Category.Id < 0)
            {
                ValidationResult validationResult = new();

                validationResult.Errors.Add(new(nameof(XmlProduct.Category), "Product does not have a category"));

                return validationResult;
            }

            List<CurrentProductImageCreateRequest> images = await GetImagesFromXmlDataAsync(product.ShopItemImages, xml);

            int? standardWarrantyTermMonths = GetWarrantyData(product.XmlProductProperties);

            (string? partNumber1, string? partNumber2) = GetPartNumbers(product);

            OneOf<List<DisplayPropertyCreateRequest>, ValidationResult> getPropertyDataResult = GetPropertyDataForDisplay(product, (uint)product.Id, out bool needsToGetCharacteristics);

            getPropertyDataResult.TryPickT0(out List<DisplayPropertyCreateRequest> propsData, out ValidationResult propertyValidationResult);

            if (propertyValidationResult != null && !propertyValidationResult.IsValid) return propertyValidationResult;

            XmlProductCreateDisplay item = new()
            {
                Id = product.Id,
                Name = product.Name,
                Status = product.Status,
                StandardWarrantyTermMonths = standardWarrantyTermMonths,
                DisplayPrice = product.Price,
                Currency = CurrencyEnumMapping.GetCurrencyEnumFromString(product.CurrencyCode),
                Properties = propsData,
                ImageFileNames = GetImageFileInfoCreateRequestsFromXmlData(product.ShopItemImages),
                Images = images,
                Category = Map(product.Category),
                Manifacturer = Map(product.Manifacturer),
                PartNumber1 = partNumber1,
                PartNumber2 = partNumber2,
                SearchString = product.SearchString,
            };

            output.Add(item);

            if (!needsToGetCharacteristics
                || item.Category.Id <= 0) continue;

            uint value = (uint)item.Category.Id;

            if (!neededCategoriesAndRequestsToBeUpdatedForThem.ContainsKey(value))
            {
                neededCategoriesAndRequestsToBeUpdatedForThem.Add(value, new() { item });

                continue;
            }

            neededCategoriesAndRequestsToBeUpdatedForThem[value].Add(item);
        }

        IEnumerable<uint> categoryIds = neededCategoriesAndRequestsToBeUpdatedForThem
            .Select(x => x.Key);

        IEnumerable<Tuple<uint, IEnumerable<ProductCharacteristic>>> characteristicsForNeededCategories = _productCharacteristicService.GetAllForSelectionOfCategoryIds(categoryIds)
            .Select(x => Tuple.Create(x.Key, x.AsEnumerable()));

        foreach (Tuple<uint, IEnumerable<ProductCharacteristic>> grouping in characteristicsForNeededCategories)
        {
            foreach (XmlProductCreateDisplay productRequest in neededCategoriesAndRequestsToBeUpdatedForThem[grouping.Item1])
            {
                productRequest.Characteristics = grouping.Item2
                    .Distinct()
                    .Where(characteristic =>
                    {
                        if (string.IsNullOrEmpty(characteristic.Name)) return false;

                        return productRequest.Properties?.FirstOrDefault(
                            property => ComparePropertyAndCharacteristicNames(characteristic.Name, property.Name)) is null;
                    })
                    .Select(characteristic => new SelectListItem() { Text = characteristic.Name, Value = characteristic.Id.ToString() });
            }
        }

        return output;
    }

    private OneOf<List<DisplayPropertyCreateRequest>, ValidationResult> GetPropertyDataForDisplay(XmlProduct product, uint productId, out bool needsToGetCharacteristics)
    {
        List<DisplayPropertyCreateRequest> output = new();

        HashSet<string>? failedPropertyNames = null;

        List<string> propertyNames = product.XmlProductProperties.Select(x => x.Name).ToList();

        needsToGetCharacteristics = false;

        IEnumerable<ProductCharacteristic> productCharacteristics = _productCharacteristicService.GetSelectionByCategoryIdAndNames((uint)product.Category.Id, propertyNames);

        for (int i = 0; i < product.XmlProductProperties!.Count; i++)
        {
            var xmlProp = product.XmlProductProperties[i];

            ProductCharacteristic? productCharacteristic = productCharacteristics.FirstOrDefault(x => ComparePropertyAndCharacteristicNames(x?.Name, xmlProp.Name));

            if (productCharacteristic is null)
            {
                needsToGetCharacteristics = true;
            }

            output.Add(new()
            {
                ProductCharacteristicId = productCharacteristic?.Id,
                Name = xmlProp.Name,
                Meaning = productCharacteristic?.Meaning,
                Value = xmlProp.Value,
                DisplayOrder = int.Parse(xmlProp.Order),
                XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList,
            });

            if (productCharacteristic is null)
            {
                failedPropertyNames ??= new();

                failedPropertyNames.Add(xmlProp.Name);
            }

            // RETURN AFTER TESTS ================================================================================================================

            //if (productCharacteristic is null) return new ValidationResult(new List<ValidationFailure>
            //{
            //    new(nameof(XmlProductProperty.Name), "Name is not a valid name for a characteristic")
            //});
        }

        if (failedPropertyNames is not null
            && failedPropertyNames.Count > 0)
        {
            _failedPropertyNameOfProductService.MultiInsert(new() { ProductId = productId, PropertyNames = failedPropertyNames });
        }

        return output;
    }

    internal static bool ComparePropertyAndCharacteristicNames(string? characteristicName, string? propertyName)
    {
        if (characteristicName is null && propertyName is null) return true;

        if (characteristicName is null || propertyName is null) return false;

        return characteristicName.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase);
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

            ProductCharacteristic? productCharacteristic = _productCharacteristicService.GetByCategoryIdAndName(categoryId, property.Name);

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