using FluentValidation;
using MOSTComputers.Services.DAL.Models.Requests.FailureData.FailedPropertyNameOfProduct;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;

namespace MOSTComputers.Services.ProductRegister.Validation.FailureData.FailedPropertyNameOfProduct;

internal sealed class FailedPropertyNameOfProductMultiCreateRequestValidator : AbstractValidator<FailedPropertyNameOfProductMultiCreateRequest>
{
    public FailedPropertyNameOfProductMultiCreateRequestValidator()
    {
        RuleFor(x => x.ProductId).GreaterThan(0);
        RuleForEach(x => x.PropertyNames).Must(IsNotNullEmptyOrWhiteSpace).MaximumLength(50);
    }
}