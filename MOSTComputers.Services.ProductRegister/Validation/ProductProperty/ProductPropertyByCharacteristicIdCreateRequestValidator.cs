using FluentValidation;
using MOSTComputers.Models.Product.Models.Requests.ProductProperty;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;

namespace MOSTComputers.Services.ProductRegister.Validation.ProductProperty;

internal sealed class ProductPropertyByCharacteristicIdCreateRequestValidator : AbstractValidator<ProductPropertyByCharacteristicIdCreateRequest>
{
    public ProductPropertyByCharacteristicIdCreateRequestValidator()
    {
        RuleFor(x => x.ProductId).GreaterThan(0);
        RuleFor(x => x.ProductCharacteristicId).GreaterThan(0);
        RuleFor(x => x.Value).Must(IsNotEmptyOrWhiteSpace).MaximumLength(200);
    }
}