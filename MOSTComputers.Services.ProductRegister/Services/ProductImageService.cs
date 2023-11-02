using FluentValidation;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
using MOSTComputers.Services.DAL.Models.Requests.ProductImage;
using MOSTComputers.Services.DAL.Models.Responses;
using MOSTComputers.Services.DAL.Models;
using OneOf.Types;
using OneOf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.Results;
using MOSTComputers.Services.ProductRegister.Services.Contracts;

namespace MOSTComputers.Services.ProductRegister.Services;

internal sealed class ProductImageService : IProductImageService
{
    public ProductImageService(IProductImageRepository productImageRepository)
    {
        _productImageRepository = productImageRepository;
    }

    private readonly IProductImageRepository _productImageRepository;

    public IEnumerable<ProductImage> GetAllInProduct(uint productId)
    {
        return _productImageRepository.GetAllInProduct(productId);
    }

    public IEnumerable<ProductImage> GetAllFirstImagesForAllProducts()
    {
        return _productImageRepository.GetAllFirstImagesForAllProducts();
    }

    public IEnumerable<ProductImage> GetAllFirstImagesForAllProducts(List<uint> productIds)
    {
        return _productImageRepository.GetFirstImagesForSelectionOfProducts(productIds);
    }

    public ProductImage? GetByIdInAllImages(uint id)
    {
        return _productImageRepository.GetByIdInAllImages(id);
    }

    public ProductImage? GetFirstImageForProduct(uint productId)
    {
        return _productImageRepository.GetByProductIdInFirstImages(productId);
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> InsertInAllImages(ProductImageCreateRequest createRequest,
        IValidator<ProductImageCreateRequest>? validator = null)
    {
        if (validator != null)
        {
            ValidationResult validationResult = validator.Validate(createRequest);

            if (!validationResult.IsValid) return validationResult;
        }

        OneOf<Success, UnexpectedFailureResult> result = _productImageRepository.InsertInAllImages(createRequest);

        return result.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
            success => success, unexpectedFailure => unexpectedFailure);
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> InsertInFirstImages(ProductFirstImageCreateRequest createRequest,
        IValidator<ProductFirstImageCreateRequest>? validator = null)
    {
        if (validator != null)
        {
            ValidationResult validationResult = validator.Validate(createRequest);

            if (!validationResult.IsValid) return validationResult;
        }

        OneOf<Success, UnexpectedFailureResult> result = _productImageRepository.InsertInFirstImages(createRequest);

        return result.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
            success => success, unexpectedFailure => unexpectedFailure);
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateInAllImages(ProductImageUpdateRequest updateRequest,
        IValidator<ProductImageUpdateRequest>? validator = null)
    {
        if (validator != null)
        {
            ValidationResult validationResult = validator.Validate(updateRequest);

            if (!validationResult.IsValid) return validationResult;
        }

        OneOf<Success, UnexpectedFailureResult> result = _productImageRepository.UpdateInAllImages(updateRequest);

        return result.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
            success => success, unexpectedFailure => unexpectedFailure);
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateInFirstImages(ProductFirstImageUpdateRequest updateRequest,
        IValidator<ProductFirstImageUpdateRequest>? validator = null)
    {
        if (validator != null)
        {
            ValidationResult validationResult = validator.Validate(updateRequest);

            if (!validationResult.IsValid) return validationResult;
        }

        OneOf<Success, UnexpectedFailureResult> result = _productImageRepository.UpdateInFirstImages(updateRequest);

        return result.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
            success => success, unexpectedFailure => unexpectedFailure);
    }

    public bool DeleteInAllImagesById(uint id)
    {
        return _productImageRepository.DeleteInAllImagesById(id);
    }

    public bool DeleteAllImagesForProduct(uint productId)
    {
        return _productImageRepository.DeleteAllWithSameProductIdInAllImages(productId);
    }

    public bool DeleteInFirstImagesByProductId(uint productId)
    {
        return _productImageRepository.DeleteInFirstImagesByProductId(productId);
    }
}