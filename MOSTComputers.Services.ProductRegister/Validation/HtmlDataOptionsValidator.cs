using FluentValidation;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImage;
using OneOf;

namespace MOSTComputers.Services.ProductRegister.Validation;

internal sealed class HtmlDataOptionsValidator : AbstractValidator<OneOf<UpdateHtmlDataToMatchCurrentProductData, DoNotUpdateHtmlData, UpdateToCustomHtmlData>>
{
    public HtmlDataOptionsValidator()
    {
        When(x => x.IsT2, () =>
        {
            RuleFor(x => x.AsT2.HtmlData).NotEmptyOrWhiteSpace();
        });
    }
}
