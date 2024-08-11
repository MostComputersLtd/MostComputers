using FluentValidation.Results;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Requests.ProductCharacteristic;
using MOSTComputers.Models.Product.Models.Validation;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.DAL.DAL.Repositories.Contracts;

public interface IProductCharacteristicsRepository
{
    IEnumerable<ProductCharacteristic> GetAllCharacteristicsAndSearchStringAbbreviationsByCategoryId(int categoryId);
    IEnumerable<ProductCharacteristic> GetAllCharacteristicsByCategoryId(int categoryId);
    IEnumerable<ProductCharacteristic> GetAllSearchStringAbbreviationsByCategoryId(int categoryId);
    IEnumerable<IGrouping<int, ProductCharacteristic>> GetCharacteristicsAndSearchStringAbbreviationsForSelectionOfCategoryIds(IEnumerable<int> categoryIds);
    IEnumerable<IGrouping<int, ProductCharacteristic>> GetCharacteristicsForSelectionOfCategoryIds(IEnumerable<int> categoryIds);
    IEnumerable<IGrouping<int, ProductCharacteristic>> GetSearchStringAbbreviationsForSelectionOfCategoryIds(IEnumerable<int> categoryIds);
    ProductCharacteristic? GetById(int id);
    ProductCharacteristic? GetByCategoryIdAndName(int categoryId, string name);
    ProductCharacteristic? GetByCategoryIdAndNameAndCharacteristicType(int categoryId, string name, ProductCharacteristicTypeEnum productCharacteristicType);
    IEnumerable<ProductCharacteristic> GetSelectionByCategoryIdAndNames(int categoryId, List<string> names);
    OneOf<int, ValidationResult, UnexpectedFailureResult> Insert(ProductCharacteristicCreateRequest createRequest);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateById(ProductCharacteristicByIdUpdateRequest updateRequest);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateByNameAndCategoryId(ProductCharacteristicByNameAndCategoryIdUpdateRequest updateRequest);
    bool Delete(int id);
    bool DeleteAllForCategory(int categoryId);
}
