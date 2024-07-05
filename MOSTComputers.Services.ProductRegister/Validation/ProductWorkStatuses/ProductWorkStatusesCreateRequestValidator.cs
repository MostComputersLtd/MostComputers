using FluentValidation;
using MOSTComputers.Models.Product.Models.Requests.ProductWorkStatuses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOSTComputers.Services.ProductRegister.Validation.ProductWorkStatuses;
internal class ProductWorkStatusesCreateRequestValidator : AbstractValidator<ProductWorkStatusesCreateRequest>
{
    public ProductWorkStatusesCreateRequestValidator()
    {
        RuleFor(x => x.ProductId).NotNull().GreaterThan(0);
    }
}