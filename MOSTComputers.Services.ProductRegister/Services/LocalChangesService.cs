using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Changes;
using MOSTComputers.Models.Product.Models.Changes.Local;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Contracts;

using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;

namespace MOSTComputers.Services.ProductRegister.Services;

internal sealed class LocalChangesService : ILocalChangesService
{
    public LocalChangesService(ILocalChangesRepository localChangesRepository)
    {
        _localChangesRepository = localChangesRepository;
    }

    private readonly ILocalChangesRepository _localChangesRepository;

    public IEnumerable<LocalChangeData> GetAll()
    {
        return _localChangesRepository.GetAll();
    }

    public IEnumerable<LocalChangeData> GetAllForTable(string tableName)
    {
        return _localChangesRepository.GetAllForTable(tableName);
    }

    public IEnumerable<LocalChangeData> GetAllForOperationType(ChangeOperationTypeEnum operationType)
    {
        return _localChangesRepository.GetAllForOperationType(operationType);
    }

    public LocalChangeData? GetById(int id)
    {
        if (id <= 0) return null;

        return _localChangesRepository.GetById(id);
    }

    public LocalChangeData? GetByTableNameAndElementIdAndOperationType(string tableName, int elementId, ChangeOperationTypeEnum changeOperationType)
    {
        return _localChangesRepository.GetByTableNameAndElementIdAndOperationType(tableName, elementId, changeOperationType);
    }

    public bool DeleteById(int id)
    {
        if (id <= 0) return false;

        return _localChangesRepository.DeleteById(id);
    }

    public bool DeleteRangeByIds(IEnumerable<int> ids)
    {
        ids = RemoveValuesSmallerThanOne(ids);

        return _localChangesRepository.DeleteRangeByIds(ids);
    }

    public bool DeleteByTableNameAndElementId(string tableName, int elementId)
    {
        return _localChangesRepository.DeleteByTableNameAndElementId(tableName, elementId);
    }

    public bool DeleteRangeByTableNameAndElementIds(string tableName, IEnumerable<int> elementIds)
    {
        return _localChangesRepository.DeleteRangeByTableNameAndElementIds(tableName, elementIds);
    }
}