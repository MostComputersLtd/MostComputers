using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DAL.Models.Requests.Manifacturer;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.DAL.DAL.Repositories.Contracts;

public interface IManifacturerRepository
{
    IEnumerable<Manifacturer> GetAll();
    Manifacturer? GetById(int id);
    OneOf<int, UnexpectedFailureResult> Insert(ManifacturerCreateRequest insertRequest);
    OneOf<Success, UnexpectedFailureResult> Update(ManifacturerUpdateRequest updateRequest);
    bool Delete(int id);
}