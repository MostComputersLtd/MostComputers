using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using MOSTComputers.Services.ProductRegister.Models.Requests;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImage.FileRelated;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImageAndPromotionFileSave;
using MOSTComputers.Services.ProductRegister.Models.Requests.PromotionProductFileInfo;
using MOSTComputers.UI.Web.Blazor.Endpoints;
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
        [FromBody] SaveAllRequest request)
    {
    }

    private static IResult? ValidateImageListItem(
        ProductImageListItemDisplayData imageListItem,
        PromotionProductFileInfoDisplayData relatedPromotionFile,
        byte[]? imageFileData)
    {
        if (imageFileData is null || imageFileData.Length == 0)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: "File is empty");
        }

        if (imageListItem.UploadedImageFileData is not null)
        {
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
            if (!IsImageContentType(imageListItem.RelatedImage.ContentType))
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    detail: "File is not in a supported image format");
            }
        }
        else if (imageListItem.RelatedImageFile?.FileName is not null)
        {
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

    private async Task<List<ProductImageAndPromotionFileUpsertRequest>> GetImageUpsertRequestsFromEditorDataAsync(
        List<ProductImageListItemDisplayData> listItems)
    {
        List<ProductImageAndPromotionFileUpsertRequest> imageRequests = new();

        for (int i = 0; i < listItems.Count; i++)
        {
            ProductImageListItemDisplayData imageListItem = listItems[i];

            bool flowControl = await NewMethod(imageListItem, imageRequests, i);
            if (!flowControl)
            {
                continue;
            }
        }

        return imageRequests;
    }

    private static async Task<ProductImageAndPromotionFileUpsertRequest> NewMethod(
        ProductImageListItemDisplayData imageListItem,
        PromotionProductFileInfoDisplayData relatedPromotionFile,
        int i)
    {
        byte[]? imageFileData = await GetImageDataFromListItemAsync(imageListItem);

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
                List<string> possibleFileExtensions = GetPossibleExtensionsFromContentType(imageListItem.RelatedImage.ContentType);

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

                imageRequests.Add(new(imageFileCreateRequest));

                return false;
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

    private async Task<byte[]?> GetImageDataFromListItemAsync(ProductImageListItemDisplayData imageListItem)
    {
        if (imageListItem.UploadedImageFileData is not null)
        {
            return imageListItem.UploadedImageFileData.Data;
        }

        int? existingImageId = imageListItem.RelatedImage?.ExistingImageId;
        string? existingImageFileName = imageListItem.RelatedImageFile?.FileName;
        int? existingPromotionFileId = imageListItem.RelatedPromotionFile?.PromotionFileInfoData?.Id;

        if (existingImageId is not null)
        {
            ProductImage? productImage = await ProductImageService.GetByIdInAllImagesAsync(existingImageId.Value);

            return productImage?.ImageData;
        }
        else if (existingImageFileName is not null)
        {
            OneOf<byte[], FileDoesntExistResult> getImageFileDataResult
                = await ProductImageFileManagementService.GetImageAsync(existingImageFileName);

            return getImageFileDataResult.Match<byte[]?>(
                fileData => fileData,
                fileDoesntExistResult => null);
        }
        else if (existingPromotionFileId is not null)
        {
            Stream? promotionFileDataStream = await PromotionFileService.GetFileDataByIdAsync(existingPromotionFileId.Value);

            byte[]? fileData = null;

            if (promotionFileDataStream is not null)
            {
                using var memoryStream = new MemoryStream();

                await promotionFileDataStream.CopyToAsync(memoryStream);

                fileData = memoryStream.ToArray();
            }

            return fileData;
        }

        return null;
    }
}
