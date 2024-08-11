using FluentValidation;
using MOSTComputers.Models.Product.Models.Requests.ProductWorkStatuses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOSTComputers.Services.ProductRegister.Validation.ProductWorkStatuses;
internal sealed class ProductWorkStatusesUpdateByIdRequestValidator : AbstractValidator<ProductWorkStatusesUpdateByIdRequest>
{
    public ProductWorkStatusesUpdateByIdRequestValidator()
    {
        RuleFor(x => x.Id).NotNull().GreaterThan(0);
    }
}