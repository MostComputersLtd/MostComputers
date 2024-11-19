using FluentValidation.Results;
using MOSTComputers.Models.Product.Models.ProductStatuses;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DAL.Models.Requests.ProductWorkStatuses;
using OneOf;

namespace MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
public interface IProductWorkStatusesRepository
{
    bool DeleteAll();
    bool DeleteByProductId(int productId);
    bool DeleteAllWithProductNewStatus(ProductNewStatusEnum productNewStatusEnum);
    bool DeleteAllWithProductXmlStatus(ProductXmlStatusEnum productXmlStatusEnum);
    bool DeleteAllWithReadyForImageInsert(bool readyForImageInsert);
    bool DeleteById(int productWorkStatusesId);
    IEnumerable<ProductWorkStatuses> GetAll();
    ProductWorkStatuses? GetByProductId(int productId);
    IEnumerable<ProductWorkStatuses> GetAllWithProductNewStatus(ProductNewStatusEnum productNewStatusEnum);
    IEnumerable<ProductWorkStatuses> GetAllWithProductXmlStatus(ProductXmlStatusEnum productXmlStatusEnum);
    IEnumerable<ProductWorkStatuses> GetAllWithReadyForImageInsert(bool readyForImageInsert);
    ProductWorkStatuses? GetById(int productWorkStatusesId);
    OneOf<int, ValidationResult, UnexpectedFailureResult> InsertIfItDoesntExist(ProductWorkStatusesCreateRequest createRequest);
    bool UpdateByProductId(ProductWorkStatusesUpdateByProductIdRequest updateRequest);
    bool UpdateById(ProductWorkStatusesUpdateByIdRequest updateRequest);
}