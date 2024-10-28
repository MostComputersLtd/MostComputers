using FluentValidation;
using MOSTComputers.Services.ProductRegister.Models.Requests.Product;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;

namespace MOSTComputers.Services.ProductRegister.Validation.Product;

internal class ProductUpdateWithoutImagesInDatabaseRequestValidator : AbstractValidator<ProductUpdateWithoutImagesInDatabaseRequest>
{
    public ProductUpdateWithoutImagesInDatabaseRequestValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
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
        RuleFor(x => x.PartNumber1).Must(IsNotEmptyOrWhiteSpace);   // seen empty
        RuleFor(x => x.PartNumber2).Must(IsNotEmptyOrWhiteSpace);   // seen empty
        RuleFor(x => x.SearchString).Must(IsNotEmptyOrWhiteSpace);  // seen empty

        RuleFor(x => x.PropertyUpsertRequests).Must(DoesNotHavePropertiesWithDuplicateCharacteristics)
            .WithMessage("Must not have more than one property corresponding to one characteristic");

        RuleForEach(x => x.PropertyUpsertRequests).SetValidator(x => new LocalProductPropertyUpsertRequestValidator());

        RuleForEach(x => x.ImageFileAndFileNameInfoUpsertRequests).SetValidator(new ImageFileAndFileNameInfoUpsertRequestValidator());

        RuleFor(x => x.ImageFileAndFileNameInfoUpsertRequests).Must(DoesNotHaveImageFileUpsertRequestsWithTheSameImageId)
           .WithMessage("Must not have more than one request with the same image id.");

        RuleFor(x => x.ImageFileAndFileNameInfoUpsertRequests).Must(DoesNotHaveImageFileUpsertRequestsWithTheSameOldFileName)
           .WithMessage("Must not have more than one request with the same old file name.");

        RuleFor(x => x.ImageFileAndFileNameInfoUpsertRequests).Must(DoesNotHaveImageFileUpsertRequestsWithTheSameDisplayOrder)
           .WithMessage("Must not have more than one request with the same display order.");

        RuleFor(x => x.CategoryId).Must(NullOrGreaterThanZero);  // not seen null
        RuleFor(x => x.ManifacturerId).Must(manifacturerId => NullOrGreaterThanOrEqualTo<short>(manifacturerId, -1));
        RuleFor(x => x.SubCategoryId).Must(NullOrGreaterThanOrEqualToZero);
    }
}