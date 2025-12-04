using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Contracts;

namespace MOSTComputers.Services.ProductRegister.Services;
internal sealed class ManufacturerService : IManufacturerService
{
    public ManufacturerService(IManufacturerRepository manufacturerRepository)
    {
        _manufacturerRepository = manufacturerRepository;
    }

    private readonly IManufacturerRepository _manufacturerRepository;

    public async Task<List<Manufacturer>> GetAllAsync()
    {
        return await _manufacturerRepository.GetAllAsync();
    }

    public Task<List<Manufacturer>> GetAllWithActiveProductsAsync()
    {
        return _manufacturerRepository.GetAllWithActiveProductsAsync();
    }

    public async Task<Manufacturer?> GetByIdAsync(int id)
    {
        if (id <= 0) return null;

        return await _manufacturerRepository.GetByIdAsync(id);
    }
}