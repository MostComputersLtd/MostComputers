using FluentValidation.Results;
using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductDocuments;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.ProductRegister.Services.Contracts;
public interface IProductDocumentFileService
{
    Task<List<ProductDocument>> GetAllForProductAsync(int productId);
    Task<ProductDocument?> GetByIdAsync(int id);
    Stream? GetFileStreamByFileName(string fullFileName);
    Task<Stream?> GetFileStreamByIdAsync(int id);
    Task<OneOf<ProductDocument, ValidationResult, FileAlreadyExistsResult, UnexpectedFailureResult>> InsertAsync(ServiceProductDocumentCreateRequest createRequest);
    Task<OneOf<Success, NotFound, ValidationResult>> UpdateAsync(ServiceProductDocumentUpdateRequest updateRequest);
    Task<OneOf<Success, NotFound>> DeleteAsync(OneOf<int, string> idOrFileName);
}