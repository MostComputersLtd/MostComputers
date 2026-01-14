using MOSTComputers.Services.DataAccess.Documents.Models;

namespace MOSTComputers.Services.DataAccess.Documents.DataAccess.Contracts;
public interface IFirmDataRepository
{
    Task<List<FirmData>> GetAllAsync();
    Task<FirmData?> GetByIdAsync(int firmId);
}