using FluentValidation;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
using MOSTComputers.Services.DAL.Models.Requests.Category;
using MOSTComputers.Services.DAL.Models;
using OneOf.Types;
using OneOf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.Results;
using MOSTComputers.Services.DAL.Models.Requests.Manifacturer;
using MOSTComputers.Services.DAL.Models.Responses;
using MOSTComputers.Services.ProductRegister.Services.Contracts;

namespace MOSTComputers.Services.ProductRegister.Services;

internal sealed class ManifacturerService : IManifacturerService
{
    public ManifacturerService(IManifacturerRepository manifacturerRepository)
    {
        _manifacturerRepository = manifacturerRepository;
    }

    private readonly IManifacturerRepository _manifacturerRepository;

    public IEnumerable<Manifacturer> GetAll()
    {
        return _manifacturerRepository.GetAll();
    }

    public Manifacturer? GetById(uint id)
    {
        return _manifacturerRepository.GetById(id);
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> Insert(ManifacturerCreateRequest createRequest,
        IValidator<ManifacturerCreateRequest>? validator = null)
    {
        if (validator != null)
        {
            ValidationResult validationResult = validator.Validate(createRequest);

            if (!validationResult.IsValid) return validationResult;
        }

        OneOf<Success, UnexpectedFailureResult> result = _manifacturerRepository.Insert(createRequest);

        return result.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
            success => success, unexpectedFailure => unexpectedFailure);
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> Update(ManifacturerUpdateRequest updateRequest,
        IValidator<ManifacturerUpdateRequest>? validator = null)
    {
        if (validator != null)
        {
            ValidationResult validationResult = validator.Validate(updateRequest);

            if (!validationResult.IsValid) return validationResult;
        }

        OneOf<Success, UnexpectedFailureResult> result = _manifacturerRepository.Update(updateRequest);

        return result.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
            success => success, unexpectedFailure => unexpectedFailure);
    }

    public bool Delete(uint id)
    {
        return _manifacturerRepository.Delete(id);
    }
}