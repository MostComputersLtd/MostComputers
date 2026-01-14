using FluentValidation.Results;
using MOSTComputers.Models.Product.Models.Validation;
using OneOf;

namespace MOSTComputers.Services.DataAccess.Products.Models.Responses.ProductWorkStatuses;
public sealed class ProductWorkStatusesCreateManyWithSameDataResponse
{
    public required Dictionary<int, OneOf<int, ValidationResult, UnexpectedFailureResult>> Results { get; init; }
}