using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Models.Product.Models.ProductImages;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.ProductImageFileManagement.Services.Contracts;
using MOSTComputers.Services.ProductRegister.Models.Requests;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImage.FileRelated;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImageAndPromotionFileSave;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductProperty;
using MOSTComputers.Services.ProductRegister.Models.Requests.PromotionProductFileInfo;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Services.ProductRegister.Services.NewSaveService;
using MOSTComputers.Services.ProductRegister.Services.ProductImages.Contracts;
using MOSTComputers.UI.Web.Blazor.Endpoints;
using OneOf;
using OneOf.Types;
using static MOSTComputers.UI.Web.Blazor.Components.ProductEditor.SingleProductEditorPopup.SingleProductEditorPopup;
using static MOSTComputers.UI.Web.Blazor.Endpoints.EndpointErrorHandlingCommonElements;
using static MOSTComputers.Utils.Files.ContentTypeUtils;

namespace MOSTComputers.UI.Web.Blazor.Components.ProductEditor.Endpoints;

internal static class SingleProductEditorSaveEndpoints
{
    

    



    private sealed class SaveAllRequest
    {
        public int ProductId { get; set; }
        public List<ProductPropertySaveRequest> ProductProperties { get; set; } = [];
        public List<ProductImageSaveRequest> ProductImages { get; set; } = [];
        public List<PromotionProductFileSaveRequest> PromotionProductFiles { get; set; } = [];
        public List<ProductDocumentSaveRequest> ProductDocuments { get; set; } = [];
    }

    private sealed class ProductPropertySaveRequest
    {
        public int? ProductCharacteristicId { get; set; }
        public string? Value { get; set; }
    }

    private sealed class ProductImageSaveRequest
    {
        public required string EditorId { get; set; }
        public IFormFile? UploadedImageFile { get; set; }
        public int? ExistingImageId { get; set; }
        public ProductImageFileSaveRequest? RelatedImageFile { get; set; }
        public string? RelatedPromotionFileEditorId { get; set; }
    }

    private sealed class ProductImageFileSaveRequest
    {
        public int? ExistingFileInfoId { get; set; }
        public bool Active { get; set; }
    }

    private sealed class PromotionProductFileSaveRequest
    {
        public required string EditorId { get; set; }
        public int? Id { get; set; }
        public int? FileInfoId { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public bool Active { get; set; }
    }

    private sealed class ProductDocumentSaveRequest
    {
        public required string EditorId { get; set; }
        public int? ExistingDocumentId { get; set; }
        public string? CurrentDescription { get; set; }
        public string? FileExtension { get; set; }
        public IFormFile? File { get; set; }
    }






    private sealed class SaveAllPreRequestData
    {
        public required List<ProductPropertySaveRequest> ProductProperties { get; set; }
        public required List<ProductImagePreRequestData> ProductImages { get; set; }
        public required List<PromotionProductFilePreRequestData> PromotionProductFiles { get; set; }
        public required List<ProductDocumentPreRequestData> ProductDocuments { get; set; }
    }

    private sealed class ProductImagePreRequestData
    {
        internal required ProductImageSaveRequest ImageListItem { get; init; }
        internal ProductImageData? RelatedImage { get; init; }
        internal ProductImageFileData? RelatedImageFile { get; init; }
        internal PromotionProductFileSaveRequest? RelatedPromotionFile { get; init; }
        internal byte[]? ImageFileData { get; init; }
    }

    private sealed class ProductDocumentPreRequestData
    {
        internal required ProductDocumentSaveRequest DocumentData { get; init; }
        internal byte[]? DocumentFileData { get; init; }
    }

    private sealed class PromotionProductFilePreRequestData
    {
        internal required PromotionProductFileSaveRequest RelatedPromotionFile { get; init; }
    }

    public static IEndpointRouteBuilder MapSingleProductEditorSaveEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/save", SaveAllAsync)
            .AddEndpointFilter<AntiforgeryEndpointFilter>();

        return endpoints;
    }

