using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.LocalChangesHandling.Services.Contracts;

namespace MOSTComputers.Services.LocalChangesHandling.Services;

internal sealed class GetProductDataFromBeforeUpdateService : IGetProductDataFromBeforeUpdateService
{
    public GetProductDataFromBeforeUpdateService()
    {

    }

    public Product? GetProductBeforeUpdate(uint productId)
    {
        return null;
    }

    public bool HandleAfterUpdate(uint productId)
    {
        return true;
    }
}