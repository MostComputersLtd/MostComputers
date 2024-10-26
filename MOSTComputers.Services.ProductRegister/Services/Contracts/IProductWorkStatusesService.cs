using FluentValidation;
using FluentValidation.Results;
using MOSTComputers.Models.Product.Models.ProductStatuses;
using MOSTComputers.Models.Product.Models.Requests.ProductWorkStatuses;
using MOSTComputers.Models.Product.Models.Validation;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.ProductRegister.Services.Contracts;
public interface IProductWorkStatusesService
{
    IEnumerable<ProductWorkStatuses> GetAll();
    IEnumerable<ProductWorkStatuses> GetAllWithProductNewStatus(ProductNewStatusEnum productNewStatusEnum);
    IEnumerable<ProductWorkStatuses> GetAllWithProductXmlStatus(ProductXmlStatusEnum productXmlStatusEnum);
    IEnumerable<ProductWorkStatuses> GetAllWithReadyForImageInsert(bool readyForImageInsert);
    ProductWorkStatuses? GetByProductId(int productId);
    ProductWorkStatuses? GetById(int productWorkStatusesId);
    OneOf<int, ValidationResult, UnexpectedFailureResult> InsertIfItDoesntExist(ProductWorkStatusesCreateRequest createRequest, IValidator<ProductWorkStatusesCreateRequest>? validator = null);
    OneOf<bool, ValidationResult> UpdateByProductId(ProductWorkStatusesUpdateByProductIdRequest updateRequest, IValidator<ProductWorkStatusesUpdateByProductIdRequest>? validator = null);
    OneOf<bool, ValidationResult> UpdateById(ProductWorkStatusesUpdateByIdRequest updateRequest, IValidator<ProductWorkStatusesUpdateByIdRequest>? validator = null);
    bool DeleteAll();
    bool DeleteAllWithProductNewStatus(ProductNewStatusEnum productNewStatusEnum);
    bool DeleteAllWithProductXmlStatus(ProductXmlStatusEnum productXmlStatusEnum);
    bool DeleteAllWithReadyForImageInsert(bool readyForImageInsert);
    bool DeleteByProductId(int productId);
    bool DeleteById(int productWorkStatusesId);
}