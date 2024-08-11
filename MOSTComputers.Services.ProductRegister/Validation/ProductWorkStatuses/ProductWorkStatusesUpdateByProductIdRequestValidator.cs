using FluentValidation;
using MOSTComputers.Models.Product.Models.Requests.ProductWorkStatuses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOSTComputers.Services.ProductRegister.Validation.ProductWorkStatuses;
internal sealed class ProductWorkStatusesUpdateByProductIdRequestValidator : AbstractValidator<ProductWorkStatusesUpdateByProductIdRequest>
{
    public ProductWorkStatusesUpdateByProductIdRequestValidator()
    {
        RuleFor(x => x.ProductId).GreaterThan(0);
    }
}