using FluentValidation.Results;
using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Models.Product.Models.Promotions.Groups;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Products.Models.Responses.Promotions.GroupPromotionImages;
using MOSTComputers.Services.ProductRegister.Models.Requests.PromotionGroups;
using MOSTComputers.Services.ProductRegister.Models.Responses;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.ProductRegister.Services.Promotions.Groups.Contracts;
public interface IGroupPromotionService : IGroupPromotionReadService
{ 
    string GetValidHtmlContentImageUrlReference(int imageRequestIndex);
    Task<OneOf<GroupPromotionCreateResult, ValidationResult, ImageFileAlreadyExistsResult, UnexpectedFailureResult>> InsertAsync(ServiceGroupPromotionContentCreateRequest createRequest);
    Task<OneOf<Success, NotFound, ValidationResult, ImageFileAlreadyExistsResult, FileDoesntExistResult, UnexpectedFailureResult>> UpdateAsync(ServiceGroupPromotionContentUpdateRequest updateRequest);
}