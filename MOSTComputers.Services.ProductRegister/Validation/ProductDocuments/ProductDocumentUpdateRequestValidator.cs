using FluentValidation;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductDocuments;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;
using static MOSTComputers.Services.ProductRegister.Validation.CommonValueConstraints.ProductDocumentConstraints;

namespace MOSTComputers.Services.ProductRegister.Validation.ProductDocuments;

public sealed class ProductDocumentUpdateRequestValidator : AbstractValidator<ServiceProductDocumentUpdateRequest>
{
    public ProductDocumentUpdateRequestValidator()
    {
        RuleFor(x => x.IdOrFileName.AsT0).GreaterThan(0)
            .When(x => x.IdOrFileName.IsT0);

        RuleFor(x => x.IdOrFileName.AsT1).NotNullOrWhiteSpace().MaximumLength(FileNameMaxLength)
            .When(x => x.IdOrFileName.IsT1);

        RuleFor(x => x.Description).NotEmptyOrWhiteSpace().MaximumLength(DescriptionMaxLength);
    }
}