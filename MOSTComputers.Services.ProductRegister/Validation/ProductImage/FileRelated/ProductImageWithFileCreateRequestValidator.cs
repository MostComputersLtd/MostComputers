using FluentValidation;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImage.FileRelated;

using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;

namespace MOSTComputers.Services.ProductRegister.Validation.ProductImage.FileRelated;
internal sealed class ProductImageWithFileCreateRequestValidator : AbstractValidator<ProductImageWithFileCreateRequest>
{
    public ProductImageWithFileCreateRequestValidator()
    {
        RuleFor(x => x.ProductId).GreaterThan(0);
        RuleFor(x => x.FileExtension).NotNullOrWhiteSpace();
        RuleFor(x => x.ImageData).NotNull().NotEmpty();
        RuleFor(x => x.HtmlData).NotEmptyOrWhiteSpace();
        RuleFor(x => x.CustomDisplayOrder).NullOrGreaterThan(0);
        RuleFor(x => x.CreateUserName).NotNullOrWhiteSpace();
    }
}