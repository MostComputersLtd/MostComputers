using FluentValidation;
using FluentValidation.Results;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.ProductImages;
using MOSTComputers.Models.Product.Models.Promotions.Files;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImage.FileRelated;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImageAndPromotionFileSave;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductProperty;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;
using static MOSTComputers.Services.ProductRegister.Validation.CommonValueConstraints;
using static MOSTComputers.Utils.Files.ContentTypeUtils;

namespace MOSTComputers.Services.ProductRegister.Services.NewSaveService;

internal sealed class ProductDataSaveValidationDataProductIdValidator
    : AbstractValidator<(ProductRelatedItemsFullSaveRequest, Product?)>
{
    public ProductDataSaveValidationDataProductIdValidator()
    {
        RuleFor(x => x.Item1.ProductId).Custom((productId, context) =>
        {
            if (context.InstanceToValidate.Item2 is null)
            {
                context.AddFailure(new ValidationFailure(context.PropertyPath, "The product does not exist.")
                {
                    ErrorCode = ProductDataSaveValidationDataValidator.InvalidIdErrorCode,
                });
            }
        });
    }
}

internal sealed class ProductDataSaveValidationDataValidator : AbstractValidator<ProductRelatedItemsFullSaveRequest>
{
    internal const string InvalidIdErrorCode = "InvalidId";
    internal const string InvalidFileTypeErrorCode = "InvalidFileType";
    internal const string FileDoesntExistErrorCode = "FileDoesntExist";
    internal const string FileAlreadyExistsErrorCode = "FileAlreadyExists";
    internal const string DuplicateDisplayOrderErrorCode = "DuplicateDisplayOrder";
    internal const string DuplicateFileNameErrorCode = "DuplicateFileName";
    internal const string MaximumLengthErrorCode = "MaximumLengthValidator";

    private const string _invalidImageIdErrorMessage = "Image id does not correspond to any known image id";

    private const string _duplicateImageOrderErrorMessage = "Cannot have 2 image files with the same order";
    private const string _duplicateImageFileNameErrorMessage = "Cannot have 2 image files with the same name";

    private const string _invalidImageFileIdErrorMessage = "Image File id does not correspond to any known image id";
    private const string _invalidPromotionFileIdErrorMessage = "Id does not correspond to any known promotion file id";
    private const string _promotionFileIsNotAnImageErrorMessage = "Promotion file is not an image";
    private const string _invalidPromotionProductFileIdErrorMessage = "Promotion Product File id does not correspond to any known image id";
    private const string _invalidFileTypeErrorMessage = "File type not supported";

    private const string _fileDoesntExistErrorMessage = "File does not exist";
    private const string _fileAlreadyExistsErrorMessage = "File already exists";

    public ProductDataSaveValidationDataValidator(ProductDataSaveValidationData data)
    {
        RuleFor(x => x.UpsertUserName)
            .NotNullOrWhiteSpace()
            .MaximumLength(PromotionProductFileInfoConstraints.CreateUserNameMaxLength);

        RuleForEach(x => x.PropertyRequests)
            .SetValidator(x => new ProductPropertyForProductUpsertRequestValidator(data.ProductCharacteristics));

        RuleFor(x => x.ImageRequests)
            .Must(AreAllDisplayOrdersUnique)
            .WithErrorCode(DuplicateDisplayOrderErrorCode)
            .WithMessage(_duplicateImageOrderErrorMessage)
            .Must(AreAllFilenamesUnique)
            .WithErrorCode(DuplicateFileNameErrorCode)
            .WithMessage(_duplicateImageFileNameErrorMessage);

        RuleForEach(x => x.ImageRequests)
            .SetValidator((request, imageRequest) =>
        {
            InlineValidator<ProductImageAndPromotionFileUpsertRequest> validator = new();

            if (imageRequest.Request.IsT0)
            {
                ProductImageWithFileForProductUpsertRequestValidator imageValidator = new(data.Images);

                validator.RuleFor(x => x.Request.AsT0).SetValidator(imageValidator);
            }
            else if (imageRequest.Request.IsT1)
            {
                ProductImageFileForProductCreateRequestValidator imageValidator = new(data.AllImageFiles);

                validator.RuleFor(x => x.Request.AsT1).SetValidator(imageValidator);
            }
            else if (imageRequest.Request.IsT2)
            {
                ProductImageFileForProductUpdateRequestValidator imageValidator = new(
                    request.ProductId,
                    data.ImageFiles,
                    data.AllImageFiles);

                validator.RuleFor(x => x.Request.AsT2).SetValidator(imageValidator);
            }
            else
            {
                PromotionProductFileForProductUpsertRequestValidator imageValidator = new(
                    data.PromotionProductFiles,
                    data.PromotionFiles);

                validator.RuleFor(x => x.Request.AsT3).SetValidator(imageValidator);
            }

            return validator;
        });

        RuleForEach(x => x.DocumentRequests)
            .SetValidator(x => new ProductDocumentUpsertRequestValidator(data.ProductDocuments));
    }

