using MOSTComputers.Models.Product.Models.Requests.Category;
using MOSTComputers.Models.Product.Models.Requests.ProductImage;
using MOSTComputers.Models.Product.Models.Requests.Promotions;
using MOSTComputers.Services.ProductRegister.Models.Requests.Category;
using Riok.Mapperly.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOSTComputers.Services.ProductRegister.Mapping;

[Mapper]
public sealed partial class ProductMapper
{
    [MapperIgnoreTarget(nameof(CategoryCreateRequest.IsLeaf))]
    [MapperIgnoreTarget(nameof(CategoryCreateRequest.RowGuid))]
    internal partial CategoryCreateRequest Map(ServiceCategoryCreateRequest serviceCategoryCreateRequest);


    [MapperIgnoreTarget(nameof(CategoryUpdateRequest.RowGuid))]
    internal partial CategoryUpdateRequest Map(ServiceCategoryUpdateRequest serviceCategoryUpdateRequest);


    [MapperIgnoreTarget(nameof(ProductImageCreateRequest.DateModified))]
    internal partial ProductImageCreateRequest Map(ServiceProductImageCreateRequest serviceCategoryUpdateRequest);


    [MapperIgnoreTarget(nameof(ProductFirstImageCreateRequest.DateModified))]
    internal partial ProductFirstImageCreateRequest Map(ServiceProductFirstImageCreateRequest serviceCategoryUpdateRequest);


    [MapperIgnoreTarget(nameof(ProductImageUpdateRequest.DateModified))]
    internal partial ProductImageUpdateRequest Map(ServiceProductImageUpdateRequest serviceCategoryUpdateRequest);


    [MapperIgnoreTarget(nameof(ProductFirstImageUpdateRequest.DateModified))]
    internal partial ProductFirstImageUpdateRequest Map(ServiceProductFirstImageUpdateRequest serviceCategoryUpdateRequest);


    [MapperIgnoreTarget(nameof(PromotionCreateRequest.PromotionAddedDate))]
    internal partial PromotionCreateRequest Map(ServicePromotionCreateRequest serviceCategoryUpdateRequest);


    [MapperIgnoreTarget(nameof(PromotionCreateRequest.PromotionAddedDate))]
    internal partial PromotionUpdateRequest Map(ServicePromotionUpdateRequest serviceCategoryUpdateRequest);
}