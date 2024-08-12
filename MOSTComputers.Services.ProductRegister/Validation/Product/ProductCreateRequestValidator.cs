using FluentValidation;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Requests.Product;
using MOSTComputers.Models.Product.Models.Requests.ProductImage;
using MOSTComputers.Services.ProductRegister.Validation.ProductProperty;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        RuleFor(x => x.CategoryId).Must(NullOrGreaterThanZero);  // not seen null
        RuleFor(x => x.ManifacturerId).Must(manifacturerId => NullOrGreaterThanOrEqualTo<short>(manifacturerId, -1));
        RuleFor(x => x.SubCategoryId).Must(NullOrGreaterThanOrEqualToZero);
    }
}

internal sealed class CurrentProductPropertyCreateRequestValidator : AbstractValidator<CurrentProductPropertyCreateRequest>
{
    public CurrentProductPropertyCreateRequestValidator()
    {
        RuleFor(x => x.ProductCharacteristicId).Must(NullOrGreaterThanZero);
        RuleFor(x => x.DisplayOrder).Must(NullOrGreaterThanOrEqualToZero);
        RuleFor(x => x.Value).Must(IsNotEmptyOrWhiteSpace).MaximumLength(200);
    }
}

internal sealed class CurrentProductImageCreateRequestValidator : AbstractValidator<CurrentProductImageCreateRequest>
{
    public CurrentProductImageCreateRequestValidator()
    {
        RuleFor(x => x.HtmlData).Must(IsNotEmptyOrWhiteSpace);
        RuleFor(x => x.ImageData).NotEmpty();
        RuleFor(x => x.ImageContentType).Must(IsNotEmptyOrWhiteSpace).MaximumLength(50);
    }
}

internal sealed class CurrentProductImageFileNameInfoCreateRequestValidator : AbstractValidator<CurrentProductImageFileNameInfoCreateRequest>
{
    public CurrentProductImageFileNameInfoCreateRequestValidator()
    {
        RuleFor(x => x.FileName).Must(IsNotEmptyOrWhiteSpace).MaximumLength(50);
        RuleFor(x => x.DisplayOrder).Must(NullOrGreaterThanZero);
    }
}