using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Models.Product.Models.ExternalXmlImport.Requests.ProductProperty;
using MOSTComputers.Models.Product.Models.FailureData.Requests.FailedPropertyNameOfProduct;
using MOSTComputers.Models.Product.Models.Requests.Manifacturer;
using MOSTComputers.Models.Product.Models.Requests.Product;
using MOSTComputers.Models.Product.Models.Requests.ProductCharacteristic;
using MOSTComputers.Models.Product.Models.Requests.ProductImage;
using MOSTComputers.Models.Product.Models.Requests.ProductProperty;
using MOSTComputers.Models.Product.Models.Requests.ProductStatuses;
using MOSTComputers.Models.Product.Models.Requests.ProductWorkStatuses;
using MOSTComputers.Models.Product.Models.Requests.Promotion;
using MOSTComputers.Services.ProductRegister.Mapping;
using MOSTComputers.Services.ProductRegister.Models.ExternalXmlImport.ProductImage;
using MOSTComputers.Services.ProductRegister.Models.ExternalXmlImport.ProductImageFileNameInfo;
using MOSTComputers.Services.ProductRegister.Models.Requests.Category;
using MOSTComputers.Services.ProductRegister.Models.Requests.Product;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImageFileNameInfo;
using MOSTComputers.Services.ProductRegister.Services;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Contracts.ExternalXmlImport;
using MOSTComputers.Services.ProductRegister.Services.ExternalXmlImport;
using MOSTComputers.Services.ProductRegister.Services.UsingCache;
using MOSTComputers.Services.ProductRegister.Validation.Category;
using MOSTComputers.Services.ProductRegister.Validation.ExternalXmlImport.ProductImage;
using MOSTComputers.Services.ProductRegister.Validation.ExternalXmlImport.ProductImageFileNameInfo;
using MOSTComputers.Services.ProductRegister.Validation.ExternalXmlImport.ProductProperty;
using MOSTComputers.Services.ProductRegister.Validation.FailureData.FailedPropertyNameOfProduct;
using MOSTComputers.Services.ProductRegister.Validation.Manifacturer;
using MOSTComputers.Services.ProductRegister.Validation.Product;
using MOSTComputers.Services.ProductRegister.Validation.ProductCharacteristic;
using MOSTComputers.Services.ProductRegister.Validation.ProductImage;
using MOSTComputers.Services.ProductRegister.Validation.ProductImageFileNameInfo;
using MOSTComputers.Services.ProductRegister.Validation.ProductProperty;
using MOSTComputers.Services.ProductRegister.Validation.ProductStatuses;
using MOSTComputers.Services.ProductRegister.Validation.ProductWorkStatuses;
using MOSTComputers.Services.ProductRegister.Validation.Promotion;
using static MOSTComputers.Services.DAL.Configuration.ConfigureServices;

namespace MOSTComputers.Services.ProductRegister.Configuration;

public static class ConfigureServices
{
    public static IServiceCollection AddProductServices(this IServiceCollection services, string connectionString)
    {
        services.AddDataAccess(connectionString)
            .AddAllRepositories();

        services.AddScoped<ProductMapper>();

        AddValidation(services);

        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IManifacturerService, ManifacturerService>();
        services.AddScoped<IProductImageFileNameInfoService, ProductImageFileNameInfoService>();
        services.AddScoped<IProductImageService, ProductImageService>();
        services.AddScoped<IProductCharacteristicService, ProductCharacteristicService>();
        services.AddScoped<IProductPropertyService, ProductPropertyService>();
        services.AddScoped<IPromotionService, PromotionService>();
        services.AddScoped<IProductStatusesService, ProductStatusesService>();

        services.AddScoped<IProductWorkStatusesService, ProductWorkStatusesService>();

        services.AddScoped<IProductService, ProductService>();

        //services.AddScoped<IProductManipulateService, ProductManipulateService>();

        services.AddScoped<ILocalChangesService, LocalChangesService>();
        
        services.AddScoped<IToDoLocalChangesService, ToDoLocalChangesService>();

        services.AddScoped<IExternalChangesService, ExternalChangesService>();

        services.AddScoped<IFailedPropertyNameOfProductService, FailedPropertyNameOfProductService>();
        services.AddScoped<ITransactionExecuteService, TransactionExecuteService>();

        services.AddScoped<IProductCharacteristicAndExternalXmlDataRelationService, ProductCharacteristicAndExternalXmlDataRelationService>();

        AddExternalXmlImportServices(services);

        return services;
    }

