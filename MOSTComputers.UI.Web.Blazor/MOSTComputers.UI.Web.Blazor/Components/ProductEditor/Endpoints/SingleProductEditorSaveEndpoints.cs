using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Models.Product.Models.ProductImages;
using MOSTComputers.Services.ProductImageFileManagement.Services.Contracts;
using MOSTComputers.Services.ProductRegister.Models.Requests;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImage.FileRelated;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImageAndPromotionFileSave;
using MOSTComputers.Services.ProductRegister.Models.Requests.PromotionProductFileInfo;
using MOSTComputers.Services.ProductRegister.Services.ProductImages.Contracts;
using MOSTComputers.UI.Web.Blazor.Endpoints;
using OneOf;
using OneOf.Types;
using static MOSTComputers.UI.Web.Blazor.Components.ProductEditor.SingleProductEditorPopup.SingleProductEditorPopup;
using static MOSTComputers.Utils.Files.ContentTypeUtils;

namespace MOSTComputers.UI.Web.Blazor.Components.ProductEditor.Endpoints;

internal static class SingleProductEditorSaveEndpoints
{
    private sealed class SaveAllRequest
    {
        [JsonPropertyName("productProperties")] public List<ProductPropertyDisplayData> ProductProperties { get; set; }
        [JsonPropertyName("productImages")] public List<ProductImageListItemDisplayData> ProductImages { get; set; }
        [JsonPropertyName("promotionProductFiles")] public List<PromotionProductFileInfoDisplayData> PromotionProductFiles { get; set; }
        [JsonPropertyName("productDocuments")] public List<ProductDocumentDisplayData> ProductDocuments { get; set; }
    }

    public static IEndpointRouteBuilder MapSingleProductEditorSaveEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/save", SaveAllAsync)
            .AddEndpointFilter<AntiforgeryEndpointFilter>();