    private static async Task<IResult> SaveAllAsync(
        HttpContext httpContext,
        [FromForm] SaveAllRequest request,
        [FromServices] IProductImageService productImageService,
        [FromServices] IProductImageFileService productImageFileService,
        [FromServices] IProductImageFileManagementService productImageFileManagementService,
        [FromServices] IProductDataSaveService productDataSaveService)
    {
        string currentUserName = httpContext.User.Identity?.Name!;

        SaveAllPreRequestData preRequestData = await GetPreRequestDataAsync(
            request.ProductId,
            request.ProductProperties,
            request.ProductImages,
            request.PromotionProductFiles,
            request.ProductDocuments,
            productImageService,
            productImageFileService,
            productImageFileManagementService);

        SaveAllPreRequestDataValidator validator = new();

        ValidationResult validationResult = validator.Validate(preRequestData);

        if (!validationResult.IsValid)
        {
            return ValidationResultProblem(validationResult);
        }

        List<ProductPropertyForProductUpsertRequest> propertyRequests = GetPropertyRequests(preRequestData.ProductProperties);
        List<ProductImageAndPromotionFileUpsertRequest> imageRequests = GetImageRequestsAsync(preRequestData.ProductImages);
        List<ProductDocumentUpsertRequest> documentRequests = GetDocumentRequests(preRequestData.ProductDocuments);

        ProductRelatedItemsFullSaveRequest saveAllRequest = new()
        {
            ProductId = request.ProductId,
            PropertyRequests = propertyRequests,
            ImageRequests = imageRequests,
            DocumentRequests = documentRequests,
            UpsertUserName = currentUserName,
        };

        OneOf<Success, ValidationResult, UnexpectedFailureResult> result
            = await productDataSaveService.SaveAsync(saveAllRequest);

        return result.Match(
            success => ,
            validationResult => ValidationResultProblem(validationResult),
            unexpectedFailureResult => Results.Problem(statusCode: StatusCodes.Status500InternalServerError, detail: GenericErrorDetail)
        );
    }

    private sealed class SaveAllPreRequestDataValidator : AbstractValidator<SaveAllPreRequestData>
    {
        internal SaveAllPreRequestDataValidator()
        {
            RuleForEach(x => x.ProductProperties)
                .SetValidator(new ProductPropertyDisplayDataValidator());

            RuleForEach(x => x.ProductImages)
                .SetValidator(new ProductImagePreRequestDataValidator());

            RuleForEach(x => x.PromotionProductFiles)
                .SetValidator(new PromotionProductFilePreRequestDataValidator());

            RuleForEach(x => x.ProductDocuments)
                .SetValidator(new ProductDocumentPreRequestDataValidator());
        }
    }

    private sealed class ProductPropertyDisplayDataValidator : AbstractValidator<ProductPropertySaveRequest>
    {
        internal ProductPropertyDisplayDataValidator()
        {
            RuleFor(x => x.ProductCharacteristicId).NotNull();
        }
    }

    private sealed class ProductImagePreRequestDataValidator : AbstractValidator<ProductImagePreRequestData>
    {
        public ProductImagePreRequestDataValidator()
        {
            RuleFor(x => x).Custom((x, context) =>
            {
                ProductImageSaveRequest imageListItem = x.ImageListItem;
                ProductImageFileData? relatedImageFile = x.RelatedImageFile;
                PromotionProductFileSaveRequest? relatedPromotionFile = x.RelatedPromotionFile;

                if (imageListItem.UploadedImageFile != null)
                {
                    IFormFile uploadedFile = imageListItem.UploadedImageFile;

                    if (uploadedFile == null || uploadedFile.Length == 0)
                    {
                        context.AddFailure("File is empty");
                        return;
                    }

                    if (!IsImageContentType(uploadedFile.ContentType))
                    {
                        context.AddFailure("File is not in a supported image format");
                        return;
                    }

                    string? imageFileExtension = Path.GetExtension(uploadedFile.FileName);

                    if (imageFileExtension is null)
                    {
                        context.AddFailure("Invalid file extension");
                        return;
                    }
                }
                else if (relatedPromotionFile is not null)
                {
                    if (relatedPromotionFile?.FileInfoId is null)
                    {
                        context.AddFailure("Promotion file not selected");
                        return;
                    }
                }
                else if (imageListItem.ExistingImageId is not null)
                {
                }
                else if (relatedImageFile?.FileName is not null)
                {
                    string? contentType = GetContentTypeFromExtension(relatedImageFile.FileName);

                    if (!IsImageContentType(contentType))
                    {
                        context.AddFailure("File is not in a supported image format");
                        return;
                    }
                }
                else
                {
                    context.AddFailure("Invalid request composition");
                }
            });
        }
    }

