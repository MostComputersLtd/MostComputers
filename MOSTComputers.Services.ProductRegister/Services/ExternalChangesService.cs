using MOSTComputers.Models.Product.Models.Changes;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
using MOSTComputers.Models.Product.Models.Changes.External;
using MOSTComputers.Services.ProductRegister.Services.Contracts;

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

    public ExternalChangeData? GetById(uint id)
    {
        return _externalChangesRepository.GetById(id);
    }

    public ExternalChangeData? GetByTableNameAndElementId(string tableName, int elementId)
    {
        return _externalChangesRepository.GetByTableNameAndElementId(tableName, elementId);
    }

    public bool DeleteById(uint id)
    {
        return _externalChangesRepository.DeleteById(id);
    }

    public bool DeleteRangeByIds(IEnumerable<uint> ids)
    {
        return _externalChangesRepository.DeleteRangeByIds(ids);
    }

    public bool DeleteByTableNameAndElementId(string tableName, int elementId)
    {
        return _externalChangesRepository.DeleteByTableNameAndElementId(tableName, elementId);
    }

    public bool DeleteRangeByTableNameAndElementIds(string tableName, IEnumerable<int> elementIds)
    {
        return _externalChangesRepository.DeleteRangeByTableNameAndElementIds(tableName, elementIds);
    }
}