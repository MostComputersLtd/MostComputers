using FluentValidation;
using MOSTComputers.Services.DAL.Models.Requests.Product;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;

namespace MOSTComputers.Services.ProductRegister.Validation.Product;

internal sealed class ProductCreateRequestValidator : AbstractValidator<ProductCreateRequest>
{
    public ProductCreateRequestValidator()
    {
        RuleFor(x => x.Name).Must(IsNotEmptyOrWhiteSpace).MaximumLength(30);
        RuleFor(x => x.AdditionalWarrantyPrice).GreaterThanOrEqualTo(0);
        RuleFor(x => x.AdditionalWarrantyTermMonths).Must(NullOrGreaterThanOrEqualToZero);  // not seen null
        RuleFor(x => x.StandardWarrantyPrice).Must(IsNotEmptyOrWhiteSpace).MaximumLength(50);
        RuleFor(x => x.StandardWarrantyTermMonths).Must(NullOrGreaterThanOrEqualToZero);  // not seen null
        RuleFor(x => x.DisplayOrder).Must(NullOrGreaterThanZero);  // not seen null
        RuleFor(x => x.PlShow).Must(NullOrGreaterThanOrEqualToZero);
        RuleFor(x => x.Price1).Must(NullOrGreaterThanOrEqualToZero);  // not seen null
        RuleFor(x => x.DisplayPrice).Must(NullOrGreaterThanOrEqualToZero);  // not seen null
        RuleFor(x => x.Price3).Must(NullOrGreaterThanOrEqualToZero);   // not seen null
        RuleFor(x => x.PromotionId).Must(NullOrGreaterThanZero);
        RuleFor(x => x.PromRid).Must(NullOrGreaterThanZero);
        RuleFor(x => x.PromotionPictureId).Must(NullOrGreaterThanZero);
        RuleFor(x => x.PromotionExpireDate).NotEqual(new DateTime(0));
        RuleFor(x => x.AlertPictureId).Must(NullOrGreaterThanOrEqualToZero);
        RuleFor(x => x.AlertExpireDate).NotEqual(new DateTime(0));
        RuleFor(x => x.PriceListDescription).Must(IsNotEmptyOrWhiteSpace);  // only null
        RuleFor(x => x.PartNumber1).Must(IsNotEmptyOrWhiteSpace); // seen empty
        RuleFor(x => x.PartNumber2).Must(IsNotEmptyOrWhiteSpace); // seen empty
        RuleFor(x => x.SearchString).Must(IsNotEmptyOrWhiteSpace);  // seen empty

        RuleFor(x => x.Properties).Must(DoesNotHavePropertiesWithDuplicateCharacteristics)
            .WithMessage("Must not have more than one property corresponding to one characteristic");

        RuleForEach(x => x.Properties).SetValidator(x => new CurrentProductPropertyCreateRequestValidator());
        RuleForEach(x => x.Images).SetValidator(x => new CurrentProductImageCreateRequestValidator());
        RuleForEach(x => x.ImageFileNames).SetValidator(x => new CurrentProductImageFileNameInfoCreateRequestValidator());

        RuleFor(x => x.CategoryId).Must(categoryId => NullOrGreaterThanOrEqualTo(categoryId, -1));  // not seen null
        RuleFor(x => x.ManifacturerId).Must(NullOrGreaterThanZero);
        RuleFor(x => x.SubCategoryId).Must(NullOrGreaterThanOrEqualToZero);
    }
}

internal sealed class CurrentProductPropertyCreateRequestValidator : AbstractValidator<CurrentProductPropertyCreateRequest>
{
    public CurrentProductPropertyCreateRequestValidator()
    {
        RuleFor(x => x.ProductCharacteristicId).GreaterThan(0);
        RuleFor(x => x.Value).Must(IsNotEmptyOrWhiteSpace).MaximumLength(200);
    }
}

internal sealed class CurrentProductImageCreateRequestValidator : AbstractValidator<CurrentProductImageCreateRequest>
{
    public CurrentProductImageCreateRequestValidator()
    {
        RuleFor(x => x.HtmlData).Must(IsNotEmptyOrWhiteSpace);
        RuleFor(x => x).Must(x => (x.ImageData is not null) == (x.ImageContentType is not null));
        RuleFor(x => x.ImageData).Must(IsNullOrNotEmpty);
        RuleFor(x => x.ImageContentType).Must(IsNotEmptyOrWhiteSpace).MaximumLength(50);
    }
}

internal sealed class CurrentProductImageFileNameInfoCreateRequestValidator : AbstractValidator<CurrentProductImageFileNameInfoCreateRequest>
{
    public CurrentProductImageFileNameInfoCreateRequestValidator()
    {
        RuleFor(x => x.FileName).Must(IsNotNullEmptyOrWhiteSpace).MaximumLength(50);
        RuleFor(x => x.DisplayOrder).Must(NullOrGreaterThanZero);
    }
}