using FluentValidation;
using MOSTComputers.Services.ProductRegister.Models.Requests.Category;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;

namespace MOSTComputers.Services.ProductRegister.Validation.Category;

internal sealed class CategoryUpdateRequestValidator : AbstractValidator<ServiceCategoryUpdateRequest>
{
    public CategoryUpdateRequestValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Description).Must(IsNotEmptyOrWhiteSpace).MaximumLength(50);
        RuleFor(x => x.DisplayOrder).Must(NullOrGreaterThanZero);
        RuleFor(x => x.ProductsUpdateCounter).Must(NullOrGreaterThanOrEqualToZero);
    }
}