using FluentValidation;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.ProductStatuses;

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