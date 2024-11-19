using FluentValidation;
using MOSTComputers.Services.DAL.Models.Requests.FailureData.FailedPropertyNameOfProduct;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;

namespace MOSTComputers.Services.ProductRegister.Validation.FailureData.FailedPropertyNameOfProduct;

internal sealed class FailedPropertyNameOfProductCreateRequestValidator : AbstractValidator<FailedPropertyNameOfProductCreateRequest>
{
    public FailedPropertyNameOfProductCreateRequestValidator()
    {
        RuleFor(x => x.ProductId).GreaterThan(0);
        RuleFor(x => x.PropertyName).Must(IsNotNullEmptyOrWhiteSpace).MaximumLength(50);
    }
}