    private static bool AreAllDisplayOrdersUnique(List<ProductImageAndPromotionFileUpsertRequest> imageRequests)
    {
        List<int> displayOrders = new();

        foreach (ProductImageAndPromotionFileUpsertRequest imageRequest in imageRequests)
        {
            int? displayOrder = imageRequest.Request.Match(
                x => x.FileUpsertRequest?.CustomDisplayOrder,
                x => x.CustomDisplayOrder,
                x => x.UpdateDisplayOrderRequest.IsT0 ? x.UpdateDisplayOrderRequest.AsT0 : null,
                x => x.UpsertInProductImagesRequest?.ImageFileUpsertRequest?.CustomDisplayOrder);

            if (displayOrder is null) continue;

            if (displayOrders.Contains(displayOrder.Value)) return false;

            displayOrders.Add(displayOrder.Value);
        }

        return true;
    }

    private static bool AreAllFilenamesUnique(List<ProductImageAndPromotionFileUpsertRequest> imageRequests)
    {
        List<string> fileNames = new();

        foreach (ProductImageAndPromotionFileUpsertRequest imageRequest in imageRequests)
        {
            string? fileName = imageRequest.Request.Match(
                x => null,
                x => x.FileData?.FileName,
                x => x.UpdateFileDataRequest.IsT0 ? x.UpdateFileDataRequest.AsT0?.FileName : null,
                x => null);

            if (fileName is null) continue;

            if (fileNames.Contains(fileName)) return false;

            fileNames.Add(fileName);
        }

        return true;
    }

    private sealed class ProductPropertyForProductUpsertRequestValidator
        : AbstractValidator<ProductPropertyForProductUpsertRequest>
    {
        public ProductPropertyForProductUpsertRequestValidator(List<ProductCharacteristic> characteristics)
        {
            RuleFor(x => x.ProductCharacteristicId)
                .GreaterThan(0)
                .Must(characteristicId => characteristics.Exists(characteristic => characteristic.Id == characteristicId))
                .WithErrorCode(InvalidIdErrorCode);

            RuleFor(x => x.Value).NotEmptyOrWhiteSpace();
        }
    }

    private sealed class ProductDocumentUpsertRequestValidator
        : AbstractValidator<ProductDocumentUpsertRequest>
    {
        public ProductDocumentUpsertRequestValidator(List<ProductDocument> productDocuments)
        {
            When(x => x.Request.IsT0, () =>
            {
                RuleFor(x => x.Request.AsT0.FileExtension)
                    .NotNullOrWhiteSpace()
                    .MaximumLength(ProductDocumentConstraints.FileExtensionMaxLength);

                RuleFor(x => x.Request.AsT0.FileData)
                    .NotNull()
                    .NotEmpty()
                    .Must(x => x.Length <= ProductDocumentConstraints.FileDataMaxSizeBytes)
                    .WithErrorCode(MaximumLengthErrorCode)
                    .IsIdentifiableImageData();

                RuleFor(x => x.Request.AsT0.Description)
                    .NotEmptyOrWhiteSpace()
                    .MaximumLength(ProductDocumentConstraints.DescriptionMaxLength);
            });

            When(x => x.Request.IsT1, () =>
            {
                RuleFor(x => x.Request.AsT1.ExistingId)
                    .GreaterThan(0)
                    .Must(id => productDocuments.Exists(document => document.Id == id))
                    .WithErrorCode(InvalidIdErrorCode);

                RuleFor(x => x.Request.AsT1.Description)
                    .NotEmptyOrWhiteSpace()
                    .MaximumLength(ProductDocumentConstraints.DescriptionMaxLength);
            });
        }
    }

