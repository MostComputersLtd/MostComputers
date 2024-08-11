using FluentValidation;
using FluentValidation.Results;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Requests.Category;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.ProductRegister.Models.Requests.Category;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.ProductRegister.Services.Contracts;

public interface ICategoryService
{
    IEnumerable<Category> GetAll();
    Category? GetById(int id);
    OneOf<int, ValidationResult, UnexpectedFailureResult> Insert(ServiceCategoryCreateRequest createRequest, IValidator<ServiceCategoryCreateRequest>? validator = null);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> Update(ServiceCategoryUpdateRequest updateRequest, IValidator<ServiceCategoryUpdateRequest>? validator = null);
    bool Delete(int id);
}