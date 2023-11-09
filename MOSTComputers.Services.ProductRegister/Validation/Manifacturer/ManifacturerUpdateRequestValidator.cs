using FluentValidation;
using MOSTComputers.Models.Product.Models.Requests.Manifacturer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;

namespace MOSTComputers.Services.ProductRegister.Validation.Manifacturer;

internal sealed class ManifacturerUpdateRequestValidator : AbstractValidator<ManifacturerUpdateRequest>
{
    public ManifacturerUpdateRequestValidator()
    {
        RuleFor(x => x.Id).GreaterThanOrEqualTo(0);
        RuleFor(x => x.DisplayOrder).Must(NullOrGreaterThanOrEqualToZero);
    }
}