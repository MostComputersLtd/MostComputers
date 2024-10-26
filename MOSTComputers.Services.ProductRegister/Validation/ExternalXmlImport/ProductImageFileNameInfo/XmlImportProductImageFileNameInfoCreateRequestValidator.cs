using FluentValidation;
using MOSTComputers.Services.ProductRegister.Models.ExternalXmlImport.ProductImageFileNameInfo;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImageFileNameInfo;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;

namespace MOSTComputers.Services.ProductRegister.Validation.ExternalXmlImport.ProductImageFileNameInfo;

internal sealed class XmlImportProductImageFileNameInfoCreateRequestValidator : AbstractValidator<XmlImportServiceProductImageFileNameInfoCreateRequest>
{
    public XmlImportProductImageFileNameInfoCreateRequestValidator()
    {
        RuleFor(x => x.ProductId).GreaterThan(0);
        RuleFor(x => x.FileName).Must(IsNotNullEmptyOrWhiteSpace).MaximumLength(50);
        RuleFor(x => x.DisplayOrder).Must(NullOrGreaterThanZero);
        RuleFor(x => x.ImagesInImagesAllForProductCount).Must(NullOrGreaterThanOrEqualToZero);
    }
}