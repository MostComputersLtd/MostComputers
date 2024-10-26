using FluentValidation;
using MOSTComputers.Models.Product.Models.ExternalXmlImport.Requests.ProductProperty;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;

namespace MOSTComputers.Services.ProductRegister.Validation.ExternalXmlImport.ProductProperty;

internal sealed class XmlImportProductPropertyByCharacteristicIdCreateRequestValidator
    : AbstractValidator<XmlImportProductPropertyByCharacteristicIdCreateRequest>
{
    public XmlImportProductPropertyByCharacteristicIdCreateRequestValidator()
    {
        RuleFor(x => x.ProductId).Must(NullOrGreaterThanZero);
        RuleFor(x => x.ProductCharacteristicId).Must(NullOrGreaterThanZero);
        RuleFor(x => x.Value).Must(IsNotEmptyOrWhiteSpace);
    }
}