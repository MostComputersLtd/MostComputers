using FluentValidation;
using MOSTComputers.Services.DAL.Models.Requests.ProductCharacteristic;
using MOSTComputers.Services.DAL.Models;
using OneOf.Types;
using OneOf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.Results;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
using MOSTComputers.Services.DAL.Models.Responses;
using MOSTComputers.Services.ProductRegister.Services.Contracts;

namespace MOSTComputers.Services.ProductRegister.Services;

internal sealed class ProductCharacteristicService : IProductCharacteristicService
{
    public ProductCharacteristicService(IProductCharacteristicsRepository productCharacteristicsRepository)
    {
        _productCharacteristicsRepository = productCharacteristicsRepository;
    }

    private readonly IProductCharacteristicsRepository _productCharacteristicsRepository;

    public IEnumerable<ProductCharacteristic> GetAllByCategoryId(uint categoryId)
    {
        return _productCharacteristicsRepository.GetAllByCategoryId(categoryId);
    }

    public ProductCharacteristic? GetByCategoryIdAndName(uint categoryId, string name)
    {
        return _productCharacteristicsRepository.GetByCategoryIdAndName(categoryId, name);
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> Insert(ProductCharacteristicCreateRequest createRequest, IValidator<ProductCharacteristicCreateRequest>? validator = null)
    {
        if (validator is not null)
        {
            ValidationResult validationResult = validator.Validate(createRequest);

            if (!validationResult.IsValid) return validationResult;
        }

        return _productCharacteristicsRepository.Insert(createRequest);
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateById(ProductCharacteristicByIdUpdateRequest updateRequest, IValidator<ProductCharacteristicByIdUpdateRequest>? validator = null)
    {
        if (validator is not null)
        {
            ValidationResult result = validator.Validate(updateRequest);

            if (!result.IsValid) return result;
        }

        return _productCharacteristicsRepository.UpdateById(updateRequest);
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateByNameAndCategoryId(ProductCharacteristicByNameAndCategoryIdUpdateRequest updateRequest, IValidator<ProductCharacteristicByNameAndCategoryIdUpdateRequest>? validator = null)
    {
        if (validator is not null)
        {
            ValidationResult result = validator.Validate(updateRequest);

            if (!result.IsValid) return result;
        }

        return _productCharacteristicsRepository.UpdateByNameAndCategoryId(updateRequest);
    }

    public bool Delete(uint id)
    {
        return _productCharacteristicsRepository.Delete(id);
    }

    public bool DeleteAllForCategory(uint productId)
    {
        return _productCharacteristicsRepository.DeleteAllForCategory(productId);
    }
}