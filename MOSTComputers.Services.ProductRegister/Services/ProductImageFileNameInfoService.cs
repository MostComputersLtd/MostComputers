using FluentValidation;
using MOSTComputers.Services.DAL.DAL;
using MOSTComputers.Services.DAL.Models.Requests.ProductImageFileNameInfo;
using MOSTComputers.Services.DAL.Models;
using OneOf.Types;
using OneOf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
using FluentValidation.Results;
using MOSTComputers.Services.DAL.Models.Responses;
using MOSTComputers.Services.ProductRegister.Services.Contracts;

namespace MOSTComputers.Services.ProductRegister.Services;

internal sealed class ProductImageFileNameInfoService : IProductImageFileNameInfoService
{
    public ProductImageFileNameInfoService(IProductImageFileNameInfoRepository imageFileNameInfoRepository)
    {
        _imageFileNameInfoRepository = imageFileNameInfoRepository;
    }

    private readonly IProductImageFileNameInfoRepository _imageFileNameInfoRepository;

    public IEnumerable<ProductImageFileNameInfo> GetAll()
    {
        return _imageFileNameInfoRepository.GetAll();
    }

    public IEnumerable<ProductImageFileNameInfo> GetAllForProduct(uint productId)
    {
        return _imageFileNameInfoRepository.GetAllForProduct(productId);
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> Insert(ProductImageFileNameInfoCreateRequest createRequest,
        IValidator<ProductImageFileNameInfoCreateRequest>? validator = null)
    {
        if (validator is not null)
        {
            ValidationResult result = validator.Validate(createRequest);

            if (!result.IsValid) return result;
        }

        return _imageFileNameInfoRepository.Insert(createRequest);
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> Update(ProductImageFileNameInfoUpdateRequest updateRequest,
        IValidator<ProductImageFileNameInfoUpdateRequest>? validator = null)
    {
        if (validator is not null)
        {
            ValidationResult result = validator.Validate(updateRequest);

            if (!result.IsValid) return result;
        }

        return _imageFileNameInfoRepository.Update(updateRequest);
    }

    public bool DeleteAllForProductId(uint productId)
    {
        return _imageFileNameInfoRepository.DeleteAllForProductId(productId);
    }

    public bool DeleteByProductIdAndDisplayOrder(uint productId, int displayOrder)
    {
        return _imageFileNameInfoRepository.DeleteByProductIdAndDisplayOrder(productId, displayOrder);
    }
}