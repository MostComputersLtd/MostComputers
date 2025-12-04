using MOSTComputers.Models.Product.Models;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Contracts;
public interface IManufacturerRepository
{
    Task<List<Manufacturer>> GetAllAsync();
    Task<List<Manufacturer>> GetAllWithActiveProductsAsync();
    Task<Manufacturer?> GetByIdAsync(int id);
}