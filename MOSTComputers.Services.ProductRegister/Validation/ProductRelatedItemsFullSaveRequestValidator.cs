using FluentValidation;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImage.FileRelated;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImageAndPromotionFileSave;
using MOSTComputers.Services.ProductRegister.Models.Requests.PromotionProductFileInfo;
using MOSTComputers.Services.ProductRegister.Validation.ProductImage.FileRelated;
using MOSTComputers.Services.ProductRegister.Validation.ProductProperty;

using static MOSTComputers.Services.ProductRegister.Validation.CommonValueConstraints;

namespace MOSTComputers.Services.ProductRegister.Validation;

internal sealed class ProductRelatedItemsFullSaveRequestValidator : AbstractValidator<ProductRelatedItemsFullSaveRequest>
{
    public ProductRelatedItemsFullSaveRequestValidator()
    {
        RuleForEach(x => x.PropertyRequests).SetValidator<ProductPropertyForProductUpsertRequestValidator>(_ => new());

        RuleForEach(x => x.ImageRequests).SetValidator<ProductImageAndPromotionFileUpsertRequestValidator>(_ => new());
    }
}

public class ProductImageAndPromotionFileUpsertRequestValidator : AbstractValidator<ProductImageAndPromotionFileUpsertRequest>
{
    public ProductImageAndPromotionFileUpsertRequestValidator()
    {
        When(x => x.Request.IsT0, () =>
        {
            RuleFor(x => x.Request.AsT0).SetValidator(new ProductImageWithFileForProductUpsertRequestValidator());
        });

        When(x => x.Request.IsT1, () =>
        {
            RuleFor(x => x.Request.AsT1).SetValidator(new ProductImageFileForProductCreateRequestValidator());
        });

        When(x => x.Request.IsT2, () =>
        {
            RuleFor(x => x.Request.AsT2).SetValidator(new ProductImageFileForProductUpdateRequestValidator());
        });

        When(x => x.Request.IsT3, () =>
        {
            RuleFor(x => x.Request.AsT3).SetValidator(new PromotionProductFileForProductUpsertRequestValidator());
        });
    }
}

internal sealed class ProductImageFileForProductCreateRequestValidator : AbstractValidator<ProductImageFileForProductCreateRequest>
{
    public ProductImageFileForProductCreateRequestValidator()
    {
        When(x => x.FileData is not null, () =>
        {
            RuleFor(x => x.FileData!.FileName).NotNullOrWhiteSpace().MaximumLength(ProductImageFileDataConstraints.FileNameMaxLength);
            RuleFor(x => x.FileData!.Data).NotNull().NotEmpty();
        });

        RuleFor(x => x.CustomDisplayOrder).NullOrGreaterThan(0);
    }
}

internal sealed class ProductImageFileForProductUpdateRequestValidator : AbstractValidator<ProductImageFileForProductUpdateRequest>
{
    public ProductImageFileForProductUpdateRequestValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);

        When(x => x.UpdateDisplayOrderRequest.IsT0, () =>
        {
            RuleFor(x => x.UpdateDisplayOrderRequest!.AsT0).GreaterThan(0);
        });

        When(x => x.UpdateFileDataRequest.IsT0, () =>
        {
            RuleFor(x => x.UpdateFileDataRequest!.AsT0).NotNull().NotEmpty();

            RuleFor(x => x.UpdateFileDataRequest.AsT0!.FileName).NotNullOrWhiteSpace().MaximumLength(ProductImageFileDataConstraints.FileNameMaxLength);

            RuleFor(x => x.UpdateFileDataRequest.AsT0!.Data).NotNull().NotEmpty();
        });
    }
}

internal sealed class PromotionProductFileForProductUpsertRequestValidator : AbstractValidator<PromotionProductFileForProductUpsertRequest>
{
    public PromotionProductFileForProductUpsertRequestValidator()
    {
        RuleFor(x => x.Id).NullOrGreaterThan(0);
        RuleFor(x => x.PromotionFileInfoId).GreaterThan(0);

        When(x => x.UpsertInProductImagesRequest is not null, () =>
        {
            RuleFor(x => x.UpsertInProductImagesRequest!).SetValidator<ServicePromotionProductImageUpsertRequestValidator>(_ => new());
        });
    }
}

internal sealed class ServicePromotionProductImageUpsertRequestValidator : AbstractValidator<ServicePromotionProductImageUpsertRequest>
{
    public ServicePromotionProductImageUpsertRequestValidator()
    {
        RuleFor(x => x.ImageFileUpsertRequest).SetValidator<ServicePromotionProductImageFileUpsertRequestValidator>(_ => new());
        RuleFor(x => x.HtmlDataOptions).SetValidator<HtmlDataOptionsValidator>(_ => new());
    }
}

internal sealed class ServicePromotionProductImageFileUpsertRequestValidator : AbstractValidator<ServicePromotionProductImageFileUpsertRequest>
{
    public ServicePromotionProductImageFileUpsertRequestValidator()
    {
        RuleFor(x => x.CustomDisplayOrder).NullOrGreaterThan(0);
    }
}