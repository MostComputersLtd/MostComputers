using FluentValidation;
using MOSTComputers.Models.Product.Models.Requests.Promotions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;

namespace MOSTComputers.Services.ProductRegister.Validation.Promotion;

internal sealed class PromotionUpdateRequestValidator : AbstractValidator<PromotionUpdateRequest>
{
    public PromotionUpdateRequestValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Name).Must(IsNotEmptyOrWhiteSpace).MaximumLength(100);
        RuleFor(x => x.PromotionAddedDate).NotEqual(new DateTime(0));
        RuleFor(x => x.Source).Must(NullOrGreaterThanZero); // 4 or 8
        RuleFor(x => x.Type).Must(NullOrGreaterThanZero); // 1 or 2
        RuleFor(x => x.Status).Must(NullOrGreaterThanOrEqualToZero); // 0, 1, 3 or 4
        RuleFor(x => x.SPOID).Must(NullOrGreaterThanZero);
        RuleFor(x => x.DiscountUSD).Must(NullOrGreaterThanZero);
        RuleFor(x => x.DiscountEUR).Must(NullOrGreaterThanZero);
        RuleFor(x => x.StartDate).NotEqual(new DateTime(0));
        RuleFor(x => x.ExpirationDate).NotEqual(new DateTime(0));
        RuleFor(x => x.MinimumQuantity).Must(NullOrGreaterThanOrEqualToZero);
        RuleFor(x => x.MaximumQuantity).Must(NullOrGreaterThanOrEqualToZero);
        RuleFor(x => x.QuantityIncrement).Must(NullOrGreaterThanOrEqualToZero);

        RuleForEach(x => x.RequiredProductIds).GreaterThan(_ => 0);

        RuleFor(x => x.ExpQuantity).Must(NullOrGreaterThanOrEqualToZero);
        RuleFor(x => x.SoldQuantity).Must(NullOrGreaterThanOrEqualToZero);
        RuleFor(x => x.Consignation).Must(NullOrGreaterThanOrEqualToZero);
        RuleFor(x => x.Points).Must(NullOrGreaterThanOrEqualToZero);
        RuleFor(x => x.TimeStamp).Must(IsNotEmptyOrWhiteSpace).MaximumLength(50);

        RuleFor(x => x.ProductId).Must(NullOrGreaterThanZero);
        RuleFor(x => x.CampaignId).Must(NullOrGreaterThanOrEqualToZero);
        RuleFor(x => x.RegistrationId).Must(NullOrGreaterThanOrEqualToZero);
        RuleFor(x => x.PromotionVisualizationId).Must(NullOrGreaterThanOrEqualToZero);
    }
}