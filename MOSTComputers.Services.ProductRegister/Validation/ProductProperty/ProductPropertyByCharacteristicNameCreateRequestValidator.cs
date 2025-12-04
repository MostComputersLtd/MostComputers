using FluentValidation;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductProperty;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;

namespace MOSTComputers.Services.ProductRegister.Validation.ProductProperty;
internal sealed class ProductPropertyByCharacteristicNameCreateRequestValidator : AbstractValidator<ServiceProductPropertyByCharacteristicNameCreateRequest>
{
    public ProductPropertyByCharacteristicNameCreateRequestValidator()
    {
        RuleFor(x => x.ProductId).GreaterThan(0);
        RuleFor(x => x.ProductCharacteristicName).NotNullOrWhiteSpace();
        RuleFor(x => x.Value).NotEmptyOrWhiteSpace();
    }
}