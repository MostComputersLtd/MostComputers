using FluentValidation;
using MOSTComputers.Models.Product.Models.Requests.ProductProperty;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;

namespace MOSTComputers.Services.ProductRegister.Validation.ProductProperty;

internal sealed class ProductPropertyByCharacteristicNameCreateRequestValidator : AbstractValidator<ProductPropertyByCharacteristicNameCreateRequest>
{
    public ProductPropertyByCharacteristicNameCreateRequestValidator()
    {
        RuleFor(x => x.ProductId).GreaterThan(0);
        RuleFor(x => x.ProductCharacteristicName).Must(IsNotNullEmptyOrWhiteSpace);
        RuleFor(x => x.Value).Must(IsNotEmptyOrWhiteSpace).MaximumLength(200);
    }
}