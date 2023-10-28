using FluentValidation;
using FluentValidation.Results;
using MOSTComputers.Services.DAL.Models;
using MOSTComputers.Services.DAL.Models.Requests.ProductProperty;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.DAL.DAL.Repositories.Contracts;

internal interface IProductPropertyRepository
{
    bool Delete(uint productId, uint characteristicId);
    IEnumerable<ProductProperty> GetAllInProduct(uint productId);
    ProductProperty? GetByNameAndProductId(string name, uint productId);
    OneOf<Success, ValidationResult> Insert(ProductPropertyCreateRequest createRequest, IValidator<ProductPropertyCreateRequest>? validator = null);
    OneOf<Success, ValidationResult> Update(ProductPropertyUpdateRequest updateRequest, IValidator<ProductPropertyUpdateRequest>? validator = null);
}