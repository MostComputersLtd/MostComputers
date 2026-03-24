using OneOf;
using OneOf.Types;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImage.FileRelated;
using MOSTComputers.Services.ProductRegister.Models.Requests.PromotionProductFileInfo;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.ProductProperty;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductProperty;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductDocuments;

namespace MOSTComputers.Services.ProductRegister.Models.Requests.ProductImageAndPromotionFileSave;

public sealed class ProductRelatedItemsFullSaveRequest
{
    public required int ProductId { get; set; }

    public List<ProductPropertyForProductUpsertRequest> PropertyRequests { get; set; } = new();
    public List<ProductImageAndPromotionFileUpsertRequest> ImageRequests { get; set; } = new();
    public List<ProductDocumentUpsertRequest> DocumentRequests { get; set; } = new();
    public required string UpsertUserName { get; set; }
}

public sealed class ProductImageAndPromotionFileUpsertRequest
{
    public ProductImageAndPromotionFileUpsertRequest(
        ProductImageWithFileForProductUpsertRequest productImageWithFileUpsertRequest)
    {
        _request = productImageWithFileUpsertRequest;
    }

    public ProductImageAndPromotionFileUpsertRequest(
        ProductImageFileForProductCreateRequest productImageFileCreateRequest)
    {
        _request = productImageFileCreateRequest;
    }

    public ProductImageAndPromotionFileUpsertRequest(
        ProductImageFileForProductUpdateRequest productImageFileUpdateRequest)
    {
        _request = productImageFileUpdateRequest;
    }

    public ProductImageAndPromotionFileUpsertRequest(
        PromotionProductFileForProductUpsertRequest promotionProductFileUpsertRequest)
    {
        _request = promotionProductFileUpsertRequest;
    }

    private readonly OneOf<
        ProductImageWithFileForProductUpsertRequest,
        ProductImageFileForProductCreateRequest,
        ProductImageFileForProductUpdateRequest,
        PromotionProductFileForProductUpsertRequest> _request;

    internal OneOf<
        ProductImageWithFileForProductUpsertRequest,
        ProductImageFileForProductCreateRequest,
        ProductImageFileForProductUpdateRequest,
        PromotionProductFileForProductUpsertRequest> Request => _request;
}

public sealed class ProductImageFileForProductCreateRequest
{
    public FileData? FileData { get; set; }
    public int? CustomDisplayOrder { get; set; }
    public bool? Active { get; set; }
}

public sealed class ProductImageFileForProductUpdateRequest
{
    public required int Id { get; set; }
    public bool? Active { get; set; }
    public required OneOf<int, No> UpdateDisplayOrderRequest { get; set; }
    public required OneOf<FileData?, No> UpdateFileDataRequest { get; set; }
}

public sealed class PromotionProductFileForProductUpsertRequest
{
    public int? Id { get; init; }
    public required int PromotionFileInfoId { get; init; }
    public DateTime? ValidFrom { get; init; }
    public DateTime? ValidTo { get; init; }
    public required bool Active { get; init; }
    public ServicePromotionProductImageUpsertRequest? UpsertInProductImagesRequest { get; init; }
}

public sealed class ProductDocumentUpsertRequest
{
    public required OneOf<ProductDocumentCreateForProductRequest, ProductDocumentUpdateForProductRequest> Request { get; set; }
}

public sealed class ProductDocumentCreateForProductRequest
{
    public required byte[] FileData { get; set; }
    public required string FileExtension { get; set; }
    public string? Description { get; set; }
}

public sealed class ProductDocumentUpdateForProductRequest
{
    public required int ExistingId { get; set; }
    public string? Description { get; set; }
}