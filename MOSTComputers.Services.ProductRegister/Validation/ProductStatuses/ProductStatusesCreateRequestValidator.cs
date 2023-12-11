using FluentValidation;
using MOSTComputers.Models.Product.Models.Requests.ProductStatuses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOSTComputers.Services.ProductRegister.Validation.ProductStatuses;

internal sealed class ProductStatusesCreateRequestValidator : AbstractValidator<ProductStatusesCreateRequest>
{
    public ProductStatusesCreateRequestValidator()
    {
        RuleFor(x => x.ProductId).GreaterThan(0);
        RuleFor(x => x.IsProcessed).NotNull();
        RuleFor(x => x.NeedsToBeUpdated).NotNull();
    }
}