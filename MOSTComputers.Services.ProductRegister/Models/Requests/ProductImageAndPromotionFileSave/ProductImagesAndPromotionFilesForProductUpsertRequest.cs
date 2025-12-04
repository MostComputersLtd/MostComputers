using OneOf;
using OneOf.Types;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImage.FileRelated;
using MOSTComputers.Services.ProductRegister.Models.Requests.PromotionProductFileInfo;

namespace MOSTComputers.Services.ProductRegister.Models.Requests.ProductImageAndPromotionFileSave;

public sealed class ProductImagesAndPromotionFilesForProductUpsertRequest
{
    public required int ProductId { get; set; }

    public List<ProductImageAndPromotionFileUpsertRequest> Requests = new();
    public required string UpsertUserName { get; set; }

    public void Add(ProductImageAndPromotionFileUpsertRequest productImageAndPromotionFileUpsertRequest)
    {
        Requests.Add(productImageAndPromotionFileUpsertRequest);
    }

    public void Add(ProductImageWithFileForProductUpsertRequest productImageWithFileUpsertRequest)
    {
        Requests.Add(new(productImageWithFileUpsertRequest));
    }

    public void Add(ProductImageFileForProductCreateRequest productImageFileCreateRequest)
    {
        Requests.Add(new(productImageFileCreateRequest));
    }

    public void Add(ProductImageFileForProductUpdateRequest productImageFileUpdateRequest)
    {
        Requests.Add(new(productImageFileUpdateRequest));
    }

    public void Add(PromotionProductFileForProductUpsertRequest promotionProductFileUpsertRequest)
    {
        Requests.Add(new(promotionProductFileUpsertRequest));
    }

    public bool Remove(ProductImageAndPromotionFileUpsertRequest productImageAndPromotionFileUpsertRequest)
    {
        return Requests.Remove(productImageAndPromotionFileUpsertRequest);
    }

    public bool Remove(ProductImageWithFileForProductUpsertRequest productImageWithFileUpsertRequest)
    {
        int requestIndex = Requests.FindIndex(x => x.Request.Value == productImageWithFileUpsertRequest);

        if (requestIndex < 0) return false;

        Requests.RemoveAt(requestIndex);

        return true;
    }

    public bool Remove(ProductImageFileForProductCreateRequest productImageFileCreateRequest)
    {
        int requestIndex = Requests.FindIndex(x => x.Request.Value == productImageFileCreateRequest);

        if (requestIndex < 0) return false;

        Requests.RemoveAt(requestIndex);

        return true;
    }

    public bool Remove(ProductImageFileForProductUpdateRequest productImageFileUpdateRequest)
    {
        int requestIndex = Requests.FindIndex(x => x.Request.Value == productImageFileUpdateRequest);

        if (requestIndex < 0) return false;

        Requests.RemoveAt(requestIndex);

        return true;
    }

    public bool Remove(PromotionProductFileForProductUpsertRequest promotionProductFileUpsertRequest)
    {
        int requestIndex = Requests.FindIndex(x => x.Request.Value == promotionProductFileUpsertRequest);

        if (requestIndex < 0) return false;

        Requests.RemoveAt(requestIndex);

        return true;
    }
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