using MOSTComputers.Models.Product.Models.Changes;
using MOSTComputers.Models.Product.Models.Changes.External;

namespace MOSTComputers.Services.ProductRegister.Services.Contracts;
internal interface IExternalChangesService
{
    bool DeleteById(uint id);
    bool DeleteByTableNameAndElementId(string tableName, int elementId);
    bool DeleteRangeByIds(IEnumerable<uint> ids);
    bool DeleteRangeByTableNameAndElementIds(string tableName, IEnumerable<int> elementIds);
    IEnumerable<ExternalChangeData> GetAll();
    IEnumerable<ExternalChangeData> GetAllForOperationType(ChangeOperationTypeEnum operationType);
    IEnumerable<ExternalChangeData> GetAllForTable(string tableName);
    ExternalChangeData? GetById(uint id);
    ExternalChangeData? GetByTableNameAndElementId(string tableName, int elementId);
}