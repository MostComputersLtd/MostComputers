using FluentValidation;
using MOSTComputers.Services.ProductRegister.Models.Requests.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;

namespace MOSTComputers.Services.ProductRegister.Validation.Category;

internal sealed class CategoryCreateRequestValidator : AbstractValidator<ServiceCategoryCreateRequest>
{
    public CategoryCreateRequestValidator()
    {
        RuleFor(x => x.Description).Must(IsNotEmptyOrWhiteSpace).MaximumLength(50);
        RuleFor(x => x.DisplayOrder).NotEqual(0);
        RuleFor(x => x.ParentCategoryId).Must(NullOrGreaterThanOrEqualToZero);
    }
}