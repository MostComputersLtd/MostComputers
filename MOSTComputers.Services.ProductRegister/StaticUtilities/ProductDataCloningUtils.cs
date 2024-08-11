using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.ProductStatuses;

namespace MOSTComputers.Services.ProductRegister.StaticUtilities;
internal static class ProductDataCloningUtils
{
    public static Category Clone(Category category)
    {
        return new()
        {
            Id = category.Id,
            Description = category.Description,
            DisplayOrder = category.DisplayOrder,
            ParentCategoryId = category.ParentCategoryId,
            IsLeaf = category.IsLeaf,
            ProductsUpdateCounter = category.ProductsUpdateCounter,
            RowGuid = category.RowGuid,
        };
    }

    public static Manifacturer Clone(Manifacturer manifacturer)
    {
        return new()
        {
            Id = manifacturer.Id,
            RealCompanyName = manifacturer.RealCompanyName,
            BGName = manifacturer.BGName,
            DisplayOrder = manifacturer.DisplayOrder,
            Active = manifacturer.Active,
        };
    }

    public static ProductImage Clone(ProductImage productImage)
    {
        return new()
        {
            Id = productImage.Id,
            ProductId = productImage.ProductId,
            ImageData = productImage.ImageData,
            ImageContentType = productImage.ImageContentType,
            HtmlData = productImage.HtmlData,
            DateModified = productImage.DateModified,
        };
    }

    public static ProductImageFileNameInfo Clone(ProductImageFileNameInfo productImageFileName)
    {
        return new()
        {
            ProductId = productImageFileName.ProductId,
            ImageNumber = productImageFileName.ImageNumber,
            DisplayOrder = productImageFileName.DisplayOrder,
            FileName = productImageFileName.FileName,
            Active = productImageFileName.Active,
        };
    }

    public static ProductProperty Clone(ProductProperty productProperty)
    {
        return new()
        {
            ProductId = productProperty.ProductId,
            ProductCharacteristicId = productProperty.ProductCharacteristicId,
            Characteristic = productProperty.Characteristic,
            Value = productProperty.Value,
            DisplayOrder = productProperty.DisplayOrder,
            XmlPlacement = productProperty.XmlPlacement,
        };
    }

    public static ProductCharacteristic Clone(ProductCharacteristic productCharacteristic)
    {
        return new()
        {
            Id = productCharacteristic.Id,
            PKUserId = productCharacteristic.PKUserId,
            CategoryId = productCharacteristic.CategoryId,
            Name = productCharacteristic.Name,
            Meaning = productCharacteristic.Meaning,
            DisplayOrder = productCharacteristic.DisplayOrder,
            Active = productCharacteristic.Active,
            KWPrCh = productCharacteristic.KWPrCh,
            LastUpdate = productCharacteristic.LastUpdate,
        };
    }

    public static ProductStatuses Clone(ProductStatuses productStatuses)
    {
        return new()
        {
            ProductId = productStatuses.ProductId,
            IsProcessed = productStatuses.IsProcessed,
            NeedsToBeUpdated = productStatuses.NeedsToBeUpdated,
        };
    }

    public static Product Clone(Product product)
    {
        return new()
        {
            Id = product.Id,
            Name = product.Name,
            AdditionalWarrantyPrice = product.AdditionalWarrantyPrice,
            AdditionalWarrantyTermMonths = product.AdditionalWarrantyTermMonths,
            StandardWarrantyPrice = product.StandardWarrantyPrice,
            StandardWarrantyTermMonths = product.StandardWarrantyTermMonths,
            DisplayOrder = product.DisplayOrder,
            Status = product.Status,
            PlShow = product.PlShow,
            Price = product.Price,
            Currency = product.Currency,
            RowGuid = product.RowGuid,
            Promotionid = product.Promotionid,
            PromRid = product.PromRid,
            PromotionPictureId = product.PromotionPictureId,
            PromotionExpireDate = product.PromotionExpireDate,
            AlertPictureId = product.AlertPictureId,
            AlertExpireDate = product.AlertExpireDate,
            PriceListDescription = product.PriceListDescription,
            PartNumber1 = product.PartNumber1,
            PartNumber2 = product.PartNumber2,
            SearchString = product.SearchString,
            CategoryID = product.CategoryID,
            Category = product.Category is not null ? Clone(product.Category) : null,
            ManifacturerId = product.ManifacturerId,
            Manifacturer = product.Manifacturer is not null ? Clone(product.Manifacturer) : null,
            SubCategoryId = product.SubCategoryId,

            Properties = product.Properties is not null ? CloneAll(product.Properties) : new(),
            Images = product.Images is not null ? CloneAll(product.Images) : null,
            ImageFileNames = product.ImageFileNames is not null ? CloneAll(product.ImageFileNames) : null,
        };
    }

    public static List<Category> CloneAll(IEnumerable<Category> categories)
    {
        List<Category> output = new();

        foreach (Category category in categories)
        {
            Category categoryClone = Clone(category);

            output.Add(categoryClone);
        }

        return output;
    }

    public static List<Manifacturer> CloneAll(IEnumerable<Manifacturer> manifacturers)
    {
        List<Manifacturer> output = new();

        foreach (Manifacturer manifacturer in manifacturers)
        {
            Manifacturer manifacturerClone = Clone(manifacturer);

            output.Add(manifacturerClone);
        }

        return output;
    }

    public static List<ProductImage> CloneAll(IEnumerable<ProductImage> productImages)
    {
        List<ProductImage> output = new();

        foreach (ProductImage productImage in productImages)
        {
            ProductImage productImageClone = Clone(productImage);

            output.Add(productImageClone);
        }

        return output;
    }

    public static List<ProductImageFileNameInfo> CloneAll(IEnumerable<ProductImageFileNameInfo> productImageFileNames)
    {
        List<ProductImageFileNameInfo> output = new();

        foreach (ProductImageFileNameInfo productImageFileName in productImageFileNames)
        {
            ProductImageFileNameInfo productImageFileNameClone = Clone(productImageFileName);

            output.Add(productImageFileNameClone);
        }

        return output;
    }

    public static List<ProductProperty> CloneAll(IEnumerable<ProductProperty> productProperties)
    {
        List<ProductProperty> output = new();

        foreach (ProductProperty productProperty in productProperties)
        {
            ProductProperty productPropertyClone = Clone(productProperty);

            output.Add(productPropertyClone);
        }

        return output;
    }

    public static List<ProductCharacteristic> CloneAll(IEnumerable<ProductCharacteristic> productCharacteristics)
    {
        List<ProductCharacteristic> output = new();

        foreach (ProductCharacteristic productCharacteristic in productCharacteristics)
        {
            ProductCharacteristic productCharacteristicClone = Clone(productCharacteristic);

            output.Add(productCharacteristicClone);
        }

        return output;
    }

    public static List<ProductStatuses> CloneAll(IEnumerable<ProductStatuses> productStatuses)
    {
        List<ProductStatuses> output = new();

        foreach (ProductStatuses productStatus in productStatuses)
        {
            ProductStatuses productStatusClone = Clone(productStatus);

            output.Add(productStatusClone);
        }

        return output;
    }

    public static List<Product> CloneAll(IEnumerable<Product> products)
    {
        List<Product> output = new();

        foreach (Product product in products)
        {
            Product productClone = Clone(product);

            output.Add(productClone);
        }

        return output;
    }
}