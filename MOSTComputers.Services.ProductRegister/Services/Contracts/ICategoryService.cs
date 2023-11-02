using FluentValidation;
using FluentValidation.Results;
using MOSTComputers.Services.DAL.Models;
using MOSTComputers.Services.DAL.Models.Requests.Category;
using MOSTComputers.Services.DAL.Models.Responses;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.ProductRegister.Services.Contracts;

public interface ICategoryService
{
    IEnumerable<Category> GetAll();
    Category? GetById(uint id);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> Insert(CategoryCreateRequest createRequest, IValidator<CategoryCreateRequest>? validator = null);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> Update(CategoryUpdateRequest updateRequest, IValidator<CategoryUpdateRequest>? validator = null);
    bool Delete(uint id);
}