using FluentValidation;
using MOSTComputers.Models.Product.Models.ExternalXmlImport.Requests.ProductProperty;
using MOSTComputers.Models.Product.Models.Requests.ProductProperty;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;

namespace MOSTComputers.Services.ProductRegister.Validation.ExternalXmlImport.ProductProperty;

internal sealed class XmlImportProductPropertyUpdateByXmlDataRequestValidator : AbstractValidator<XmlImportProductPropertyUpdateByXmlDataRequest>
{
    public XmlImportProductPropertyUpdateByXmlDataRequestValidator()
    {
        RuleFor(x => x.ProductId).Must(NullOrGreaterThanZero);
        RuleFor(x => x.ProductCharacteristicId).Must(NullOrGreaterThanZero);
        RuleFor(x => x.Value).Must(IsNotEmptyOrWhiteSpace);
    }
}