    public static IServiceCollection AddCachedProductServices(this IServiceCollection services, string connectionString)
    {
        services.AddDataAccess(connectionString)
            .AddAllRepositories();

        services.AddScoped<ProductMapper>();

        AddValidation(services);

        services.AddScoped<CategoryService>();
        services.AddScoped<ManifacturerService>();
        services.AddScoped<ProductImageService>();
        services.AddScoped<ProductImageFileNameInfoService>();
        services.AddScoped<ProductCharacteristicService>();
        services.AddScoped<ProductPropertyService>();
        services.AddScoped<ProductStatusesService>();
        services.AddScoped<IPromotionService, PromotionService>();

        services.AddScoped<ProductService>();

        //services.AddScoped<IProductManipulateService, ProductManipulateService>();

        services.AddScoped<ILocalChangesService, LocalChangesService>();

        services.AddScoped<IToDoLocalChangesService, ToDoLocalChangesService>();

        services.AddScoped<IExternalChangesService, ExternalChangesService>();
        services.AddScoped<IFailedPropertyNameOfProductService, FailedPropertyNameOfProductService>();
        services.AddScoped<ITransactionExecuteService, TransactionExecuteService>();

        services.AddScoped<IProductWorkStatusesService, ProductWorkStatusesService>();

        services.AddScoped<ICategoryService, CachedCategoryService>();
        services.AddScoped<IManifacturerService, CachedManifacturerService>();
        services.AddScoped<IProductImageFileNameInfoService, CachedProductImageFileNameInfoService>();
        services.AddScoped<IProductImageService, CachedProductImageService>();
        services.AddScoped<IProductCharacteristicService, CachedProductCharacteristicService>();
        services.AddScoped<IProductPropertyService, CachedProductPropertyService>();
        services.AddScoped<IProductService, CachedProductService>();
        services.AddScoped<IProductStatusesService, CachedProductStatusesService>();

        services.AddScoped<IProductCharacteristicAndExternalXmlDataRelationService, ProductCharacteristicAndExternalXmlDataRelationService>();

        AddExternalXmlImportServices(services);

        return services;
    }

    private static void AddExternalXmlImportServices(IServiceCollection services)
    {
        services.AddScoped<IXmlImportProductImageFileNameInfoService, XmlImportProductImageFileNameInfoService>();
        services.AddScoped<IXmlImportProductPropertyService, XmlImportProductPropertyService>();
        services.AddScoped<IXmlImportProductImageService, XmlImportProductImageService>();
    }

