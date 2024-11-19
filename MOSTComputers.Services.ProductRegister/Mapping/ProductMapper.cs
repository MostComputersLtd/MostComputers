using MOSTComputers.Services.DAL.Models.Requests.Category;
using MOSTComputers.Services.DAL.Models.Requests.ExternalXmlImport.ProductImage;
using MOSTComputers.Services.DAL.Models.Requests.ExternalXmlImport.ProductImageFileNameInfo;
using MOSTComputers.Services.DAL.Models.Requests.Product;
using MOSTComputers.Services.DAL.Models.Requests.ProductImage;
using MOSTComputers.Services.DAL.Models.Requests.ProductImageFileNameInfo;
using MOSTComputers.Services.DAL.Models.Requests.Promotion;
using MOSTComputers.Services.DAL.Models.Requests.ToDoLocalChanges;
using MOSTComputers.Services.ProductRegister.Models.ExternalXmlImport.ProductImage;
using MOSTComputers.Services.ProductRegister.Models.ExternalXmlImport.ProductImageFileNameInfo;
using MOSTComputers.Services.ProductRegister.Models.Requests.Category;
using MOSTComputers.Services.ProductRegister.Models.Requests.Product;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImage;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImageFileNameInfo;
using MOSTComputers.Services.ProductRegister.Models.Requests.Promotion;
using MOSTComputers.Services.ProductRegister.Models.Requests.ToDoLocalChanges;
using Riok.Mapperly.Abstractions;

namespace MOSTComputers.Services.ProductRegister.Mapping;

[Mapper]
public sealed partial class ProductMapper
{
    [MapperIgnoreTarget(nameof(CategoryCreateRequest.IsLeaf))]
    [MapperIgnoreTarget(nameof(CategoryCreateRequest.RowGuid))]
    internal partial CategoryCreateRequest Map(ServiceCategoryCreateRequest serviceCategoryCreateRequest);


    [MapperIgnoreTarget(nameof(CategoryUpdateRequest.RowGuid))]
    internal partial CategoryUpdateRequest Map(ServiceCategoryUpdateRequest serviceCategoryUpdateRequest);


    [MapperIgnoreTarget(nameof(ProductImageCreateRequest.DateModified))]
    internal partial ProductImageCreateRequest Map(ServiceProductImageCreateRequest serviceCategoryUpdateRequest);


    [MapperIgnoreTarget(nameof(ProductFirstImageCreateRequest.DateModified))]
    internal partial ProductFirstImageCreateRequest Map(ServiceProductFirstImageCreateRequest serviceCategoryUpdateRequest);


    [MapperIgnoreTarget(nameof(ProductImageUpdateRequest.DateModified))]
    internal partial ProductImageUpdateRequest Map(ServiceProductImageUpdateRequest serviceCategoryUpdateRequest);


    [MapperIgnoreTarget(nameof(ProductFirstImageUpdateRequest.DateModified))]
    internal partial ProductFirstImageUpdateRequest Map(ServiceProductFirstImageUpdateRequest serviceCategoryUpdateRequest);


    [MapperIgnoreTarget(nameof(PromotionCreateRequest.PromotionAddedDate))]
    internal partial PromotionCreateRequest Map(ServicePromotionCreateRequest serviceCategoryUpdateRequest);


    [MapperIgnoreTarget(nameof(PromotionCreateRequest.PromotionAddedDate))]
    internal partial PromotionUpdateRequest Map(ServicePromotionUpdateRequest serviceCategoryUpdateRequest);

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable RMG020 // Source member is not mapped to any target member

    [MapperIgnoreTarget(nameof(ProductImageFileNameInfoCreateRequest.Active))]
    internal partial ProductImageFileNameInfoCreateRequest Map(ServiceProductImageFileNameInfoCreateRequest serviceCategoryUpdateRequest);


    [MapperIgnoreTarget(nameof(ProductImageFileNameInfoByImageNumberUpdateRequest.Active))]
    internal partial ProductImageFileNameInfoByImageNumberUpdateRequest Map(ServiceProductImageFileNameInfoByImageNumberUpdateRequest serviceCategoryUpdateRequest);


    [MapperIgnoreTarget(nameof(ProductImageFileNameInfoByFileNameUpdateRequest.Active))]
    internal partial ProductImageFileNameInfoByFileNameUpdateRequest Map(ServiceProductImageFileNameInfoByFileNameUpdateRequest serviceCategoryUpdateRequest);

