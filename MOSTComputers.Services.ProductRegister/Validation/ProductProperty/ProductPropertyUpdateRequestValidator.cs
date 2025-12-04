using FluentValidation;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.ProductProperty;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;

namespace MOSTComputers.Services.ProductRegister.Validation.ProductProperty;
internal sealed class ProductPropertyUpdateRequestValidator : AbstractValidator<ProductPropertyUpdateRequest>
{
    public ProductPropertyUpdateRequestValidator()
    {
        RuleFor(x => x.ProductId).GreaterThan(0);
        RuleFor(x => x.ProductCharacteristicId).GreaterThan(0);
        RuleFor(x => x.Value).NotEmptyOrWhiteSpace();
    }
}