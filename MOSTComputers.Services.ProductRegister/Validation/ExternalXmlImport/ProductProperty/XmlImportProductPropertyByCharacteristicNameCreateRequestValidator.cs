using FluentValidation;
using MOSTComputers.Services.DAL.Models.Requests.ExternalXmlImport.ProductProperty;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;

namespace MOSTComputers.Services.ProductRegister.Validation.ExternalXmlImport.ProductProperty;

internal sealed class XmlImportProductPropertyByCharacteristicNameCreateRequestValidator
    : AbstractValidator<XmlImportProductPropertyByCharacteristicNameCreateRequest>
{
    public XmlImportProductPropertyByCharacteristicNameCreateRequestValidator()
    {
        RuleFor(x => x.ProductId).Must(NullOrGreaterThanZero);
        RuleFor(x => x.ProductCharacteristicName).Must(IsNotNullEmptyOrWhiteSpace);
        RuleFor(x => x.Value).Must(IsNotEmptyOrWhiteSpace);
    }
}