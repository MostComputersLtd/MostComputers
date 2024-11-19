using FluentValidation;
using MOSTComputers.Services.ProductRegister.Models.ExternalXmlImport.ProductImageFileNameInfo;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;

namespace MOSTComputers.Services.ProductRegister.Validation.ExternalXmlImport.ProductImageFileNameInfo;
internal class XmlImportProductImageFileNameInfoByFileNameUpdateRequestValidator
    : AbstractValidator<XmlImportServiceProductImageFileNameInfoByFileNameUpdateRequest>
{
    public XmlImportProductImageFileNameInfoByFileNameUpdateRequestValidator()
    {
        RuleFor(x => x.ProductId).GreaterThan(0);
        RuleFor(x => x.FileName).Must(IsNotNullEmptyOrWhiteSpace).MaximumLength(50);
        RuleFor(x => x.NewDisplayOrder).Must(NullOrGreaterThanZero);
        RuleFor(x => x.NewFileName).Must(IsNotEmptyOrWhiteSpace).MaximumLength(50);
        RuleFor(x => x.ImagesInImagesAllForProductCount).Must(NullOrGreaterThanOrEqualToZero);
    }
}