using FluentValidation;
using MOSTComputers.Services.DAL.DAL;
using MOSTComputers.Services.DAL.Models.Requests.Promotions;
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
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Contracts;

namespace MOSTComputers.Services.ProductRegister.Services;

internal sealed class PromotionService : IPromotionService
{
    public PromotionService(IPromotionRepository promotionRepository)
    {
        _promotionRepository = promotionRepository;
    }

    private readonly IPromotionRepository _promotionRepository;

    public IEnumerable<Promotion> GetAll()
    {
        return _promotionRepository.GetAll();
    }

    public IEnumerable<Promotion> GetAllActive()
    {
        return _promotionRepository.GetAllActive();
    }

    public IEnumerable<Promotion> GetAllForProduct(uint productId)
    {
        return _promotionRepository.GetAllForProduct(productId);
    }

    public IEnumerable<Promotion> GetAllForSelectionOfProducts(List<uint> productIds)
    {
        return _promotionRepository.GetAllForSelectionOfProducts(productIds);
    }

    public IEnumerable<Promotion> GetAllActiveForSelectionOfProducts(List<uint> productIds)
    {
        return _promotionRepository.GetAllActiveForSelectionOfProducts(productIds);
    }

    public Promotion? GetActiveForProduct(uint productId)
    {
        return _promotionRepository.GetActiveForProduct(productId);
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> Insert(PromotionCreateRequest createRequest, IValidator<PromotionCreateRequest>? validator = null)
    {
        if (validator is not null)
        {
            ValidationResult validationResult = validator.Validate(createRequest);

            if (!validationResult.IsValid) return validationResult;
        }

        OneOf<Success, UnexpectedFailureResult> result = _promotionRepository.Insert(createRequest);

        return result.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
            success => success, unexpectedFailure => unexpectedFailure);
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> Update(PromotionUpdateRequest updateRequest, IValidator<PromotionUpdateRequest>? validator = null)
    {
        if (validator is not null)
        {
            ValidationResult validationResult = validator.Validate(updateRequest);

            if (!validationResult.IsValid) return validationResult;
        }

        OneOf<Success, UnexpectedFailureResult> result = _promotionRepository.Update(updateRequest);

        return result.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
            success => success, unexpectedFailure => unexpectedFailure);
    }
    public bool Delete(uint id)
    {
        return _promotionRepository.Delete(id);
    }

    public bool DeleteAllByProductId(uint productId)
    {
        return _promotionRepository.DeleteAllByProductId(productId);
    }
}