using FluentValidation;
using MOSTComputers.Services.ProductRegister.Models.Requests.Product;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;

namespace MOSTComputers.Services.ProductRegister.Validation.Product;

internal class ProductFullUpdateRequestValidator : AbstractValidator<ProductFullUpdateRequest>
{
    public ProductFullUpdateRequestValidator()
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
           .WithMessage("Must not have more than one property corresponding to one characteristic.");

        RuleForEach(x => x.PropertyUpsertRequests).SetValidator(x => new LocalProductPropertyUpsertRequestValidator());

        RuleForEach(x => x.ImageAndFileNameUpsertRequests).SetValidator(new ImageAndImageFileNameUpsertRequestValidator());

        RuleFor(x => x.ImageAndFileNameUpsertRequests).Must(DoesNotHaveImagesWithTheSameId)
           .WithMessage("Must not have more than one image with the same id.");

        RuleFor(x => x.ImageAndFileNameUpsertRequests).Must(DoesNotHaveImageFileNamesWithTheSameImageNumber)
           .WithMessage("Must not have more than one image file name with the same image number.");

        RuleFor(x => x.ImageAndFileNameUpsertRequests).Must(DoesNotHaveImageFileNamesWithTheSameDisplayOrder)
           .WithMessage("Must not have more than one image file name with the same display order.");

        RuleFor(x => x.ImageAndFileNameUpsertRequests).Must(DoesNotHaveImageFileNamesWithOutOfBoundsDisplayOrder)
           .WithMessage("Must not have image file name with display order that is higher than all image file names.");

        RuleFor(x => x.CategoryId).Must(NullOrGreaterThanZero);  // not seen null
        RuleFor(x => x.ManifacturerId).Must(manifacturerId => NullOrGreaterThanOrEqualTo<short>(manifacturerId, -1));
        RuleFor(x => x.SubCategoryId).Must(NullOrGreaterThanOrEqualToZero);
    }
}