    private static void AddValidation(IServiceCollection services)
    {
        services.AddScoped<IValidator<ServiceCategoryCreateRequest>, CategoryCreateRequestValidator>();
        services.AddScoped<IValidator<ServiceCategoryUpdateRequest>, CategoryUpdateRequestValidator>();
        services.AddScoped<IValidator<ManifacturerCreateRequest>, ManifacturerCreateRequestValidator>();
        services.AddScoped<IValidator<ManifacturerUpdateRequest>, ManifacturerUpdateRequestValidator>();
        services.AddScoped<IValidator<ProductCreateRequest>, ProductCreateRequestValidator>();
        services.AddScoped<IValidator<ProductCreateWithoutImagesInDatabaseRequest>, ProductCreateWithoutImagesInDatabaseRequestValidator>();
        services.AddScoped<IValidator<ProductUpdateRequest>, ProductUpdateRequestValidator>();
        services.AddScoped<IValidator<ProductUpdateWithoutImagesInDatabaseRequest>, ProductUpdateWithoutImagesInDatabaseRequestValidator>();
        services.AddScoped<IValidator<ProductFullUpdateRequest>, ProductFullUpdateRequestValidator>();
        services.AddScoped<IValidator<ProductCharacteristicCreateRequest>, ProductCharacteristicCreateRequestValidator>();
        services.AddScoped<IValidator<ProductCharacteristicByIdUpdateRequest>, ProductCharacteristicByIdUpdateRequestValidator>();
        services.AddScoped<IValidator<ProductCharacteristicByNameAndCategoryIdUpdateRequest>, ProductCharacteristicByNameAndCategoryIdUpdateRequestValidator>();
        services.AddScoped<IValidator<ServiceProductImageCreateRequest>, ProductImageCreateRequestValidator>();
        services.AddScoped<IValidator<ServiceProductFirstImageCreateRequest>, ProductFirstImageCreateRequestValidator>();
        services.AddScoped<IValidator<ServiceProductImageUpdateRequest>, ProductImageUpdateRequestValidator>();
        services.AddScoped<IValidator<ServiceProductFirstImageUpdateRequest>, ProductFirstImageUpdateRequestValidator>();
        services.AddScoped<IValidator<ServiceProductImageFileNameInfoCreateRequest>, ProductImageFileNameInfoCreateRequestValidator>();
        services.AddScoped<IValidator<ServiceProductImageFileNameInfoByImageNumberUpdateRequest>, ProductImageFileNameInfoByImageNumberUpdateRequestValidator>();
        services.AddScoped<IValidator<ServiceProductImageFileNameInfoByFileNameUpdateRequest>, ProductImageFileNameInfoByFileNameUpdateRequestValidator>();
        services.AddScoped<IValidator<ProductPropertyByCharacteristicIdCreateRequest>, ProductPropertyByCharacteristicIdCreateRequestValidator>();
        services.AddScoped<IValidator<ProductPropertyByCharacteristicNameCreateRequest>, ProductPropertyByCharacteristicNameCreateRequestValidator>();
        services.AddScoped<IValidator<ProductPropertyUpdateRequest>, ProductPropertyUpdateRequestValidator>();
        services.AddScoped<IValidator<ServicePromotionCreateRequest>, PromotionCreateRequestValidator>();
        services.AddScoped<IValidator<ServicePromotionUpdateRequest>, PromotionUpdateRequestValidator>();
        services.AddScoped<IValidator<ProductStatusesCreateRequest>, ProductStatusesCreateRequestValidator>();
        services.AddScoped<IValidator<ProductStatusesUpdateRequest>, ProductStatusesUpdateRequestValidator>();

        services.AddScoped<IValidator<FailedPropertyNameOfProductCreateRequest>, FailedPropertyNameOfProductCreateRequestValidator>();
        services.AddScoped<IValidator<FailedPropertyNameOfProductMultiCreateRequest>, FailedPropertyNameOfProductMultiCreateRequestValidator>();
        services.AddScoped<IValidator<FailedPropertyNameOfProductUpdateRequest>, FailedPropertyNameOfProductUpdateRequestValidator>();

        services.AddScoped<IValidator<ProductWorkStatusesCreateRequest>, ProductWorkStatusesCreateRequestValidator>();
        services.AddScoped<IValidator<ProductWorkStatusesUpdateByIdRequest>, ProductWorkStatusesUpdateByIdRequestValidator>();
        services.AddScoped<IValidator<ProductWorkStatusesUpdateByProductIdRequest>, ProductWorkStatusesUpdateByProductIdRequestValidator>();

        AddValidationForExternalXmlImport(services);
    }

    private static void AddValidationForExternalXmlImport(IServiceCollection services)
    {
        services.AddScoped<IValidator<XmlImportServiceProductImageFileNameInfoCreateRequest>, XmlImportProductImageFileNameInfoCreateRequestValidator>();
        services.AddScoped<IValidator<XmlImportServiceProductImageFileNameInfoByImageNumberUpdateRequest>, XmlImportProductImageFileNameInfoByImageNumberUpdateRequestValidator>();
        services.AddScoped<IValidator<XmlImportServiceProductImageFileNameInfoByFileNameUpdateRequest>, XmlImportProductImageFileNameInfoByFileNameUpdateRequestValidator>();

        services.AddScoped<IValidator<XmlImportProductPropertyByCharacteristicIdCreateRequest>, XmlImportProductPropertyByCharacteristicIdCreateRequestValidator>();
        services.AddScoped<IValidator<XmlImportProductPropertyByCharacteristicNameCreateRequest>, XmlImportProductPropertyByCharacteristicNameCreateRequestValidator>();
        services.AddScoped<IValidator<XmlImportProductPropertyUpdateByXmlDataRequest>, XmlImportProductPropertyUpdateByXmlDataRequestValidator>();

        services.AddScoped<IValidator<XmlImportServiceProductImageCreateRequest>, XmlImportProductImageCreateRequestValidator>();
        services.AddScoped<IValidator<XmlImportServiceProductFirstImageCreateRequest>, XmlImportProductFirstImageCreateRequestValidator>();
        services.AddScoped<IValidator<XmlImportServiceProductImageUpdateRequest>, XmlImportProductImageUpdateRequestValidator>();
        services.AddScoped<IValidator<XmlImportServiceProductFirstImageUpdateRequest>, XmlImportProductFirstImageUpdateRequestValidator>();
    }
}