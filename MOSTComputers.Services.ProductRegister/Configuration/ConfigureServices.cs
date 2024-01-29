using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Models.Product.Models.FailureData;
using MOSTComputers.Models.Product.Models.FailureData.Requests.FailedPropertyNameOfProduct;
using MOSTComputers.Models.Product.Models.Requests.Category;
using MOSTComputers.Models.Product.Models.Requests.Manifacturer;
using MOSTComputers.Models.Product.Models.Requests.Product;
using MOSTComputers.Models.Product.Models.Requests.ProductCharacteristic;
using MOSTComputers.Models.Product.Models.Requests.ProductImage;
using MOSTComputers.Models.Product.Models.Requests.ProductImageFileNameInfo;
using MOSTComputers.Models.Product.Models.Requests.ProductProperty;
using MOSTComputers.Models.Product.Models.Requests.ProductStatuses;
using MOSTComputers.Models.Product.Models.Requests.Promotion;
using MOSTComputers.Services.ProductRegister.Mapping;
using MOSTComputers.Services.ProductRegister.Models.Requests.Category;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImageFileNameInfo;
using MOSTComputers.Services.ProductRegister.Services;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Services.ProductRegister.Services.UsingCache;
using MOSTComputers.Services.ProductRegister.Validation.Category;
using MOSTComputers.Services.ProductRegister.Validation.FailureData.FailedPropertyNameOfProduct;
using MOSTComputers.Services.ProductRegister.Validation.Manifacturer;
using MOSTComputers.Services.ProductRegister.Validation.Product;
using MOSTComputers.Services.ProductRegister.Validation.ProductCharacteristic;
using MOSTComputers.Services.ProductRegister.Validation.ProductImage;
using MOSTComputers.Services.ProductRegister.Validation.ProductImageFileNameInfo;
using MOSTComputers.Services.ProductRegister.Validation.ProductProperty;
using MOSTComputers.Services.ProductRegister.Validation.ProductStatuses;
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
        services.AddScoped<IProductImageService, ProductImageService>();
        services.AddScoped<IProductImageFileNameInfoService, ProductImageFileNameInfoService>();
        services.AddScoped<IProductCharacteristicService, ProductCharacteristicService>();
        services.AddScoped<IProductPropertyService, ProductPropertyService>();
        services.AddScoped<IPromotionService, PromotionService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IProductStatusesService, ProductStatusesService>();
        services.AddScoped<ILocalChangesService, LocalChangesService>();
        services.AddScoped<IExternalChangesService, ExternalChangesService>();

        services.AddScoped<IFailedPropertyNameOfProductService, FailedPropertyNameOfProductService>();
        services.AddScoped<ITransactionExecuteService, TransactionExecuteService>();

        return services;
    }

    private static void AddValidation(IServiceCollection services)
    {
        services.AddScoped<IValidator<ServiceCategoryCreateRequest>, CategoryCreateRequestValidator>();
        services.AddScoped<IValidator<ServiceCategoryUpdateRequest>, CategoryUpdateRequestValidator>();
        services.AddScoped<IValidator<ManifacturerCreateRequest>, ManifacturerCreateRequestValidator>();
        services.AddScoped<IValidator<ManifacturerUpdateRequest>, ManifacturerUpdateRequestValidator>();
        services.AddScoped<IValidator<ProductCreateRequest>, ProductCreateRequestValidator>();
        services.AddScoped<IValidator<ProductUpdateRequest>, ProductUpdateRequestValidator>();
        services.AddScoped<IValidator<ProductCharacteristicCreateRequest>, ProductCharacteristicCreateRequestValidator>();
        services.AddScoped<IValidator<ProductCharacteristicByIdUpdateRequest>, ProductCharacteristicByIdUpdateRequestValidator>();
        services.AddScoped<IValidator<ProductCharacteristicByNameAndCategoryIdUpdateRequest>, ProductCharacteristicByNameAndCategoryIdUpdateRequestValidator>();
        services.AddScoped<IValidator<ServiceProductImageCreateRequest>, ProductImageCreateRequestValidator>();
        services.AddScoped<IValidator<ServiceProductFirstImageCreateRequest>, ProductFirstImageCreateRequestValidator>();
        services.AddScoped<IValidator<ServiceProductImageUpdateRequest>, ProductImageUpdateRequestValidator>();
        services.AddScoped<IValidator<ServiceProductFirstImageUpdateRequest>, ProductFirstImageUpdateRequestValidator>();
        services.AddScoped<IValidator<ServiceProductImageFileNameInfoCreateRequest>, ProductImageFileNameInfoCreateRequestValidator>();
        services.AddScoped<IValidator<ServiceProductImageFileNameInfoUpdateRequest>, ProductImageFileNameInfoUpdateRequestValidator>();
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
    }
}