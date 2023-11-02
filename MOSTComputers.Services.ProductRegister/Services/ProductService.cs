using FluentValidation;
using FluentValidation.Results;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
using MOSTComputers.Services.DAL.Models;
using MOSTComputers.Services.DAL.Models.Requests.Product;
using MOSTComputers.Services.DAL.Models.Responses;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using OneOf;
using OneOf.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOSTComputers.Services.ProductRegister.Services;

internal sealed class ProductService : IProductService
{
    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    private readonly IProductRepository _productRepository;

    public IEnumerable<Product> GetAllWithoutImagesAndProps()
    {
        return _productRepository.GetAll_WithManifacturerAndCategory();
    }

    public IEnumerable<Product> GetSelectionWithoutImagesAndProps(List<uint> ids)
    {
        return _productRepository.GetAll_WithManifacturerAndCategory_ByIds(ids);
    }

    public IEnumerable<Product> GetSelectionWithFirstImage(List<uint> ids)
    {
        return _productRepository.GetAll_WithManifacturerAndCategoryAndFirstImage_ByIds(ids);
    }

    public IEnumerable<Product> GetSelectionWithProps(List<uint> ids)
    {
        return _productRepository.GetAll_WithManifacturerAndCategoryAndProperties_ByIds(ids);
    }

    public IEnumerable<Product> GetFirstItemsBetweenStartAndEnd(uint start, uint end)
    {
        return _productRepository.GetFirstBetweenStartAndEnd_WithCategoryAndManifacturer(start, end);
    }

    public Product? GetByIdWithFirstImage(uint id)
    {
        return _productRepository.GetById_WithManifacturerAndCategoryAndFirstImage(id);
    }

    public Product? GetByIdWithProps(uint id)
    {
        return _productRepository.GetById_WithManifacturerAndCategoryAndProperties(id);
    }

    public Product? GetSelectionWithImages(uint id)
    {
        return _productRepository.GetById_WithManifacturerAndCategoryAndImages(id);
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> Insert(ProductCreateRequest createRequest,
        IValidator<ProductCreateRequest>? validator = null)
    {
        if (validator is not null)
        {
            ValidationResult validationResult = validator.Validate(createRequest);

            if (!validationResult.IsValid) return validationResult;
        }

        OneOf<Success, UnexpectedFailureResult> result = _productRepository.Insert(createRequest);

        return result.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
            success => success, unexpectedFailure => unexpectedFailure);
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> Update(ProductUpdateRequest updateRequest, IValidator<ProductUpdateRequest>? validator = null)
    {
        if (validator is not null)
        {
            ValidationResult validationResult = validator.Validate(updateRequest);

            if (!validationResult.IsValid) return validationResult;
        }

        OneOf<Success, UnexpectedFailureResult> result = _productRepository.Update(updateRequest);

        return result.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
            success => success, unexpectedFailure => unexpectedFailure);
    }

    public bool Delete(uint id)
    {
        return _productRepository.Delete(id);
    }
}