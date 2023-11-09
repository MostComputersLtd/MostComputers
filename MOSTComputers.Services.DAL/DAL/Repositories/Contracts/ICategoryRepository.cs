using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Requests.Category;
using MOSTComputers.Models.Product.Models.Validation;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.DAL.DAL.Repositories.Contracts;

public interface ICategoryRepository
{
    bool Delete(uint id);
    IEnumerable<Category> GetAll();
    Category? GetById(uint id);
    OneOf<Success, UnexpectedFailureResult> Insert(CategoryCreateRequest createRequest);
    OneOf<Success, UnexpectedFailureResult> Update(CategoryUpdateRequest updateRequest);
}