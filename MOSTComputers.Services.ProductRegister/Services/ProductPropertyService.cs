using FluentValidation;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
using MOSTComputers.Services.DAL.Models.Requests.ProductProperty;
using MOSTComputers.Services.DAL.Models;
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

internal sealed class ProductPropertyService : IProductPropertyService
{
    public ProductPropertyService(IProductPropertyRepository productPropertyRepository)
    {
        _productPropertyRepository = productPropertyRepository;
    }

    private readonly IProductPropertyRepository _productPropertyRepository;

    public IEnumerable<ProductProperty> GetAllInProduct(uint productId)
    {
        return _productPropertyRepository.GetAllInProduct(productId);
    }

    public ProductProperty? GetByNameAndProductId(string name, uint productId)
    {
        return _productPropertyRepository.GetByNameAndProductId(name, productId);
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> Insert(ProductPropertyCreateRequest createRequest,
        IValidator<ProductPropertyCreateRequest>? validator = null)
    {
        if (validator != null)
        {
            ValidationResult validationResult = validator.Validate(createRequest);

            if (!validationResult.IsValid) return validationResult;
        }

        OneOf<Success, ValidationResult, UnexpectedFailureResult> result = _productPropertyRepository.Insert(createRequest);

        return result.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
            success => success, validationResult => validationResult, unexpectedResult => unexpectedResult);
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> Update(ProductPropertyUpdateRequest updateRequest,
        IValidator<ProductPropertyUpdateRequest>? validator = null)
    {
        if (validator != null)
        {
            ValidationResult validationResult = validator.Validate(updateRequest);

            if (!validationResult.IsValid) return validationResult;
        }

        OneOf<Success, ValidationResult, UnexpectedFailureResult> result = _productPropertyRepository.Update(updateRequest);

        return result.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
            success => success, validationResult => validationResult, unexpectedResult => unexpectedResult);
    }

    public bool Delete(uint productId, uint characteristicId)
    {
        return _productPropertyRepository.Delete(productId, characteristicId);
    }

    public bool DeleteAllForProduct(uint productId)
    {
        return _productPropertyRepository.DeleteAllForProduct(productId);
    }

    public bool DeleteAllForCharacteristic(uint characteristicId)
    {
        return _productPropertyRepository.DeleteAllForCharacteristic(characteristicId);
    }
}