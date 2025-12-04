using FluentValidation;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImageFileData;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;
using static MOSTComputers.Services.ProductRegister.Validation.CommonValueConstraints.ProductImageFileNameInfoConstraints;

namespace MOSTComputers.Services.ProductRegister.Validation.ProductImageFile;
internal sealed class ProductImageFileRequestValidator
    : AbstractValidator<ProductImageFileUpdateRequest>
{
    public ProductImageFileRequestValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);

        RuleFor(x => x.UpdateUserName).NotNullOrWhiteSpace();

        RuleFor(x => x.UpdateImageIdRequest.AsT0).NullOrGreaterThan(0)
            .WithName(nameof(ProductImageFileUpdateRequest.UpdateImageIdRequest))
            .When(x => x.UpdateImageIdRequest.IsT0);

        RuleFor(x => x.NewDisplayOrder).NullOrGreaterThan(0);

        RuleFor(x => x.UpdateFileDataRequest).ChildRules(request =>
        {
            request.RuleFor(x => x!.FileData!.FileName).NotNullOrWhiteSpace().MaximumLength(FileNameMaxLength)
                .When(x => x!.FileData is not null);

            request.RuleFor(x => x!.FileData!.Data).NotNull().NotEmpty()
               .When(x => x!.FileData is not null);
        })
            .When(x => x is not null);
    }
}