    [MapperIgnoreTarget(nameof(ToDoLocalChangeCreateRequest.TimeStamp))]
    internal partial ToDoLocalChangeCreateRequest Map(ServiceToDoLocalChangeCreateRequest serviceToDoLocalChangeCreateRequest);


#pragma warning restore RMG020 // Source member is not mapped to any target member
#pragma warning restore IDE0079 // Remove unnecessary suppression

#pragma warning disable CA1822 // Mark members as static
    public ProductCreateRequest MapWithoutIncludingImages(ProductCreateWithoutImagesInDatabaseRequest productCreateWithoutImagesInDBRequest)
    {
        return new()
        {
            Name = productCreateWithoutImagesInDBRequest.Name,
            AdditionalWarrantyPrice = productCreateWithoutImagesInDBRequest.AdditionalWarrantyPrice,
            AdditionalWarrantyTermMonths = productCreateWithoutImagesInDBRequest.AdditionalWarrantyTermMonths,
            StandardWarrantyPrice = productCreateWithoutImagesInDBRequest.StandardWarrantyPrice,
            StandardWarrantyTermMonths = productCreateWithoutImagesInDBRequest.StandardWarrantyTermMonths,
            DisplayOrder = productCreateWithoutImagesInDBRequest.DisplayOrder,
            Status = productCreateWithoutImagesInDBRequest.Status,
            PlShow = productCreateWithoutImagesInDBRequest.PlShow,
            Price1 = productCreateWithoutImagesInDBRequest.Price1,
            DisplayPrice = productCreateWithoutImagesInDBRequest.DisplayPrice,
            Price3 = productCreateWithoutImagesInDBRequest.Price3,
            Currency = productCreateWithoutImagesInDBRequest.Currency,
            RowGuid = productCreateWithoutImagesInDBRequest.RowGuid,
            PromotionId = productCreateWithoutImagesInDBRequest.PromotionId,
            PromRid = productCreateWithoutImagesInDBRequest.PromRid,
            PromotionPictureId = productCreateWithoutImagesInDBRequest.PromotionPictureId,
            PromotionExpireDate = productCreateWithoutImagesInDBRequest.PromotionExpireDate,
            AlertPictureId = productCreateWithoutImagesInDBRequest.AlertPictureId,
            AlertExpireDate = productCreateWithoutImagesInDBRequest.AlertExpireDate,
            PriceListDescription = productCreateWithoutImagesInDBRequest.PriceListDescription,
            PartNumber1 = productCreateWithoutImagesInDBRequest.PartNumber1,
            PartNumber2 = productCreateWithoutImagesInDBRequest.PartNumber2,
            SearchString = productCreateWithoutImagesInDBRequest.SearchString,
            CategoryId = productCreateWithoutImagesInDBRequest.CategoryId,
            ManifacturerId = productCreateWithoutImagesInDBRequest.ManifacturerId,
            SubCategoryId = productCreateWithoutImagesInDBRequest.SubCategoryId,
            Properties = productCreateWithoutImagesInDBRequest.Properties?
                .Select(property => new CurrentProductPropertyCreateRequest()
                {
                    ProductCharacteristicId = property.ProductCharacteristicId,
                    Value = property.Value,
                    CustomDisplayOrder = property.CustomDisplayOrder,
                    XmlPlacement = property.XmlPlacement,
                }).ToList(),

            Images = new(),
            ImageFileNames = new(),
        };
    }

    public ProductUpdateRequest MapWithoutIncludingImagesAndProperties(ProductUpdateWithoutImagesInDatabaseRequest productUpdateRequestWithoutImagesInDB)
    {
        return new()
        {
            Id = productUpdateRequestWithoutImagesInDB.Id,
            Name = productUpdateRequestWithoutImagesInDB.Name,
            AdditionalWarrantyPrice = productUpdateRequestWithoutImagesInDB.AdditionalWarrantyPrice,
            AdditionalWarrantyTermMonths = productUpdateRequestWithoutImagesInDB.AdditionalWarrantyTermMonths,
            StandardWarrantyPrice = productUpdateRequestWithoutImagesInDB.StandardWarrantyPrice,
            StandardWarrantyTermMonths = productUpdateRequestWithoutImagesInDB.StandardWarrantyTermMonths,
            DisplayOrder = productUpdateRequestWithoutImagesInDB.DisplayOrder,
            Status = productUpdateRequestWithoutImagesInDB.Status,
            PlShow = productUpdateRequestWithoutImagesInDB.PlShow,
            Price1 = productUpdateRequestWithoutImagesInDB.Price1,
            DisplayPrice = productUpdateRequestWithoutImagesInDB.DisplayPrice,
            Price3 = productUpdateRequestWithoutImagesInDB.Price3,
            Currency = productUpdateRequestWithoutImagesInDB.Currency,
            RowGuid = productUpdateRequestWithoutImagesInDB.RowGuid,
            PromotionId = productUpdateRequestWithoutImagesInDB.PromotionId,
            PromRid = productUpdateRequestWithoutImagesInDB.PromRid,
            PromotionPictureId = productUpdateRequestWithoutImagesInDB.PromotionPictureId,
            PromotionExpireDate = productUpdateRequestWithoutImagesInDB.PromotionExpireDate,
            AlertPictureId = productUpdateRequestWithoutImagesInDB.AlertPictureId,
            AlertExpireDate = productUpdateRequestWithoutImagesInDB.AlertExpireDate,
            PriceListDescription = productUpdateRequestWithoutImagesInDB.PriceListDescription,
            PartNumber1 = productUpdateRequestWithoutImagesInDB.PartNumber1,
            PartNumber2 = productUpdateRequestWithoutImagesInDB.PartNumber2,
            SearchString = productUpdateRequestWithoutImagesInDB.SearchString,

            Properties = new(),
            Images = new(),
            ImageFileNames = new(),

            CategoryId = productUpdateRequestWithoutImagesInDB.CategoryId,
            ManifacturerId = productUpdateRequestWithoutImagesInDB.ManifacturerId,
            SubCategoryId = productUpdateRequestWithoutImagesInDB.SubCategoryId,
        };
    }

