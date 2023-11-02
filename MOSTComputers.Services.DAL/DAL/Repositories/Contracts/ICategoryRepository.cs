using FluentValidation;
using FluentValidation.Results;
using MOSTComputers.Services.DAL.Models;
using MOSTComputers.Services.DAL.Models.Requests.Category;
using MOSTComputers.Services.DAL.Models.Responses;
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