using FluentValidation;
using FluentValidation.Results;
using MOSTComputers.Services.DAL.Models;
using MOSTComputers.Services.DAL.Models.Requests.Manifacturer;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.DAL.DAL.Repositories.Contracts;

internal interface IManifacturerRepository
{
    IEnumerable<Manifacturer> GetAll();
    Manifacturer? GetById(uint id);
    OneOf<Success, ValidationResult> Insert(ManifacturerInsertRequest insertRequest, IValidator<ManifacturerInsertRequest>? validator = null);
    bool TryDelete(uint id);
    OneOf<Success, ValidationResult> Update(ManifacturerUpdateRequest updateRequest, IValidator<ManifacturerUpdateRequest>? validator = null);
}