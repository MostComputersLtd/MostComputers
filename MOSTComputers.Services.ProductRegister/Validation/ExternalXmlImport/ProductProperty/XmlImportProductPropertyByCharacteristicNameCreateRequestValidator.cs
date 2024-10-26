using FluentValidation;
using MOSTComputers.Models.Product.Models.ExternalXmlImport.Requests.ProductProperty;
using MOSTComputers.Models.Product.Models.Requests.ProductProperty;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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