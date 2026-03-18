using FluentValidation;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductDocuments;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;
using static MOSTComputers.Services.ProductRegister.Validation.CommonValueConstraints.ProductDocumentConstraints;

namespace MOSTComputers.Services.ProductRegister.Validation.ProductDocuments;

public sealed class ProductDocumentCreateRequestValidator : AbstractValidator<ServiceProductDocumentCreateRequest>
{
    public ProductDocumentCreateRequestValidator()
    {
        RuleFor(x => x.ProductId).GreaterThan(0);
        RuleFor(x => x.FileExtension).NotNullOrWhiteSpace().MaximumLength(FileExtensionMaxLength);
        RuleFor(x => x.FileData).NotNull().NotEmpty().MaximumLength(FileDataMaxSizeBytes);
        RuleFor(x => x.Description).NotEmptyOrWhiteSpace().MaximumLength(DescriptionMaxLength);
    }
}