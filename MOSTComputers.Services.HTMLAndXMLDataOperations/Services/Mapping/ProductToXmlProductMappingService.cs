using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Contracts;
using static MOSTComputers.Models.Product.MappingUtils.CurrencyEnumMapping;
using static MOSTComputers.Models.Product.MappingUtils.ProductStatusMapping;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Mapping;

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
            ShopItemImages = GetXmlImageDataFromImageFilePathInfos(product.Images, product.ImageFileNames, _initialImageFileNameInfoPath),
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

    private static List<XmlShopItemImage> GetXmlImageDataFromImageFilePathInfos(
        List<ProductImage>? productImages,
        List<ProductImageFileNameInfo>? productImageFileNameInfos,
        string initialPath)
    {
        List<XmlShopItemImage> output = new();

        if (productImages is null)
        {
            if (productImageFileNameInfos is null) return new();

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

        if (productImageFileNameInfos is null)
        {
            for (int i = 0; i < productImages.Count; i++)
            {
                ProductImage productImage = productImages[i];

                string? fileName = null;

                if (productImage.ImageContentType is not null)
                {
                    int indexOfSlashInFileType = productImage.ImageContentType.IndexOf('/');

                    if (indexOfSlashInFileType == -1)
                    {
                        fileName = $"{productImage.Id}";
                    }

                    string fileExtensionFromImageFileType = productImage.ImageContentType[(indexOfSlashInFileType + 1)..];

                    fileName = $"{productImage.Id}.{fileExtensionFromImageFileType}";
                }

                XmlShopItemImage xmlShopItem = new()
                {
                    PictureUrl = initialPath + fileName ?? "",
                    ThumbnailUrl = initialPath + fileName ?? "",
                    DisplayOrder = (short)(i)
                };

                output.Add(xmlShopItem);
            }

            return output;
        }

        for (int i = 0; i < productImages.Count; i++)
        {
            ProductImage productImage = productImages[i];

            if (productImage.ImageContentType is not null)
            {
                int indexOfSlashInFileType = productImage.ImageContentType.IndexOf('/');

                string fileExtensionFromImageFileType = productImage.ImageContentType[(indexOfSlashInFileType + 1)..];

                string fileName = $"{productImage.Id}.{fileExtensionFromImageFileType}";

                ProductImageFileNameInfo? relatedFileNameInfo = productImageFileNameInfos.Find(imageFileName =>
                    imageFileName.FileName == fileName);

                XmlShopItemImage xmlShopItem = new()
                {
                    PictureUrl = initialPath + fileName ?? "",
                    ThumbnailUrl = initialPath + fileName ?? "",
                    DisplayOrder = (short)(relatedFileNameInfo?.DisplayOrder ?? i)
                };

                output.Add(xmlShopItem);

                output = output.OrderBy(x => x.DisplayOrder)
                    .ToList();

                for (int k = 0; k < output.Count; k++)
                {
                    output[k].DisplayOrder = (short)(k + 1);
                }
            }
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