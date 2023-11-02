using FluentValidation;
using FluentValidation.Results;
using MOSTComputers.Services.DAL.Models;
using MOSTComputers.Services.DAL.Models.Requests.Manifacturer;
using MOSTComputers.Services.DAL.Models.Responses;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.DAL.DAL.Repositories.Contracts;

public interface IManifacturerRepository
{
    IEnumerable<Manifacturer> GetAll();
    Manifacturer? GetById(uint id);
    OneOf<Success, UnexpectedFailureResult> Insert(ManifacturerCreateRequest insertRequest);
    OneOf<Success, UnexpectedFailureResult> Update(ManifacturerUpdateRequest updateRequest);
    bool Delete(uint id);
}