    private sealed class ProductImageWithFileForProductUpsertRequestValidator
        : AbstractValidator<ProductImageWithFileForProductUpsertRequest>
    {
        public ProductImageWithFileForProductUpsertRequestValidator(
            List<ProductImageData> images)
        {
            RuleFor(x => x.ExistingImageId)
                .Must(id => id is null || images.Exists(x => x.Id == id))
                .WithErrorCode(InvalidIdErrorCode)
                .WithMessage(_invalidImageIdErrorMessage);

            RuleFor(x => x.FileExtension)
                .NotNullOrWhiteSpace()
                .Must(extension =>
                {
                    string? contentType = GetContentTypeFromExtension(extension!);

                    return contentType != null;
                })
                .WithErrorCode(InvalidFileTypeErrorCode)
                .WithMessage(_invalidFileTypeErrorMessage)
                .Must(extension =>
                {
                    string contentType = GetContentTypeFromExtension(extension!)!;

                    return contentType.Length <= ProductImageConstraints.ContentTypeMaxLength;
                })
                .WithErrorCode(MaximumLengthErrorCode);

            RuleFor(x => x.ImageData)
                .NotNull()
                .NotEmpty()
                .IsIdentifiableImageData();

            RuleFor(x => x.HtmlDataOptions.AsT2.HtmlData)
                .NotEmptyOrWhiteSpace()
                .When(x => x.HtmlDataOptions.IsT2);

            When(x => x.FileUpsertRequest != null, () =>
            {
                RuleFor(x => x.FileExtension)
                    .Must(x => 
                    {
                        string fileExtensionWithDot = x.StartsWith('.')
                            ? x
                            : '.' + x;

                        return fileExtensionWithDot.Length <= ProductImageFileDataConstraints.FileExtensionWhenSaveWithImageMaxLength;
                    })
                    .WithErrorCode(MaximumLengthErrorCode);
            });

            When(x => x.FileUpsertRequest?.CustomDisplayOrder != null, () =>
            {
                RuleFor(x => x.FileUpsertRequest!.CustomDisplayOrder).GreaterThan(0);
            });
        }
    }

    private sealed class ProductImageFileForProductCreateRequestValidator
        : AbstractValidator<ProductImageFileForProductCreateRequest>
    {
        public ProductImageFileForProductCreateRequestValidator(
            List<ProductImageFileData> allFileInfos)
        { 
            When(x => x.FileData != null, () =>
            {
                RuleFor(x => x.FileData!.FileName)
                    .NotNullOrWhiteSpace()
                    .MaximumLength(ProductImageFileDataConstraints.FileNameMaxLength)
                    .Must(fileName => !allFileInfos.Exists(x => x.FileName == fileName))
                    .WithErrorCode(FileAlreadyExistsErrorCode)
                    .WithMessage(_fileAlreadyExistsErrorMessage);

                RuleFor(x => x.FileData!.Data)
                    .NotNull()
                    .NotEmpty()
                    .IsIdentifiableImageData();
            });

            RuleFor(x => x.CustomDisplayOrder)
                .GreaterThan(0)
                .When(x => x.CustomDisplayOrder != null);
        }
    }

