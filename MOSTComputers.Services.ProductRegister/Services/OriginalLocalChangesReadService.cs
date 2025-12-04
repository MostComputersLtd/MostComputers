using MOSTComputers.Models.Product.Models.Changes;
using MOSTComputers.Models.Product.Models.Changes.Local;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Contracts;

namespace MOSTComputers.Services.ProductRegister.Services;
internal class OriginalLocalChangesReadService : IOriginalLocalChangesReadService
{
    public OriginalLocalChangesReadService(IOriginalLocalChangesReadRepository originalLocalChangesReadRepository)
    {
        _originalLocalChangesReadRepository = originalLocalChangesReadRepository;
    }

    private readonly IOriginalLocalChangesReadRepository _originalLocalChangesReadRepository;

    public string ProductsTableName => "MOSTPRices";

    public async Task<List<LocalChangeData>> GetAllAsync()
    {
        return await _originalLocalChangesReadRepository.GetAllAsync();
    }

    public async Task<List<LocalChangeData>> GetAllForOperationTypeAsync(ChangeOperationType changeOperationType)
    {
        return await _originalLocalChangesReadRepository.GetAllForOperationTypeAsync(changeOperationType);
    }

    public async Task<List<LocalChangeData>> GetAllForTableNameAndOperationTypeAsync(string tableName, ChangeOperationType changeOperationType)
    {
        return await _originalLocalChangesReadRepository.GetAllForTableNameAndOperationTypeAsync(tableName, changeOperationType);
    }

    public async Task<List<LocalChangeData>> GetAllForTableAsync(string tableName)
    {
        return await _originalLocalChangesReadRepository.GetAllForTableAsync(tableName);
    }

    public async Task<LocalChangeData?> GetByIdAsync(int id)
    {
        return await _originalLocalChangesReadRepository.GetByIdAsync(id);
    }

    public async Task<LocalChangeData?> GetByTableNameAndElementIdAndOperationTypeAsync(string tableName, int elementId, ChangeOperationType changeOperationType)
    {
        return await _originalLocalChangesReadRepository.GetByTableNameAndElementIdAndOperationTypeAsync(tableName, elementId, changeOperationType);
    }
}