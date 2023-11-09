using FluentValidation;
using MOSTComputers.Models.Product.Models.Requests.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;

namespace MOSTComputers.Services.ProductRegister.Validation.Product;

internal sealed class ProductUpdateRequestValidator : AbstractValidator<ProductUpdateRequest>
{
    public ProductUpdateRequestValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Name).Must(IsNotEmptyOrWhiteSpace).MaximumLength(30);
        RuleFor(x => x.AdditionalWarrantyPrice).GreaterThanOrEqualTo(0);
        RuleFor(x => x.AdditionalWarrantyTermMonths).GreaterThanOrEqualTo(0);
        RuleFor(x => x.StandardWarrantyPrice).Must(IsNotEmptyOrWhiteSpace).MaximumLength(50);
        RuleFor(x => x.StandardWarrantyTermMonths).GreaterThanOrEqualTo(0);
        RuleFor(x => x.DisplayOrder).Must(NullOrGreaterThanOrEqualToZero);
        RuleFor(x => x.PlShow).Must(NullOrGreaterThanOrEqualToZero);
        RuleFor(x => x.Price1).Must(NullOrGreaterThanOrEqualToZero);
        RuleFor(x => x.DisplayPrice).Must(NullOrGreaterThanOrEqualToZero);
        RuleFor(x => x.Price3).Must(NullOrGreaterThanOrEqualToZero);
        RuleFor(x => x.Promotionid).Must(NullOrGreaterThanZero);
        RuleFor(x => x.PromRid).Must(NullOrGreaterThanOrEqualToZero);
        RuleFor(x => x.PromotionPictureId).Must(NullOrGreaterThanOrEqualToZero);
        RuleFor(x => x.PromotionExpireDate).NotEqual(new DateTime());
        RuleFor(x => x.AlertPictureId).Must(NullOrGreaterThanOrEqualToZero);
        RuleFor(x => x.AlertExpireDate).NotEqual(new DateTime());
        RuleFor(x => x.PriceListDescription).Must(IsNotEmptyOrWhiteSpace);
        RuleFor(x => x.PartNumber1).Must(IsNotEmptyOrWhiteSpace);
        RuleFor(x => x.PartNumber2).Must(IsNotEmptyOrWhiteSpace);
        RuleFor(x => x.SearchString).Must(IsNotEmptyOrWhiteSpace);

        RuleForEach(x => x.Properties).SetValidator(x => new CurrentProductPropertyUpdateRequestValidator());
        RuleForEach(x => x.Images).SetValidator(x => new CurrentProductImageCreateUpdateValidator());
        RuleForEach(x => x.ImageFileNames).SetValidator(x => new CurrentProductImageFileNameInfoUpdateRequestValidator());

        RuleFor(x => x.CategoryID).Must(NullOrGreaterThanZero);
        RuleFor(x => x.ManifacturerId).Must(NullOrGreaterThanZero);
        RuleFor(x => x.SubCategoryId).Must(NullOrGreaterThanZero);
    }
}

internal sealed class CurrentProductPropertyUpdateRequestValidator : AbstractValidator<CurrentProductPropertyUpdateRequest>
{
    public CurrentProductPropertyUpdateRequestValidator()
    {
        RuleFor(x => x.ProductCharacteristicId).Must(NullOrGreaterThanZero);
        RuleFor(x => x.DisplayOrder).Must(NullOrGreaterThanOrEqualToZero);
        RuleFor(x => x.Value).Must(IsNotEmptyOrWhiteSpace).MaximumLength(200);
    }
}

internal sealed class CurrentProductImageCreateUpdateValidator : AbstractValidator<CurrentProductImageUpdateRequest>
{
    public CurrentProductImageCreateUpdateValidator()
    {
        RuleFor(x => x.XML).Must(IsNotEmptyOrWhiteSpace);
        RuleFor(x => x.ImageData);
        RuleFor(x => x.ImageFileExtension).Must(IsNotEmptyOrWhiteSpace).MaximumLength(50);
    }
}

internal sealed class CurrentProductImageFileNameInfoUpdateRequestValidator : AbstractValidator<CurrentProductImageFileNameInfoUpdateRequest>
{
    public CurrentProductImageFileNameInfoUpdateRequestValidator()
    {
        RuleFor(x => x.FileName).Must(IsNotEmptyOrWhiteSpace).MaximumLength(50);
        RuleFor(x => x.DisplayOrder).Must(NullOrGreaterThanZero);
        RuleFor(x => x.NewDisplayOrder).Must(NullOrGreaterThanZero);
    }
}