    private sealed class ProductImageFileForProductUpdateRequestValidator
        : AbstractValidator<ProductImageFileForProductUpdateRequest>
    {
        public ProductImageFileForProductUpdateRequestValidator(
            int productId,
            List<ProductImageFileData> productImageFiles,
            List<ProductImageFileData> allImageFiles)
        {
            RuleFor(x => x.Id)
                .Must(id => productImageFiles.Exists(x => x.Id == id))
                .WithErrorCode(InvalidIdErrorCode)
                .WithMessage(_invalidImageFileIdErrorMessage);

            RuleFor(x => x.UpdateDisplayOrderRequest.AsT0)
                .GreaterThan(0)
                .When(x => x.UpdateDisplayOrderRequest.IsT0);

            When(x => x.UpdateFileDataRequest.IsT0 && x.UpdateFileDataRequest.AsT0 != null, () =>
            {
                RuleFor(x => x.UpdateFileDataRequest.AsT0!.FileName)
                    .NotNullOrWhiteSpace()
                    .Must(fileName => allImageFiles.Exists(x => x.FileName == fileName))
                    .WithErrorCode(FileDoesntExistErrorCode)
                    .WithMessage(_fileDoesntExistErrorMessage)
                    .MaximumLength(ProductImageFileDataConstraints.FileNameMaxLength)
                    .Must(fileName => !allImageFiles.Exists(x => x.FileName == fileName && x.ProductId != productId))
                    .WithErrorCode(FileAlreadyExistsErrorCode)
                    .WithMessage(_fileAlreadyExistsErrorMessage);

                RuleFor(x => x.UpdateFileDataRequest.AsT0!.Data)
                    .NotNull()
                    .NotEmpty()
                    .IsIdentifiableImageData();
            });
        }
    }

    private sealed class PromotionProductFileForProductUpsertRequestValidator
        : AbstractValidator<PromotionProductFileForProductUpsertRequest>
    {
        public PromotionProductFileForProductUpsertRequestValidator(
            List<PromotionProductFileInfo> promotionProductFiles,
            List<PromotionFileInfo> promotionFiles)
        {
            RuleFor(x => x.Id)
                .Must(id => id is null || promotionProductFiles.Exists(x => x.Id == id))
                .WithErrorCode(InvalidIdErrorCode)
                .WithMessage(_invalidPromotionProductFileIdErrorMessage);

            RuleFor(x => x.PromotionFileInfoId)
                .GreaterThan(0)
                .Must(id => promotionFiles.Exists(x => x.Id == id))
                .WithErrorCode(InvalidIdErrorCode)
                .WithMessage(_invalidPromotionFileIdErrorMessage)
                .Must(id =>
                {
                    PromotionFileInfo promotionFile = promotionFiles.Find(x => x.Id == id)!;

                    return IsImageContentType(GetContentTypeFromExtension(promotionFile.FileName));
                })
                .WithErrorCode(InvalidFileTypeErrorCode)
                .WithMessage(_promotionFileIsNotAnImageErrorMessage);

            RuleFor(x => x.ValidTo)
                .GreaterThanOrEqualTo(x => x.ValidFrom)
                .When(x => x.ValidFrom != null && x.ValidTo != null);

            When(x => x.UpsertInProductImagesRequest != null, () =>
            {
                RuleFor(x => x.PromotionFileInfoId)
                    .Must(id => 
                    {
                        PromotionFileInfo promotionFile = promotionFiles.Find(x => x.Id == id)!;
                        
                        string fileExtension = Path.GetExtension(promotionFile.FileName);

                        return fileExtension.Length <= ProductImageFileDataConstraints.FileExtensionWhenSaveWithImageMaxLength;
                    })
                        .WithErrorCode(InvalidFileTypeErrorCode);

                RuleFor(x => x.UpsertInProductImagesRequest!.HtmlDataOptions.AsT2.HtmlData)
                    .NotEmptyOrWhiteSpace()
                    .When(x => x.UpsertInProductImagesRequest!.HtmlDataOptions.IsT2);

                RuleFor(x => x.UpsertInProductImagesRequest!.ImageFileUpsertRequest.CustomDisplayOrder)
                    .GreaterThan(0)
                    .When(x => x.UpsertInProductImagesRequest!.ImageFileUpsertRequest.CustomDisplayOrder != null);
            });
        }
    }
}
