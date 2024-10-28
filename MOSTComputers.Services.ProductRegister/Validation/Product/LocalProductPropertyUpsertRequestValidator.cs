using FluentValidation;
using MOSTComputers.Services.ProductRegister.Models.Requests.Product;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;

namespace MOSTComputers.Services.ProductRegister.Validation.Product;

internal sealed class LocalProductPropertyUpsertRequestValidator : AbstractValidator<LocalProductPropertyUpsertRequest>
{
    public LocalProductPropertyUpsertRequestValidator()
    {
        RuleFor(x => x.ProductCharacteristicId).GreaterThan(0);
        RuleFor(x => x.Value).Must(IsNotEmptyOrWhiteSpace).MaximumLength(200);
    }
}