using MOSTComputers.Models.Product.Models;

namespace MOSTComputers.Services.LocalChangesHandling.Services.Contracts;
public interface IGetProductDataFromBeforeUpdateService
{
    Product? GetProductBeforeUpdate(uint productId);
    bool HandleAfterUpdate(uint productId);
}