    private sealed class PromotionProductFilePreRequestDataValidator : AbstractValidator<PromotionProductFilePreRequestData>
    {
        internal PromotionProductFilePreRequestDataValidator()
        {
            RuleFor(x => x.RelatedPromotionFile.FileInfoId).NotNull();
        }
    }

    private sealed class ProductDocumentPreRequestDataValidator : AbstractValidator<ProductDocumentPreRequestData>
    {
        internal ProductDocumentPreRequestDataValidator()
        {
            RuleFor(x => x.DocumentFileData).NotNull().NotEmpty()
                .When(x => x.DocumentData.ExistingDocumentId == null);
        }
    }

    private static async Task<SaveAllPreRequestData> GetPreRequestDataAsync(
        int productId,
        List<ProductPropertySaveRequest> productProperties,
        List<ProductImageSaveRequest> productImages,
        List<PromotionProductFileSaveRequest> promotionProductFiles,
        List<ProductDocumentSaveRequest> productDocuments,
        IProductImageService productImageService,
        IProductImageFileService productImageFileService,
        IProductImageFileManagementService productImageFileManagementService)
    {
        List<PromotionProductFileSaveRequest> promotionProductFilesCopy = new(promotionProductFiles);

        List<ProductImagePreRequestData> imageRequests = new();

        List<ProductImageData> existingImages = await productImageService.GetAllInProductWithoutFileDataAsync(productId);
        List<ProductImageFileData> existingImageFiles = await productImageFileService.GetAllInProductAsync(productId);

        for (int i = 0; i < productImages.Count; i++)
        {
            ProductImageSaveRequest imageListItem = productImages[i];

            PromotionProductFileSaveRequest? relatedPromotionFile = null;

            if (imageListItem.RelatedPromotionFileEditorId != null)
            {
                int relatedPromotionFileIndex = promotionProductFilesCopy.FindIndex(
                    x => x.EditorId == imageListItem.RelatedPromotionFileEditorId);

                relatedPromotionFile = promotionProductFilesCopy[relatedPromotionFileIndex];

                promotionProductFilesCopy.RemoveAt(relatedPromotionFileIndex);
            }

            byte[]? imageFileData = await GetImageDataFromListItemAsync(
                imageListItem, imageListItem.UploadedImageFile, productImageService, productImageFileManagementService);

            ProductImageData? productImage = null;

            if (imageListItem.ExistingImageId != null)
            {
                productImage = existingImages.Find(x => x.Id == imageListItem.ExistingImageId);
            }

            ProductImageFileData? productImageFile = null;

            if (imageListItem.RelatedImageFile != null)
            {
                productImageFile = existingImageFiles.Find(x => x.Id == imageListItem.ExistingImageId);
            }

            ProductImagePreRequestData preRequestData = new()
            {
                ImageListItem = imageListItem,
                RelatedPromotionFile = relatedPromotionFile,
                RelatedImage = productImage,
                RelatedImageFile = productImageFile,
                ImageFileData = imageFileData,
            };

            imageRequests.Add(preRequestData);
        }

        List<PromotionProductFilePreRequestData> promotionProductFileRequests = new();

        for (int i = 0; i < promotionProductFilesCopy.Count; i++)
        {
            PromotionProductFilePreRequestData promotionFilePreRequestData = new()
            {
                RelatedPromotionFile = promotionProductFilesCopy[i],
            };

            promotionProductFileRequests.Add(promotionFilePreRequestData);
        }

        List<ProductDocumentPreRequestData> documentRequests = [];

        foreach (ProductDocumentSaveRequest productDocumentData in productDocuments)
        {
            byte[]? uploadedFileData = null;

            if (productDocumentData.File != null)
            {
                using MemoryStream memoryStream = new();

                await productDocumentData.File.CopyToAsync(memoryStream);

                uploadedFileData = memoryStream.ToArray();
            }

            ProductDocumentPreRequestData documentPreRequest = new()
            {
                DocumentData = productDocumentData,
                DocumentFileData = uploadedFileData
            };

            documentRequests.Add(documentPreRequest);
        }

        return new()
        {
            ProductProperties = productProperties,
            ProductImages = imageRequests,
            PromotionProductFiles = promotionProductFileRequests,
            ProductDocuments = documentRequests,
        };
    }

