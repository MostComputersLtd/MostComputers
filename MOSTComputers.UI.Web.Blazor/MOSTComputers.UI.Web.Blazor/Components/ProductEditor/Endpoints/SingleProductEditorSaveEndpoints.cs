using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
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
using MOSTComputers.Services.ProductRegister.Services.ProductProperties.Contacts;
using MOSTComputers.Services.ProductRegister.Services.ProductStatus.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Promotions.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Promotions.PromotionFiles.Contracts;
using MOSTComputers.Services.SearchStringOrigin.Services.Contracts;
using MOSTComputers.UI.Web.Blazor.Endpoints;
using MOSTComputers.Utils.OneOf;
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
        [JsonPropertyName("productId")] public int ProductId { get; set; }
        [JsonPropertyName("productProperties")] public List<ProductPropertySaveRequest> ProductProperties { get; set; } = [];
        [JsonPropertyName("productImages")] public List<SaveImageRequestBase> ProductImages { get; set; } = [];
        [JsonPropertyName("promotionProductFiles")] public List<PromotionProductFileSaveRequest> PromotionProductFiles { get; set; } = [];
        [JsonPropertyName("productDocuments")] public List<ProductDocumentSaveRequest> ProductDocuments { get; set; } = [];
        [JsonPropertyName("uploadedProductDocuments")] public List<ProductDocumentUploadRequest> UploadedProductDocuments { get; set; } = [];
    }

    private sealed class ProductPropertySaveRequest
    {
        [JsonPropertyName("productCharacteristicId")] public int? ProductCharacteristicId { get; set; }
        [JsonPropertyName("value")] public string? Value { get; set; }
    }

    [JsonPolymorphic]
    [JsonDerivedType(typeof(SavePromotionProductFileImageRequest), "promotionProductFile")]
    [JsonDerivedType(typeof(SaveImageWithFileRequest), "image")]
    [JsonDerivedType(typeof(SaveImageFileRequest), "imageFile")]
    [JsonDerivedType(typeof(SaveUploadedImageInitialDataRequest), "uploadedImage")]
    private class SaveImageRequestBase
    {
    }

    private sealed class SavePromotionProductFileImageRequest : SaveImageRequestBase
    {
        [JsonPropertyName("promotionFileEditorId")] public required string PromotionFileEditorId { get; set; }
        [JsonPropertyName("active")] public required bool Active { get; set; }
    }

    private sealed class SaveImageWithFileRequest : SaveImageRequestBase
    {
        [JsonPropertyName("id")] public required int Id { get; set; }
        [JsonPropertyName("saveFileRequest")] public SaveImageWithFileFileDataRequest? SaveFileRequest { get; set; }
    }

    private sealed class SaveImageWithFileFileDataRequest
    {
        [JsonPropertyName("active")] public required bool Active { get; set; }
    }

    private sealed class SaveImageFileRequest : SaveImageRequestBase
    {
        [JsonPropertyName("id")] public required int Id { get; set; }
        [JsonPropertyName("active")] public required bool Active { get; set; }
    } 

    private sealed class SaveUploadedImageInitialDataRequest : SaveImageRequestBase
    {
        [JsonPropertyName("editorId")] public required string EditorId { get; set; }
        [JsonPropertyName("active")] public required bool Active { get; set; }
    }

    private sealed class SaveUploadedImageRequest : SaveImageRequestBase
    {
        public required IFormFile UploadedFile { get; set; }
        public required bool Active { get; set; }
    }

    private sealed class PromotionProductFileSaveRequest
    {
        [JsonPropertyName("editorId")] public required string EditorId { get; set; }
        [JsonPropertyName("id")] public int? Id { get; set; }
        [JsonPropertyName("promotionFileId")] public required int PromotionFileId { get; set; }
        [JsonPropertyName("validFrom")] public DateTime? ValidFrom { get; set; }
        [JsonPropertyName("validTo")] public DateTime? ValidTo { get; set; }
        [JsonPropertyName("active")] public bool Active { get; set; }
    }

    private sealed class ProductDocumentSaveRequest
    {
        [JsonPropertyName("id")] public required int Id { get; set; }
        [JsonPropertyName("description")] public string? Description { get; set; }
    }

    private sealed class ProductDocumentUploadRequest
    {
        [JsonPropertyName("editorId")] public required string EditorId { get; set; }
        [JsonIgnore] public IFormFile File { get; set; } = default!;
        [JsonPropertyName("description")] public string? Description { get; set; }
    }

    internal const string InvalidIdErrorCode = "InvalidId";
    private const string _invalidRequestError = "Request data invalid";

    public static IEndpointRouteBuilder MapSingleProductEditorSaveEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPut("/save", SaveAllAsync)
            .AddEndpointFilter<AntiforgeryEndpointFilter>();

        return endpoints;
    }

    private static async Task<IResult> SaveAllAsync(
        HttpContext httpContext,
        [FromServices] IProductService productService,
        [FromServices] IProductPropertyService productPropertyService,
        [FromServices] IProductCharacteristicService productCharacteristicService,
        [FromServices] IProductImageService productImageService,
        [FromServices] IProductImageFileService productImageFileService,
        [FromServices] IProductImageFileManagementService productImageFileManagementService,
        [FromServices] IProductDataSaveService productDataSaveService,
        [FromServices] IProductWorkStatusesService productWorkStatusesService,
        [FromServices] IPromotionService promotionService,
        [FromServices] IPromotionProductFileService promotionProductFileService,
        [FromServices] IProductDocumentFileService productDocumentFileService,
        [FromServices] ISearchStringOriginService searchStringOriginService)
    {
        IFormCollection form = httpContext.Request.Form;

        StringValues requestValue = form["data"];

        if (requestValue.Count == 0)
        {
            return Results.Problem(statusCode: StatusCodes.Status400BadRequest, detail: _invalidRequestError);
        }

        SaveAllRequest? request = JsonSerializer.Deserialize<SaveAllRequest>(requestValue[0]!);

        if (request == null)
        {
            return Results.Problem(statusCode: StatusCodes.Status400BadRequest, detail: _invalidRequestError);
        }

        Console.WriteLine(JsonSerializer.Serialize(request, new JsonSerializerOptions
{
    WriteIndented = true
}));

        for (int i = 0; i < request.ProductImages.Count; i++)
        {
            SaveImageRequestBase imageRequest = request.ProductImages[i];

            if (imageRequest is not SaveUploadedImageInitialDataRequest initialUploadRequest) continue;

            IFormFile? file = form.Files.GetFile(initialUploadRequest.EditorId);

            if (file == null)
            {
                return Results.Problem(statusCode: StatusCodes.Status400BadRequest, detail: _invalidRequestError);
            }

            Console.WriteLine($"Image file: {initialUploadRequest.EditorId}, {file.ContentType}, {file.Length}");

            request.ProductImages[i] = new SaveUploadedImageRequest()
            {
                UploadedFile = file,
                Active = initialUploadRequest.Active,
            };
        }

        for (int i = 0; i < request.UploadedProductDocuments.Count; i++)
        {
            ProductDocumentUploadRequest documentUploadRequest = request.UploadedProductDocuments[i];

            IFormFile? file = form.Files.GetFile(documentUploadRequest.EditorId);

            if (file == null)
            {
                return Results.Problem(statusCode: StatusCodes.Status400BadRequest, detail: _invalidRequestError);
            }

            Console.WriteLine($"Document file: {documentUploadRequest.EditorId}, {file.ContentType}, {file.Length}");

            documentUploadRequest.File = file;
        }

        string currentUserName = httpContext.User.Identity?.Name!;

        int productId = request.ProductId;

        List<ProductImageData> existingImages = await productImageService.GetAllInProductWithoutFileDataAsync(productId);
        List<ProductImageFileData> existingImageFiles = await productImageFileService.GetAllInProductAsync(productId);

        SaveAllRequestValidator validator = new(existingImages, existingImageFiles);

        ValidationResult validationResult = validator.Validate(request);

        if (!validationResult.IsValid)
        {
            return ValidationResultProblem(validationResult);
        }

        List<ProductPropertyForProductUpsertRequest> propertyRequests = GetPropertyRequests(request.ProductProperties);

        List<ProductImageAndPromotionFileUpsertRequest> imageRequests = await GetImageAndPromotionFileRequestsAsync(
            request.ProductImages,
            existingImages,
            existingImageFiles,
            request.PromotionProductFiles,
            productImageService,
            productImageFileManagementService);

        List<ProductDocumentUpsertRequest> documentRequests = GetDocumentRequests(request.ProductDocuments, request.UploadedProductDocuments);

        ProductRelatedItemsFullSaveRequest saveAllRequest = new()
        {
            ProductId = request.ProductId,
            PropertyRequests = propertyRequests,
            ImageRequests = imageRequests,
            DocumentRequests = documentRequests,
            UpsertUserName = currentUserName,
        };

try{

        OneOf<Success, ValidationResult, UnexpectedFailureResult> result
            = await productDataSaveService.SaveAsync(saveAllRequest);

        return await result.MatchAsync(
            async success =>
            {
                MOSTComputers.Models.Product.Models.Product? product = await productService.GetByIdAsync(request.ProductId);

                if (product == null)
                {
                    return Results.Problem(statusCode: StatusCodes.Status500InternalServerError);
                }

                return await SingleProductEditorComponentEndpoints.GetSingleProductEditorPopupForProductAsync(
                    product,
                    productPropertyService,
                    productCharacteristicService,
                    productImageService,
                    productImageFileService,
                    productWorkStatusesService,
                    promotionService,
                    promotionProductFileService,
                    productDocumentFileService,
                    searchStringOriginService);
            },
            validationResult => ValidationResultProblem(validationResult),
            unexpectedFailureResult => Results.Problem(statusCode: StatusCodes.Status500InternalServerError, detail: GenericErrorDetail)
        );
}
catch(Exception e)
{
return Results.Json(new
    {
        Type = e.GetType().FullName,
        e.Message,
        e.StackTrace,
        InnerException = e.InnerException?.ToString()
    }, statusCode: StatusCodes.Status500InternalServerError);
}
    }

    private sealed class SaveAllRequestValidator : AbstractValidator<SaveAllRequest>
    {
        internal SaveAllRequestValidator(
            List<ProductImageData> productImages,
            List<ProductImageFileData> productImageFiles)
        {
            RuleForEach(x => x.ProductProperties)
                .SetValidator(new ProductPropertyDisplayDataValidator());

            RuleForEach(x => x.ProductImages)
                .SetValidator((saveAllRequest) =>
                    new SaveImageRequestBaseValidator(productImages, productImageFiles, saveAllRequest.PromotionProductFiles));

            RuleForEach(x => x.PromotionProductFiles)
                .SetValidator(new PromotionProductFileSaveRequestValidator());

            RuleForEach(x => x.UploadedProductDocuments)
                .SetValidator(new ProductDocumentUploadRequestValidator());
        }
    }

    private sealed class ProductPropertyDisplayDataValidator : AbstractValidator<ProductPropertySaveRequest>
    {
        internal ProductPropertyDisplayDataValidator()
        {
            RuleFor(x => x.ProductCharacteristicId).NotNull();
        }
    }

    private sealed class SaveImageRequestBaseValidator : AbstractValidator<SaveImageRequestBase>
    {
        internal SaveImageRequestBaseValidator(
            List<ProductImageData> productImages,
            List<ProductImageFileData> productImageFiles,
            List<PromotionProductFileSaveRequest> promotionProductFiles)
        {
            RuleFor(x => x).Custom((request, context) =>
            {
                ValidationResult validationResult = request switch
                {
                    SavePromotionProductFileImageRequest promotionRequest =>
                        new SavePromotionProductFileImageRequestValidator(promotionProductFiles)
                            .Validate(promotionRequest),

                    SaveImageWithFileRequest imageWithFileRequest =>
                        new SaveImageWithFileRequestValidator(productImages)
                            .Validate(imageWithFileRequest),

                    SaveImageFileRequest imageFileRequest =>
                        new SaveImageFileRequestValidator(productImageFiles)
                            .Validate(imageFileRequest),

                    SaveUploadedImageRequest uploadedImageRequest =>
                        new SaveUploadedImageRequestValidator()
                            .Validate(uploadedImageRequest),

                    _ => throw new NotSupportedException()
                };

                foreach (ValidationFailure error in validationResult.Errors)
                {
                    string propertyPath = context.PropertyPath;

                    if (!string.IsNullOrEmpty(error.PropertyName))
                    {
                        propertyPath += '.';
                        propertyPath += error.PropertyName;
                    }

                    error.PropertyName = propertyPath;

                    context.AddFailure(error);
                }
            });
        }
    }

    private sealed class SavePromotionProductFileImageRequestValidator : AbstractValidator<SavePromotionProductFileImageRequest>
    {
        internal SavePromotionProductFileImageRequestValidator(List<PromotionProductFileSaveRequest> promotionFileRequests)
        {
            RuleFor(x => x.PromotionFileEditorId)
                .Must(x => promotionFileRequests.Exists(promotionFile => promotionFile.EditorId == x))
                .WithErrorCode(InvalidIdErrorCode);
        }
    }

    private sealed class SaveImageWithFileRequestValidator : AbstractValidator<SaveImageWithFileRequest>
    {
        internal SaveImageWithFileRequestValidator(List<ProductImageData> productImages)
        {
            RuleFor(x => x.Id)
                .Must(x => productImages.Exists(image => image.Id == x))
                .WithErrorCode(InvalidIdErrorCode);
        }
    }

    private sealed class SaveImageFileRequestValidator : AbstractValidator<SaveImageFileRequest>
    {
        internal SaveImageFileRequestValidator(List<ProductImageFileData> productImageFiles)
        {
            RuleFor(x => x.Id)
                .Must(x => productImageFiles.Exists(imageFile => imageFile.Id == x))
                .WithErrorCode(InvalidIdErrorCode);
        }
    }

    private sealed class SaveUploadedImageRequestValidator : AbstractValidator<SaveUploadedImageRequest>
    {
        internal SaveUploadedImageRequestValidator()
        {
            RuleFor(x => x.UploadedFile).NotNull();
            RuleFor(x => x.UploadedFile.Length).GreaterThan(0);
        }
    }

    private sealed class PromotionProductFileSaveRequestValidator : AbstractValidator<PromotionProductFileSaveRequest>
    {
        internal PromotionProductFileSaveRequestValidator()
        {
            RuleFor(x => x.PromotionFileId).NotNull();
        }
    }

    private sealed class ProductDocumentUploadRequestValidator : AbstractValidator<ProductDocumentUploadRequest>
    {
        internal ProductDocumentUploadRequestValidator()
        {
            RuleFor(x => x.File).NotNull();
            RuleFor(x => x.File.Length).GreaterThan(0);
        }
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

    private static List<ProductDocumentUpsertRequest> GetDocumentRequests(
        List<ProductDocumentSaveRequest> saveRequests,
        List<ProductDocumentUploadRequest> uploadRequests)
    {
        List<ProductDocumentUpsertRequest> documentRequests = new();

        foreach (ProductDocumentSaveRequest saveRequest in saveRequests)
        {
            ProductDocumentUpdateForProductRequest documentUpdateRequest = new()
            {
                ExistingId = saveRequest.Id,
                Description = saveRequest.Description,
            };

            documentRequests.Add(new() { Request = documentUpdateRequest });
        }

        foreach (ProductDocumentUploadRequest uploadRequest in uploadRequests)
        {
            string fileExtension = Path.GetExtension(uploadRequest.File.Name);

            using MemoryStream memoryStream = new();

            uploadRequest.File.CopyTo(memoryStream);

            byte[] uploadedFileData = memoryStream.ToArray();

            ProductDocumentCreateForProductRequest documentCreateRequest = new()
            {
                FileExtension = fileExtension,
                FileData = uploadedFileData,
                Description = uploadRequest.Description,
            };

            documentRequests.Add(new() { Request = documentCreateRequest });
        }

        return documentRequests;
    }

    private static async Task<List<ProductImageAndPromotionFileUpsertRequest>> GetImageAndPromotionFileRequestsAsync(
        List<SaveImageRequestBase> imageRequests,
        List<ProductImageData> productImages,
        List<ProductImageFileData> productImageFiles,
        List<PromotionProductFileSaveRequest> promotionProductFileRequests,
        IProductImageService productImageService,
        IProductImageFileManagementService productImageFileManagementService)
    {
        List<PromotionProductFileSaveRequest> promotionProductFileRequestsCopy = new(promotionProductFileRequests);
        List<ProductImageAndPromotionFileUpsertRequest> output = new();

        for (int i = 0; i < imageRequests.Count; i++)
        {
            SaveImageRequestBase imageSaveRequest = imageRequests[i];

            if (imageSaveRequest is SavePromotionProductFileImageRequest promotionProductFileImageRequest)
            {
                int promotionFileRequestIndex
                    = promotionProductFileRequestsCopy.FindIndex(x => x.EditorId == promotionProductFileImageRequest.PromotionFileEditorId);

                PromotionProductFileSaveRequest promotionFileRequest = promotionProductFileRequestsCopy[promotionFileRequestIndex];

                promotionProductFileRequestsCopy.RemoveAt(promotionFileRequestIndex);

                PromotionProductFileForProductUpsertRequest promotionProductFileUpdateRequest = new()
                {
                    Id = promotionFileRequest.Id,
                    PromotionFileInfoId = promotionFileRequest.PromotionFileId,
                    Active = promotionFileRequest.Active,
                    ValidFrom = promotionFileRequest.ValidFrom,
                    ValidTo = promotionFileRequest.ValidTo,
                    UpsertInProductImagesRequest = new()
                    {
                        ImageFileUpsertRequest = new()
                        {
                            Active = promotionProductFileImageRequest.Active,
                            CustomDisplayOrder = i + 1,
                        },
                    },
                };

                output.Add(new(promotionProductFileUpdateRequest));
            }
            else if (imageSaveRequest is SaveImageWithFileRequest imageWithFileRequest)
            {
                string? imageFileExtension;

                ProductImageData productImage = productImages.First(x => x.Id == imageWithFileRequest.Id);
                ProductImageFileData? productImageFile = productImageFiles.Find(x => x.ImageId == imageWithFileRequest.Id);

                ProductImage? productImageWithData = await productImageService.GetByIdInAllImagesAsync(imageWithFileRequest.Id);

                if (productImageFile?.FileName != null)
                {
                    imageFileExtension = Path.GetExtension(productImageFile.FileName);
                }
                else
                {
                    List<string> possibleFileExtensions = GetPossibleExtensionsFromContentType(productImage.ImageContentType!);

                    imageFileExtension = possibleFileExtensions.First();
                }

                FileForImageForProductUpsertRequest? imageFileUpsertRequest = null;

                if (imageWithFileRequest.SaveFileRequest != null)
                {
                    imageFileUpsertRequest = new FileForImageForProductUpsertRequest()
                    {
                        Active = imageWithFileRequest.SaveFileRequest.Active,
                        CustomDisplayOrder = i + 1,
                    };
                }

                ProductImageWithFileForProductUpsertRequest imageUpsertRequest = new()
                {
                    ExistingImageId = imageWithFileRequest.Id,
                    FileExtension = imageFileExtension,
                    ImageData = productImageWithData!.ImageData!,
                    FileUpsertRequest = imageFileUpsertRequest,
                };

                output.Add(new(imageUpsertRequest));
            }
            else if (imageSaveRequest is SaveImageFileRequest imageFileRequest)
            {
                ProductImageFileData productImageFile = productImageFiles.First(x => x.Id == imageFileRequest.Id);

                OneOf<byte[], FileDoesntExistResult> getImageFileDataResult
                    = await productImageFileManagementService.GetImageAsync(productImageFile.FileName!);

                ProductImageFileForProductUpdateRequest imageFileUpdateRequest = new()
                {
                    Id = imageFileRequest.Id,
                    Active = imageFileRequest.Active,
                    UpdateDisplayOrderRequest = i + 1,
                    UpdateFileDataRequest = new FileData()
                    {
                        FileName = productImageFile.FileName!,
                        Data = getImageFileDataResult.AsT0,
                    },
                };

                output.Add(new(imageFileUpdateRequest));
            }
            else if (imageSaveRequest is SaveUploadedImageRequest uploadedImageRequest)
            {
                string? imageFileExtension = Path.GetExtension(uploadedImageRequest.UploadedFile.FileName);

                using MemoryStream memoryStream = new();

                await uploadedImageRequest.UploadedFile.CopyToAsync(memoryStream);

                byte[] uploadedImageData = memoryStream.ToArray();

                ProductImageWithFileForProductUpsertRequest imageUpsertRequest = new()
                {
                    ExistingImageId = null,
                    FileExtension = imageFileExtension,
                    ImageData = uploadedImageData!,
                    FileUpsertRequest = new FileForImageForProductUpsertRequest()
                    {
                        Active = uploadedImageRequest.Active,
                        CustomDisplayOrder = i + 1,
                    },
                };

                output.Add(new(imageUpsertRequest));
            }
        }

        foreach (PromotionProductFileSaveRequest promotionFileRequest in promotionProductFileRequestsCopy)
        {
            PromotionProductFileForProductUpsertRequest promotionProductFileUpdateRequest = new()
            {
                Id = promotionFileRequest.Id,
                PromotionFileInfoId = promotionFileRequest.PromotionFileId,
                Active = promotionFileRequest.Active,
                ValidFrom = promotionFileRequest.ValidFrom,
                ValidTo = promotionFileRequest.ValidTo,
                UpsertInProductImagesRequest = null,
            };

            output.Add(new(promotionProductFileUpdateRequest));
        }

        return output;
    }
}
