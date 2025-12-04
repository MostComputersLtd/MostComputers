using MOSTComputers.Models.Product.Models.Changes;
using MOSTComputers.Models.Product.Models.Changes.Local;

namespace MOSTComputers.Services.ProductRegister.Services.Contracts;
public interface IOriginalLocalChangesReadService
{
    string ProductsTableName { get; }

    Task<List<LocalChangeData>> GetAllAsync();
    Task<List<LocalChangeData>> GetAllForOperationTypeAsync(ChangeOperationType changeOperationType);
    Task<List<LocalChangeData>> GetAllForTableAsync(string tableName);
    Task<List<LocalChangeData>> GetAllForTableNameAndOperationTypeAsync(string tableName, ChangeOperationType changeOperationType);
    Task<LocalChangeData?> GetByIdAsync(int id);
    Task<LocalChangeData?> GetByTableNameAndElementIdAndOperationTypeAsync(string tableName, int elementId, ChangeOperationType changeOperationType);
}