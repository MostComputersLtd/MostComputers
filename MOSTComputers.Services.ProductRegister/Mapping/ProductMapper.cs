using MOSTComputers.Services.DataAccess.Products.Models.Requests.ProductIdentifiers.ProductGTINCode;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.ProductImage;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.ToDoLocalChanges;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductIdentifiers.ProductGTINCode;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImage;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImage.FirstImage;
using MOSTComputers.Services.ProductRegister.Models.Requests.ToDoLocalChanges;

namespace MOSTComputers.Services.ProductRegister.Mapping;
public sealed class ProductMapper
{
#pragma warning disable CA1822 // Mark members as static
    internal ProductImageCreateRequest Map(ServiceProductImageCreateRequest serviceCategoryUpdateRequest)
    {
        ProductImageCreateRequest target = new()
        {
            ProductId = serviceCategoryUpdateRequest.ProductId,
            HtmlData = serviceCategoryUpdateRequest.HtmlData,
            ImageData = serviceCategoryUpdateRequest.ImageData,
            ImageContentType = serviceCategoryUpdateRequest.ImageContentType
        };

        return target;
    }

    internal ProductFirstImageCreateRequest Map(ServiceProductFirstImageCreateRequest serviceCategoryUpdateRequest)
    {
        ProductFirstImageCreateRequest target = new()
        {
            ProductId = serviceCategoryUpdateRequest.ProductId,
            HtmlData = serviceCategoryUpdateRequest.HtmlData,
            ImageData = serviceCategoryUpdateRequest.ImageData,
            ImageContentType = serviceCategoryUpdateRequest.ImageContentType
        };

        return target;
    }

    internal ProductImageUpdateRequest Map(ServiceProductImageUpdateRequest serviceCategoryUpdateRequest)
    {
        ProductImageUpdateRequest target = new()
        {
            Id = serviceCategoryUpdateRequest.Id,
            HtmlData = serviceCategoryUpdateRequest.HtmlData,
            ImageData = serviceCategoryUpdateRequest.ImageData,
            ImageContentType = serviceCategoryUpdateRequest.ImageContentType,
        };

        return target;
    }

    internal ProductFirstImageUpdateRequest Map(ServiceProductFirstImageUpdateRequest serviceCategoryUpdateRequest)
    {
        ProductFirstImageUpdateRequest target = new()
        {
            ProductId = serviceCategoryUpdateRequest.ProductId,
            HtmlData = serviceCategoryUpdateRequest.HtmlData,
            ImageData = serviceCategoryUpdateRequest.ImageData,
            ImageContentType = serviceCategoryUpdateRequest.ImageContentType
        };

        return target;
    }

    internal ToDoLocalChangeCreateRequest Map(ServiceToDoLocalChangeCreateRequest serviceToDoLocalChangeCreateRequest)
    {
        ToDoLocalChangeCreateRequest target = new()
        {
            TableName = serviceToDoLocalChangeCreateRequest.TableName,
            TableElementId = serviceToDoLocalChangeCreateRequest.TableElementId,
            OperationType = serviceToDoLocalChangeCreateRequest.OperationType
        };

        return target;
    }
#pragma warning restore CA1822 // Mark members as static
}