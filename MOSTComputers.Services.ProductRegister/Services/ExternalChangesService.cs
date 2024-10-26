using MOSTComputers.Models.Product.Models.Changes;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
using MOSTComputers.Models.Product.Models.Changes.External;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;

namespace MOSTComputers.Services.ProductRegister.Services;

internal sealed class ExternalChangesService : IExternalChangesService
{
    public ExternalChangesService(IExternalChangesRepository externalChangesRepository)
    {
        _externalChangesRepository = externalChangesRepository;
    }

    private readonly IExternalChangesRepository _externalChangesRepository;

    public IEnumerable<ExternalChangeData> GetAll()
    {
        return _externalChangesRepository.GetAll();
    }

    public IEnumerable<ExternalChangeData> GetAllForTable(string tableName)
    {
        return _externalChangesRepository.GetAllForTable(tableName);
    }

    public IEnumerable<ExternalChangeData> GetAllForOperationType(ChangeOperationTypeEnum operationType)
    {
        return _externalChangesRepository.GetAllForOperationType(operationType);
    }

    public IEnumerable<ExternalChangeData> GetAllByTableNameAndElementId(string tableName, int elementId)
    {
        if (string.IsNullOrWhiteSpace(tableName)
            || elementId <= 0)
        {
            return Enumerable.Empty<ExternalChangeData>();
        }

        return _externalChangesRepository.GetAllByTableNameAndElementId(tableName, elementId);
    }

    public ExternalChangeData? GetById(int id)
    {
        if (id <= 0) return null;

        return _externalChangesRepository.GetById(id);
    }

    public bool DeleteById(int id)
    {
        if (id <= 0) return false;

        return _externalChangesRepository.DeleteById(id);
    }

    public bool DeleteByTableNameAndElementIdAndOperationType(string tableName, int elementId, ChangeOperationTypeEnum operationType)
    {
        if (!string.IsNullOrWhiteSpace(tableName)
            || elementId <= 0)
        {
            return false;
        }

        return _externalChangesRepository.DeleteByTableNameAndElementIdAndOperationType(tableName, elementId, operationType);
    }

    public bool DeleteRangeByIds(IEnumerable<int> ids)
    {
        ids = RemoveValuesSmallerThanOne(ids);

        return _externalChangesRepository.DeleteRangeByIds(ids);
    }

    public bool DeleteAllByTableNameAndElementId(string tableName, int elementId)
    {
        return _externalChangesRepository.DeleteAllByTableNameAndElementId(tableName, elementId);
    }

    public bool DeleteRangeByTableNameAndElementIds(string tableName, IEnumerable<int> elementIds)
    {
        elementIds = RemoveValuesSmallerThanOne(elementIds);

        return _externalChangesRepository.DeleteRangeByTableNameAndElementIds(tableName, elementIds);
    }
}