    private static List<ProductPropertyForProductUpsertRequest> GetPropertyRequests(
        List<ProductPropertySaveRequest> productProperties)
    {
        List<ProductPropertyForProductUpsertRequest> propertyUpsertRequests = new();

        foreach (ProductPropertySaveRequest productProperty in productProperties)
        {
            ProductPropertyForProductUpsertRequest upsertRequest = new()
            {
                ProductCharacteristicId = productProperty.ProductCharacteristicId!.Value,
                Value = productProperty.Value,
                CustomDisplayOrder = null,
            };

            propertyUpsertRequests.Add(upsertRequest);
        }

        return propertyUpsertRequests;
    } 

    private static List<ProductDocumentUpsertRequest> GetDocumentRequests(List<ProductDocumentPreRequestData> documentPreRequests)
    {
        List<ProductDocumentUpsertRequest> documentRequests = new();

        if (documentPreRequests.Count == 0) return documentRequests;

        foreach (ProductDocumentPreRequestData productDocumentData in documentPreRequests)
        {
            ProductDocumentUpsertRequest upsertRequest;

            ProductDocumentSaveRequest documentData = productDocumentData.DocumentData;

            string? newDescription = documentData.CurrentDescription?.Trim();

            if (newDescription == string.Empty)
            {
                newDescription = null;
            }

            if (documentData.ExistingDocumentId != null)
            {
                ProductDocumentUpdateForProductRequest documentUpdateRequest = new()
                {
                    ExistingId = documentData.ExistingDocumentId.Value,
                    Description = newDescription,
                };

                upsertRequest = new()
                {
                    Request = documentUpdateRequest,
                };

                documentRequests.Add(upsertRequest);

                continue;
            }

            ProductDocumentCreateForProductRequest documentCreateRequest = new()
            {
                FileData = productDocumentData.DocumentFileData!,
                FileExtension = documentData.FileExtension,
                Description = newDescription,
            };

            upsertRequest = new()
            {
                Request = documentCreateRequest,
            };

            documentRequests.Add(upsertRequest);
        }

        return documentRequests;
    }

    private static List<ProductImageAndPromotionFileUpsertRequest> GetImageRequestsAsync(
        List<ProductImagePreRequestData> preRequests)
    {
        List<ProductImageAndPromotionFileUpsertRequest> requests = [];

        for (int i = 0; i < preRequests.Count; i++)
        {
            ProductImagePreRequestData? preRequestData = preRequests[i];

            ProductImageAndPromotionFileUpsertRequest request = GetImageAndPromotionFileRequestAsync(
                preRequestData.ImageListItem,
                preRequestData.RelatedPromotionFile,
                preRequestData.RelatedImage,
                preRequestData.RelatedImageFile,
                preRequestData.ImageFileData,
                i);

            requests.Add(request);
        }

        return requests;
    }

