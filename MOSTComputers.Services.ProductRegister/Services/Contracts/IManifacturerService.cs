using FluentValidation;
using FluentValidation.Results;
using MOSTComputers.Services.DAL.Models;
using MOSTComputers.Services.DAL.Models.Requests.Manifacturer;
using MOSTComputers.Services.DAL.Models.Responses;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.ProductRegister.Services.Contracts;

public interface IManifacturerService
{
    IEnumerable<Manifacturer> GetAll();
    Manifacturer? GetById(uint id);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> Insert(ManifacturerCreateRequest createRequest, IValidator<ManifacturerCreateRequest>? validator = null);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> Update(ManifacturerUpdateRequest updateRequest, IValidator<ManifacturerUpdateRequest>? validator = null);
    bool Delete(uint id);
}