using FluentValidation;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductProperty;

namespace MOSTComputers.Services.ProductRegister.Validation.ProductProperty;

internal sealed class ProductPropertyForProductUpsertRequestValidator : AbstractValidator<ProductPropertyForProductUpsertRequest>
{
    public ProductPropertyForProductUpsertRequestValidator()
    {
        RuleFor(x => x.ProductCharacteristicId).GreaterThan(0);
        RuleFor(x => x.Value).NotEmptyOrWhiteSpace();
    }
}