using FluentValidation;
using MOSTComputers.Services.ProductRegister.Models.ExternalXmlImport.ProductImage;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;

namespace MOSTComputers.Services.ProductRegister.Validation.ExternalXmlImport.ProductImage;

internal sealed class XmlImportProductImageUpdateRequestValidator : AbstractValidator<XmlImportServiceProductImageUpdateRequest>
{
    public XmlImportProductImageUpdateRequestValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.HtmlData).Must(IsNotEmptyOrWhiteSpace);
        RuleFor(x => x).Must(x => x.ImageData is not null == x.ImageContentType is not null);
        RuleFor(x => x.ImageData).Must(IsNullOrNotEmpty);
        RuleFor(x => x.ImageContentType).Must(IsNotEmptyOrWhiteSpace).MaximumLength(50);
    }
}