    public ProductUpdateRequest MapWithoutIncludingImagesAndProperties(ProductFullUpdateRequest productFullUpdateRequest)
    {
        return new()
        {
            Id = productFullUpdateRequest.Id,
            Name = productFullUpdateRequest.Name,
            AdditionalWarrantyPrice = productFullUpdateRequest.AdditionalWarrantyPrice,
            AdditionalWarrantyTermMonths = productFullUpdateRequest.AdditionalWarrantyTermMonths,
            StandardWarrantyPrice = productFullUpdateRequest.StandardWarrantyPrice,
            StandardWarrantyTermMonths = productFullUpdateRequest.StandardWarrantyTermMonths,
            DisplayOrder = productFullUpdateRequest.DisplayOrder,
            Status = productFullUpdateRequest.Status,
            PlShow = productFullUpdateRequest.PlShow,
            Price1 = productFullUpdateRequest.Price1,
            DisplayPrice = productFullUpdateRequest.DisplayPrice,
            Price3 = productFullUpdateRequest.Price3,
            Currency = productFullUpdateRequest.Currency,
            RowGuid = productFullUpdateRequest.RowGuid,
            PromotionId = productFullUpdateRequest.PromotionId,
            PromRid = productFullUpdateRequest.PromRid,
            PromotionPictureId = productFullUpdateRequest.PromotionPictureId,
            PromotionExpireDate = productFullUpdateRequest.PromotionExpireDate,
            AlertPictureId = productFullUpdateRequest.AlertPictureId,
            AlertExpireDate = productFullUpdateRequest.AlertExpireDate,
            PriceListDescription = productFullUpdateRequest.PriceListDescription,
            PartNumber1 = productFullUpdateRequest.PartNumber1,
            PartNumber2 = productFullUpdateRequest.PartNumber2,
            SearchString = productFullUpdateRequest.SearchString,

            Properties = new(),
            Images = new(),
            ImageFileNames = new(),

            CategoryId = productFullUpdateRequest.CategoryId,
            ManifacturerId = productFullUpdateRequest.ManifacturerId,
            SubCategoryId = productFullUpdateRequest.SubCategoryId,
        };
#pragma warning restore CA1822 // Mark members as static
    }

    [MapperIgnoreTarget(nameof(XmlImportProductImageFileNameInfoCreateRequest.Active))]
    internal partial XmlImportProductImageFileNameInfoCreateRequest Map(XmlImportServiceProductImageFileNameInfoCreateRequest serviceCategoryUpdateRequest);

    [MapperIgnoreTarget(nameof(XmlImportProductImageFileNameInfoByImageNumberUpdateRequest.Active))]
    internal partial XmlImportProductImageFileNameInfoByImageNumberUpdateRequest Map(XmlImportServiceProductImageFileNameInfoByImageNumberUpdateRequest serviceCategoryUpdateRequest);

    [MapperIgnoreTarget(nameof(XmlImportProductImageFileNameInfoByFileNameUpdateRequest.Active))]
    internal partial XmlImportProductImageFileNameInfoByFileNameUpdateRequest Map(XmlImportServiceProductImageFileNameInfoByFileNameUpdateRequest serviceCategoryUpdateRequest);


    [MapperIgnoreTarget(nameof(XmlImportProductImageUpsertRequest.DateModified))]
    internal partial XmlImportProductImageUpsertRequest Map(XmlImportServiceProductImageCreateRequest serviceCategoryUpdateRequest);


    [MapperIgnoreTarget(nameof(XmlImportProductFirstImageCreateRequest.DateModified))]
    internal partial XmlImportProductFirstImageCreateRequest Map(XmlImportServiceProductFirstImageCreateRequest serviceCategoryUpdateRequest);


    [MapperIgnoreTarget(nameof(XmlImportProductImageUpdateRequest.DateModified))]
    internal partial XmlImportProductImageUpdateRequest Map(XmlImportServiceProductImageUpdateRequest serviceCategoryUpdateRequest);


    [MapperIgnoreTarget(nameof(XmlImportProductFirstImageUpdateRequest.DateModified))]
    internal partial XmlImportProductFirstImageUpdateRequest Map(XmlImportServiceProductFirstImageUpdateRequest serviceCategoryUpdateRequest);
}