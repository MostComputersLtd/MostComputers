using FluentValidation;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
using OneOf.Types;
using OneOf;
using FluentValidation.Results;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Models.Product.Models;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;
using MOSTComputers.Services.DAL.Models.Requests.Manifacturer;

namespace MOSTComputers.Services.ProductRegister.Services;

internal sealed class ManifacturerService : IManifacturerService
{
    public ManifacturerService(
        IManifacturerRepository manifacturerRepository, 
        IValidator<ManifacturerCreateRequest>? createRequestValidator,
        IValidator<ManifacturerUpdateRequest>? updateRequestValidator)
    {
        _manifacturerRepository = manifacturerRepository;
        _createRequestValidator = createRequestValidator;
        _updateRequestValidator = updateRequestValidator;
    }

    private readonly IManifacturerRepository _manifacturerRepository;
    private readonly IValidator<ManifacturerCreateRequest>? _createRequestValidator;
    private readonly IValidator<ManifacturerUpdateRequest>? _updateRequestValidator;

    public IEnumerable<Manifacturer> GetAll()
    {
        return _manifacturerRepository.GetAll();
    }

    public Manifacturer? GetById(int id)
    {
        if (id <= 0) return null;

        return _manifacturerRepository.GetById(id);
    }

    public OneOf<int, ValidationResult, UnexpectedFailureResult> Insert(ManifacturerCreateRequest createRequest,
        IValidator<ManifacturerCreateRequest>? validator = null)
    {
        ValidationResult validationResult = ValidateTwoValidatorsDefault(createRequest, validator, _createRequestValidator);

        if (!validationResult.IsValid) return validationResult;

        OneOf<int, UnexpectedFailureResult> result = _manifacturerRepository.Insert(createRequest);

        return result.Match<OneOf<int, ValidationResult, UnexpectedFailureResult>>(
            id => id, unexpectedFailure => unexpectedFailure);
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> Update(ManifacturerUpdateRequest updateRequest,
        IValidator<ManifacturerUpdateRequest>? validator = null)
    {
        ValidationResult validationResult = ValidateTwoValidatorsDefault(updateRequest, validator, _updateRequestValidator);

        if (!validationResult.IsValid) return validationResult;

        OneOf<Success, UnexpectedFailureResult> result = _manifacturerRepository.Update(updateRequest);

        return result.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
            success => success, unexpectedFailure => unexpectedFailure);
    }

    public bool Delete(int id)
    {
        if (id <= 0) return false;

        return _manifacturerRepository.Delete(id);
    }
}