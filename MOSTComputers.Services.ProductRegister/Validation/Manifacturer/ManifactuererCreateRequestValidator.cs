using FluentValidation;
using MOSTComputers.Models.Product.Models.Requests.Manifacturer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;

namespace MOSTComputers.Services.ProductRegister.Validation.Manifacturer;

internal sealed class ManifacturerCreateRequestValidator : AbstractValidator<ManifacturerCreateRequest>
{
    public ManifacturerCreateRequestValidator()
    {
        RuleFor(x => x.DisplayOrder).Must(NullOrGreaterThanOrEqualToZero);
    }
}