    private static ProductImageAndPromotionFileUpsertRequest GetImageAndPromotionFileRequestAsync(
        ProductImageSaveRequest imageListItem,
        PromotionProductFileSaveRequest? relatedPromotionFile,
        ProductImageData? relatedImage,
        ProductImageFileData? relatedImageFile,
        byte[]? imageFileData,
        int index)
    {
        if (imageListItem.UploadedImageFile is not null)
        {
            string? imageFileExtension = Path.GetExtension(imageListItem.UploadedImageFile.FileName);

            ProductImageWithFileForProductUpsertRequest imageUpsertRequest = new()
            {
                ExistingImageId = imageListItem.ExistingImageId,
                FileExtension = imageFileExtension,
                ImageData = imageFileData!,
                FileUpsertRequest = new FileForImageForProductUpsertRequest()
                {
                    Active = imageListItem.RelatedImageFile?.Active ?? true,
                    CustomDisplayOrder = index + 1,
                },
            };

            return new(imageUpsertRequest);
        }
        else if (relatedPromotionFile is not null)
        {
            ServicePromotionProductImageUpsertRequest? servicePromotionProductImageUpsertRequest = null;

            if (imageListItem.ExistingImageId is not null)
            {
                servicePromotionProductImageUpsertRequest = new()
                {
                    ImageFileUpsertRequest = new()
                    {
                        Active = imageListItem.RelatedImageFile?.Active ?? false,
                        CustomDisplayOrder = index + 1,
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
        else if (imageListItem.ExistingImageId is not null)
        {
            string? imageFileExtension;

            if (relatedImageFile?.FileName is not null)
            {
                imageFileExtension = Path.GetExtension(relatedImageFile.FileName);
            }
            else
            {
                List<string> possibleFileExtensions = GetPossibleExtensionsFromContentType(relatedImage.ImageContentType!);

                imageFileExtension = possibleFileExtensions.First();
            }

            FileForImageForProductUpsertRequest? imageFileUpsertRequest = null;

            if (imageListItem.RelatedImageFile is not null)
            {
                imageFileUpsertRequest = new FileForImageForProductUpsertRequest()
                {
                    Active = imageListItem.RelatedImageFile?.Active ?? true,
                    CustomDisplayOrder = index + 1,
                };
            }

            ProductImageWithFileForProductUpsertRequest imageUpsertRequest = new()
            {
                ExistingImageId = imageListItem.ExistingImageId,
                FileExtension = imageFileExtension,
                ImageData = imageFileData!,
                FileUpsertRequest = imageFileUpsertRequest,
            };

            return new(imageUpsertRequest);
        }
        else if (relatedImageFile is not null)
        {
            if (imageListItem.RelatedImageFile?.ExistingFileInfoId is null)
            {
                ProductImageFileForProductCreateRequest imageFileCreateRequest = new()
                {
                    Active = imageListItem.RelatedImageFile?.Active,
                    CustomDisplayOrder = index + 1,
                    FileData = new()
                    {
                        FileName = relatedImageFile.FileName,
                        Data = imageFileData!,
                    },
                };

                return new(imageFileCreateRequest);
            }

            ProductImageFileForProductUpdateRequest imageFileUpdateRequest = new()
            {
                Id = imageListItem.RelatedImageFile.ExistingFileInfoId.Value,
                Active = imageListItem.RelatedImageFile.Active,
                UpdateDisplayOrderRequest = index + 1,
                UpdateFileDataRequest = new FileData()
                {
                    FileName = relatedImageFile.FileName,
                    Data = imageFileData!,
                },
            };

            return new(imageFileUpdateRequest);
        }

        return default!;
    }

    private static async Task<byte[]?> GetImageDataFromListItemAsync(
        ProductImageSaveRequest imageListItem,
        IFormFile? uploadedFile,
        IProductImageService productImageService,
        IProductImageFileManagementService productImageFileManagementService)
    {
        if (imageListItem.UploadedImageFile is not null)
        {
            if (uploadedFile == null) return null;

            using MemoryStream memoryStream = new();

            await uploadedFile.CopyToAsync(memoryStream);

            return memoryStream.ToArray();
        }

        int? existingImageId = imageListItem.ExistingImageId;
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
}
