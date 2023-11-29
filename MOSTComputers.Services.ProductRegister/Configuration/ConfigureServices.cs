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
using MOSTComputers.Models.Product.Models.Requests.Promotion;
using MOSTComputers.Services.ProductRegister.Mapping;
using MOSTComputers.Services.ProductRegister.Models.Requests.Category;
using MOSTComputers.Services.ProductRegister.Services;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Services.ProductRegister.Validation.Category;
using MOSTComputers.Services.ProductRegister.Validation.FailureData.FailedPropertyNameOfProduct;
using MOSTComputers.Services.ProductRegister.Validation.Manifacturer;
using MOSTComputers.Services.ProductRegister.Validation.Product;
using MOSTComputers.Services.ProductRegister.Validation.ProductCharacteristic;
using MOSTComputers.Services.ProductRegister.Validation.ProductImage;
using MOSTComputers.Services.ProductRegister.Validation.ProductImageFileNameInfo;
using MOSTComputers.Services.ProductRegister.Validation.ProductProperty;
using MOSTComputers.Services.ProductRegister.Validation.Promotion;
using static MOSTComputers.Services.DAL.Configuration.ConfigureServices;

namespace MOSTComputers.Services.ProductRegister.Configuration;

public static class ConfigureServices
{
    public static IServiceCollection AddProductServices(this IServiceCollection services, string connectionString)
    {
        services.AddDataAccess(connectionString)
            .AddAllRepositories();

        services.AddTransient<ProductMapper>();

        AddValidation(services);

        services.AddTransient<ICategoryService, CategoryService>();
        services.AddTransient<IManifacturerService, ManifacturerService>();
        services.AddTransient<IProductImageService, ProductImageService>();
        services.AddTransient<IProductImageFileNameInfoService, ProductImageFileNameInfoService>();
        services.AddTransient<IProductCharacteristicService, ProductCharacteristicService>();
        services.AddTransient<IProductPropertyService, ProductPropertyService>();
        services.AddTransient<IPromotionService, PromotionService>();
        services.AddTransient<IProductService, ProductService>();

        services.AddTransient<IFailedPropertyNameOfProductService, FailedPropertyNameOfProductService>();

        return services;
    }

    private static void AddValidation(IServiceCollection services)
    {
        services.AddTransient<IValidator<ServiceCategoryCreateRequest>, CategoryCreateRequestValidator>();
        services.AddTransient<IValidator<ServiceCategoryUpdateRequest>, CategoryUpdateRequestValidator>();
        services.AddTransient<IValidator<ManifacturerCreateRequest>, ManifacturerCreateRequestValidator>();
        services.AddTransient<IValidator<ManifacturerUpdateRequest>, ManifacturerUpdateRequestValidator>();
        services.AddTransient<IValidator<ProductCreateRequest>, ProductCreateRequestValidator>();
        services.AddTransient<IValidator<ProductUpdateRequest>, ProductUpdateRequestValidator>();
        services.AddTransient<IValidator<ProductCharacteristicCreateRequest>, ProductCharacteristicCreateRequestValidator>();
        services.AddTransient<IValidator<ProductCharacteristicByIdUpdateRequest>, ProductCharacteristicByIdUpdateRequestValidator>();
        services.AddTransient<IValidator<ProductCharacteristicByNameAndCategoryIdUpdateRequest>, ProductCharacteristicByNameAndCategoryIdUpdateRequestValidator>();
        services.AddTransient<IValidator<ServiceProductImageCreateRequest>, ProductImageCreateRequestValidator>();
        services.AddTransient<IValidator<ServiceProductFirstImageCreateRequest>, ProductFirstImageCreateRequestValidator>();
        services.AddTransient<IValidator<ServiceProductImageUpdateRequest>, ProductImageUpdateRequestValidator>();
        services.AddTransient<IValidator<ServiceProductFirstImageUpdateRequest>, ProductFirstImageUpdateRequestValidator>();
        services.AddTransient<IValidator<ProductImageFileNameInfoCreateRequest>, ProductImageFileNameInfoCreateRequestValidator>();
        services.AddTransient<IValidator<ProductImageFileNameInfoUpdateRequest>, ProductImageFileNameInfoUpdateRequestValidator>();
        services.AddTransient<IValidator<ProductPropertyByCharacteristicIdCreateRequest>, ProductPropertyByCharacteristicIdCreateRequestValidator>();
        services.AddTransient<IValidator<ProductPropertyByCharacteristicNameCreateRequest>, ProductPropertyByCharacteristicNameCreateRequestValidator>();
        services.AddTransient<IValidator<ProductPropertyUpdateRequest>, ProductPropertyUpdateRequestValidator>();
        services.AddTransient<IValidator<ServicePromotionCreateRequest>, PromotionCreateRequestValidator>();
        services.AddTransient<IValidator<ServicePromotionUpdateRequest>, PromotionUpdateRequestValidator>();

        services.AddTransient<IValidator<FailedPropertyNameOfProductCreateRequest>, FailedPropertyNameOfProductCreateRequestValidator>();
        services.AddTransient<IValidator<FailedPropertyNameOfProductMultiCreateRequest>, FailedPropertyNameOfProductMultiCreateRequestValidator>();
        services.AddTransient<IValidator<FailedPropertyNameOfProductUpdateRequest>, FailedPropertyNameOfProductUpdateRequestValidator>();
    }
}