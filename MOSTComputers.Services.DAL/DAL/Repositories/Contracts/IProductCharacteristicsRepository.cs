using FluentValidation.Results;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Requests.ProductCharacteristic;
using MOSTComputers.Models.Product.Models.Validation;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.DAL.DAL.Repositories.Contracts;

public interface IProductCharacteristicsRepository
{
    IEnumerable<ProductCharacteristic> GetAllCharacteristicsAndSearchStringAbbreviationsByCategoryId(uint categoryId);
    IEnumerable<ProductCharacteristic> GetAllCharacteristicsByCategoryId(uint categoryId);
    IEnumerable<ProductCharacteristic> GetAllSearchStringAbbreviationsByCategoryId(uint categoryId);
    IEnumerable<IGrouping<uint, ProductCharacteristic>> GetCharacteristicsAndSearchStringAbbreviationsForSelectionOfCategoryIds(IEnumerable<uint> categoryIds);
    IEnumerable<IGrouping<uint, ProductCharacteristic>> GetCharacteristicsForSelectionOfCategoryIds(IEnumerable<uint> categoryIds);
    IEnumerable<IGrouping<uint, ProductCharacteristic>> GetSearchStringAbbreviationsForSelectionOfCategoryIds(IEnumerable<uint> categoryIds);
    ProductCharacteristic? GetByCategoryIdAndName(uint categoryId, string name);
    IEnumerable<ProductCharacteristic> GetSelectionByCategoryIdAndNames(uint categoryId, List<string> names);
    OneOf<uint, ValidationResult, UnexpectedFailureResult> Insert(ProductCharacteristicCreateRequest createRequest);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateById(ProductCharacteristicByIdUpdateRequest updateRequest);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateByNameAndCategoryId(ProductCharacteristicByNameAndCategoryIdUpdateRequest updateRequest);
    bool Delete(uint id);
    bool DeleteAllForCategory(uint categoryId);
}
