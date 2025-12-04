using MOSTComputers.Models.Product.Models;

namespace MOSTComputers.Services.ProductRegister.Services.Contracts;
public interface IManufacturerService
{
    Task<List<Manufacturer>> GetAllAsync();
    Task<List<Manufacturer>> GetAllWithActiveProductsAsync();
    Task<Manufacturer?> GetByIdAsync(int id);
}