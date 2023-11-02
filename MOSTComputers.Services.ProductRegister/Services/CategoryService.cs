using FluentValidation;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
using MOSTComputers.Services.DAL.Models;
using MOSTComputers.Services.DAL.Models.Requests.Category;
using OneOf.Types;
using OneOf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.Results;
using MOSTComputers.Services.DAL.Models.Responses;
using MOSTComputers.Services.ProductRegister.Services.Contracts;

namespace MOSTComputers.Services.ProductRegister.Services;

internal sealed class CategoryService : ICategoryService
{
    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    private readonly ICategoryRepository _categoryRepository;

    public IEnumerable<Category> GetAll()
    {
        return _categoryRepository.GetAll();
    }

    public Category? GetById(uint id)
    {
        return _categoryRepository.GetById(id);
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> Insert(CategoryCreateRequest createRequest,
        IValidator<CategoryCreateRequest>? validator = null)
    {
        if (validator != null)
        {
            ValidationResult validationResult = validator.Validate(createRequest);

            if (!validationResult.IsValid) return validationResult;
        }

        OneOf<Success, UnexpectedFailureResult> result = _categoryRepository.Insert(createRequest);

        return result.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
            success => success, unexpectedFailure => unexpectedFailure);
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> Update(CategoryUpdateRequest updateRequest,
        IValidator<CategoryUpdateRequest>? validator = null)
    {
        if (validator != null)
        {
            ValidationResult validationResult = validator.Validate(updateRequest);

            if (!validationResult.IsValid) return validationResult;
        }

        OneOf<Success, UnexpectedFailureResult> result = _categoryRepository.Update(updateRequest);

        return result.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
            success => success, unexpectedFailure => unexpectedFailure);
    }

    public bool Delete(uint id)
    {
        return _categoryRepository.Delete(id);
    }
}