        return endpoints;
    }

    private static async Task<IResult> SaveAllAsync(
        HttpRequest httpRequest,
        [FromServices] IProductImageService productImageService,
        [FromServices] IProductImageFileManagementService productImageFileManagementService)
    {
        IFormCollection form = await httpRequest.ReadFormAsync();

        string? metadata = form["metadata"].FirstOrDefault();

        if (metadata is null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: "Metadata is required");
        }

        SaveAllRequest? editorRequest = JsonSerializer.Deserialize<SaveAllRequest>(metadata);

        if (editorRequest is null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: "Invalid metadata");
        }

        Dictionary<string, IFormFile> localFileData = form.Files
            .ToDictionary(file => file.Name, StringComparer.Ordinal);
    } 

    private async Task<List<ProductImageAndPromotionFileUpsertRequest>> GetImageUpsertRequestsFromEditorDataAsync(
        List<ProductImageListItemDisplayData> productImages,
        List<PromotionProductFileInfoDisplayData> promotionProductFiles,
        Dictionary<string, IFormFile> files,
        IProductImageService productImageService,
        IProductImageFileManagementService productImageFileManagementService)
    {
        List<ProductImageAndPromotionFileUpsertRequest> imageRequests = new();

        for (int i = 0; i < productImages.Count; i++)
        {
            ProductImageListItemDisplayData imageListItem = productImages[i];
            PromotionProductFileInfoDisplayData? relatedPromotionFile = null;

            if (imageListItem.RelatedPromotionFileEditorId != null)
            {
                int relatedPromotionFileIndex = promotionProductFiles.FindIndex(
                    x => x.EditorId == imageListItem.RelatedPromotionFileEditorId);

                relatedPromotionFile = promotionProductFiles[relatedPromotionFileIndex];

                promotionProductFiles.RemoveAt(relatedPromotionFileIndex);
            }

            bool hasFile = files.TryGetValue(imageListItem.EditorId, out IFormFile? uploadedFile);

            byte[]? imageFileData = await GetImageDataFromListItemAsync(imageListItem, uploadedFile);

            IResult? problemDetailsResult = ValidateImageListItem(imageListItem, relatedPromotionFile, imageFileData);

            if (problemDetailsResult != null) return problemDetailsResult;

            ProductImageAndPromotionFileUpsertRequest request = await GetImageAndPromotionFileRequestAsync(
                imageListItem, relatedPromotionFile, imageFileData, i);

            imageRequests.Add(request);
        }

        return imageRequests;
    }

    private static async Task<ProductImageAndPromotionFileUpsertRequest> GetImageAndPromotionFileRequestAsync(
        ProductImageListItemDisplayData imageListItem,
        PromotionProductFileInfoDisplayData? relatedPromotionFile,
        byte[]? imageFileData,
        int i)
    {
        if (imageListItem.UploadedImageFileData is not null)
        {
            string? imageFileExtension = Path.GetExtension(imageListItem.UploadedImageFileData.FileName);

            ProductImageWithFileForProductUpsertRequest imageUpsertRequest = new()
            {
                ExistingImageId = imageListItem.RelatedImage?.ExistingImageId,
                FileExtension = imageFileExtension,
                ImageData = imageFileData!,
                FileUpsertRequest = new FileForImageForProductUpsertRequest()
                {
                    Active = imageListItem.RelatedImageFile?.Active ?? true,
                    CustomDisplayOrder = i + 1,
                },
            };

            return new(imageUpsertRequest);
        }
        else if (relatedPromotionFile is not null)
        {
            ServicePromotionProductImageUpsertRequest? servicePromotionProductImageUpsertRequest = null;

            if (imageListItem.RelatedImage is not null)
            {
                servicePromotionProductImageUpsertRequest = new()
                {
                    ImageFileUpsertRequest = new()
                    {
                        Active = imageListItem.RelatedImageFile?.Active ?? false,
                        CustomDisplayOrder = i + 1,
                    },
                };
            }

            PromotionProductFileForProductUpsertRequest servicePromotionProductFileUpdateRequest = new()
            {
                Id = relatedPromotionFile.Id,
                PromotionFileInfoId = relatedPromotionFile.FileInfoId!.Value,
                Active = relatedPromotionFile.Active,
                ValidFrom = relatedPromotionFile.ValidFrom,
                ValidTo = relatedPromotionFile.ValidTo,
                UpsertInProductImagesRequest = servicePromotionProductImageUpsertRequest,
            };

            return new(servicePromotionProductFileUpdateRequest);
        }
        else if (imageListItem.RelatedImage is not null)
        {
            string? imageFileExtension;

            if (imageListItem.RelatedImageFile?.FileName is not null)
            {
                imageFileExtension = Path.GetExtension(imageListItem.RelatedImageFile.FileName);
            }
            else
            {
                List<string> possibleFileExtensions = GetPossibleExtensionsFromContentType(imageListItem.RelatedImage.ContentType!);

                imageFileExtension = possibleFileExtensions.First();
            }

            FileForImageForProductUpsertRequest? imageFileUpsertRequest = null;

            if (imageListItem.RelatedImageFile is not null)
            {
                imageFileUpsertRequest = new FileForImageForProductUpsertRequest()
                {
                    Active = imageListItem.RelatedImageFile?.Active ?? true,
                    CustomDisplayOrder = i + 1,
                };
            }

            ProductImageWithFileForProductUpsertRequest imageUpsertRequest = new()
            {
                ExistingImageId = imageListItem.RelatedImage.ExistingImageId,
                FileExtension = imageFileExtension,
                ImageData = imageFileData!,
                FileUpsertRequest = imageFileUpsertRequest,
            };

            return new(imageUpsertRequest);
        }
        else if (imageListItem.RelatedImageFile?.FileName is not null)
        {
            if (imageListItem.RelatedImageFile.ExistingFileInfoId is null)
            {
                ProductImageFileForProductCreateRequest imageFileCreateRequest = new()
                {
                    Active = imageListItem.RelatedImageFile.Active,
                    CustomDisplayOrder = i + 1,
                    FileData = new()
                    {
                        FileName = imageListItem.RelatedImageFile.FileName,
                        Data = imageFileData!,
                    },
                };

                return new(imageFileCreateRequest);
            }

            ProductImageFileForProductUpdateRequest imageFileUpdateRequest = new()
            {
                Id = imageListItem.RelatedImageFile.ExistingFileInfoId.Value,
                Active = imageListItem.RelatedImageFile.Active,
                UpdateDisplayOrderRequest = i + 1,
                UpdateFileDataRequest = new FileData()
                {
                    FileName = imageListItem.RelatedImageFile.FileName,
                    Data = imageFileData!,
                },
            };

            return new(imageFileUpdateRequest);
        }

        return default!;
    }

    private static IResult? ValidateImageListItem(
        ProductImageListItemDisplayData imageListItem,
        PromotionProductFileInfoDisplayData? relatedPromotionFile,
        byte[]? imageFileData)
    {
        if (imageListItem.UploadedImageFileData is not null)
        {
            if (imageFileData is null || imageFileData.Length == 0)
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    detail: "File is empty");
            }

            if (!IsImageContentType(imageListItem.UploadedImageFileData.ContentType))
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    detail: "File is not in a supported image format");
            }

            string? imageFileExtension = Path.GetExtension(imageListItem.UploadedImageFileData.FileName);

            if (imageFileExtension is null)
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    detail: "Invalid file extension");
            }
        }
        else if (relatedPromotionFile is not null)
        {
            if (relatedPromotionFile?.FileInfoId is null)
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    detail: "Promotion file not selected");
            }
        }
        else if (imageListItem.RelatedImage is not null)
        {
            if (imageFileData is null || imageFileData.Length == 0)
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    detail: "File is empty");
            }

            if (!IsImageContentType(imageListItem.RelatedImage.ContentType))
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    detail: "File is not in a supported image format");
            }
        }
        else if (imageListItem.RelatedImageFile?.FileName is not null)
        {
            if (imageFileData is null || imageFileData.Length == 0)
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    detail: "File is empty");
            }

            string? contentType = GetContentTypeFromExtension(imageListItem.RelatedImageFile.FileName);

            if (!IsImageContentType(contentType))
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    detail: "File is not in a supported image format");
            }
        }
        else
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest);
        }

        return null;
    }

    private static async Task<byte[]?> GetImageDataFromListItemAsync(
        ProductImageListItemDisplayData imageListItem,
        IFormFile? uploadedFile,
        IProductImageService productImageService,
        IProductImageFileManagementService productImageFileManagementService)
    {
        if (imageListItem.UploadedImageFileData is not null)
        {
            if (uploadedFile == null) return null;

            using MemoryStream memoryStream = new();

            await uploadedFile.CopyToAsync(memoryStream);

            return memoryStream.ToArray();
        }

        int? existingImageId = imageListItem.RelatedImage?.ExistingImageId;
        string? existingImageFileName = imageListItem.RelatedImageFile?.FileName;

        if (existingImageId is not null)
        {
            ProductImage? productImage = await productImageService.GetByIdInAllImagesAsync(existingImageId.Value);

            return productImage?.ImageData;
        }
        else if (existingImageFileName is not null)
        {
            OneOf<byte[], FileDoesntExistResult> getImageFileDataResult
                = await productImageFileManagementService.GetImageAsync(existingImageFileName);

            return getImageFileDataResult.Match<byte[]?>(
                fileData => fileData,
                fileDoesntExistResult => null);
        }

        return null;
    }

    private static string GetImageRequestValidationErrorPath(ProductImageAndPromotionFileUpsertRequest upsertRequest, int requestIndex)
    {
        string requestType = GetImageRequestTypeInResultErrorPath(upsertRequest);

        string prefix = $"ImageRequests[{requestIndex}].Request.{requestType}.";

        return prefix;
    }

    private static string GetImageRequestTypeInResultErrorPath(ProductImageAndPromotionFileUpsertRequest upsertRequest)
    {
        return upsertRequest.Request.Match(
            x => "AsT0",
            x => "AsT1",
            x => "AsT2",
            x => "AsT3");
    }
}
