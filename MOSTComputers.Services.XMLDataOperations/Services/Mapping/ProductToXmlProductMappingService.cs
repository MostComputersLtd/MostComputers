using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.XMLDataOperations.Models;
using MOSTComputers.Services.XMLDataOperations.Services.Contracts;
using static MOSTComputers.Models.Product.MappingUtils.CurrencyEnumMapping;
using static MOSTComputers.Models.Product.MappingUtils.ProductStatusMapping;

namespace MOSTComputers.Services.XMLDataOperations.Services.Mapping;

internal sealed class ProductToXmlProductMappingService : IProductToXmlProductMappingService
{
    const string _initialImageFileNameInfoPath = "http://www.mostcomputers.bg/upload/";

    public XmlProduct MapToXmlProduct(Product product)
    {
        return new()
        {
            Id = product.Id,
            UId = 0,
            Name = product.Name ?? string.Empty,
            StatusString = GetBGStatusStringFromStatusEnum(product.Status) ?? string.Empty,
            Price = product.Price ?? 0,
            Code = product.Name ?? string.Empty,
            CurrencyCode = GetStringFromCurrencyEnum(product.Currency) ?? string.Empty,
            SearchString = product.SearchString ?? string.Empty,
            ShopItemImages = product.ImageFileNames is not null ? GetXmlImageDataFromImageFilePathInfos(product.ImageFileNames, _initialImageFileNameInfoPath) : new(),
            PartNumbers = GetPartNumberXmlStringFromPartNumbers(product.PartNumber1, product.PartNumber2),
            Category = product.Category is not null ? Map(product.Category) : new(),
            Manifacturer = product.Manifacturer is not null ? Map(product.Manifacturer) : new(),
            XmlProductProperties = product.Properties is not null ? GetXmlPropertiesFromProductProperties(product.Properties) : new()
        };
    }

    private static string GetPartNumberXmlStringFromPartNumbers(string? partNumber1, string? partNumber2)
    {
        if (partNumber1 is null)
        {
            if (partNumber2 is null) return string.Empty;

            return $"/{partNumber2}";
        }

        if (partNumber2 is null) return $"{partNumber1}/";

        return $"{partNumber1}/{partNumber2}";
    }

    private static XmlShopItemCategory Map(Category category)
    {
        return new()
        {
            Id = category.Id,
            Name = category.Description ?? string.Empty,
            ParentCategoryId = category.ParentCategoryId,
        };
    }

    private static XmlManifacturer Map(Manifacturer manifacturer)
    {
        return new()
        {
            Id = manifacturer.Id,
            Name = manifacturer.RealCompanyName ?? string.Empty,
        };
    }

    private static List<XmlShopItemImage> GetXmlImageDataFromImageFilePathInfos(List<ProductImageFileNameInfo> productImageFileNameInfos, string initialPath)
    {
        List<XmlShopItemImage> output = new();

        foreach (ProductImageFileNameInfo fileNameInfo in productImageFileNameInfos)
        {
            XmlShopItemImage xmlShopItem = new()
            {
                PictureUrl = initialPath + fileNameInfo.FileName,
                ThumbnailUrl = initialPath + fileNameInfo.FileName,
                DisplayOrder = (short)(fileNameInfo.DisplayOrder ?? 0)
            };

            output.Add(xmlShopItem);
        }

        return output;
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