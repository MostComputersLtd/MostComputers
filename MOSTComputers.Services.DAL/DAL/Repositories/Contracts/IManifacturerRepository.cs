using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Requests.Manifacturer;
using MOSTComputers.Models.Product.Models.Validation;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.DAL.DAL.Repositories.Contracts;

public interface IManifacturerRepository
{
    IEnumerable<Manifacturer> GetAll();
    Manifacturer? GetById(uint id);
    OneOf<uint, UnexpectedFailureResult> Insert(ManifacturerCreateRequest insertRequest);
    OneOf<Success, UnexpectedFailureResult> Update(ManifacturerUpdateRequest updateRequest);
    bool Delete(uint id);
}