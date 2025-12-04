using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Services.Caching.Models;
using MOSTComputers.Services.Caching.Services.Contracts;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Contracts;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.ProductIdentifiers.ProductSerialNumber;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.ProductProperty;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.ProductStatuses;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.ProductWorkStatuses;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.Promotions.Groups;
using MOSTComputers.Services.ProductRegister.Mapping;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductIdentifiers.ProductGTINCode;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImage;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImage.FileRelated;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImage.FirstImage;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImageFileData;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductProperty;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductWorkStatuses;
using MOSTComputers.Services.ProductRegister.Models.Requests.PromotionFile;
using MOSTComputers.Services.ProductRegister.Models.Requests.PromotionProductFileInfo;
using MOSTComputers.Services.ProductRegister.Models.Requests.ToDoLocalChanges;

using MOSTComputers.Services.ProductRegister.Services;
using MOSTComputers.Services.ProductRegister.Services.Cached;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Services.ProductRegister.Services.ExternalXmlImport;
using MOSTComputers.Services.ProductRegister.Services.ExternalXmlImport.Contracts;
using MOSTComputers.Services.ProductRegister.Services.ProductHtml;
using MOSTComputers.Services.ProductRegister.Services.ProductHtml.Contracts;
using MOSTComputers.Services.ProductRegister.Services.ProductIdentifiers;
using MOSTComputers.Services.ProductRegister.Services.ProductIdentifiers.Contracts;
using MOSTComputers.Services.ProductRegister.Services.ProductImages;
using MOSTComputers.Services.ProductRegister.Services.ProductImages.Cached;
using MOSTComputers.Services.ProductRegister.Services.ProductImages.Contracts;
using MOSTComputers.Services.ProductRegister.Services.ProductProperties;
using MOSTComputers.Services.ProductRegister.Services.ProductProperties.Cached;
using MOSTComputers.Services.ProductRegister.Services.ProductProperties.Contacts;
using MOSTComputers.Services.ProductRegister.Services.ProductStatus;
using MOSTComputers.Services.ProductRegister.Services.ProductStatus.Cached;
using MOSTComputers.Services.ProductRegister.Services.ProductStatus.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Promotions;
using MOSTComputers.Services.ProductRegister.Services.Promotions.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Promotions.Groups;
using MOSTComputers.Services.ProductRegister.Services.Promotions.Groups.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Promotions.PromotionFiles;
using MOSTComputers.Services.ProductRegister.Services.Promotions.PromotionFiles.Cached;
using MOSTComputers.Services.ProductRegister.Services.Promotions.PromotionFiles.Contracts;
using MOSTComputers.Services.ProductRegister.Validation.ProductIdentifiers.ProductGTINCode;
using MOSTComputers.Services.ProductRegister.Validation.ProductIdentifiers.ProductSerialNumber;
using MOSTComputers.Services.ProductRegister.Validation.ProductImage;
using MOSTComputers.Services.ProductRegister.Validation.ProductImage.FileRelated;
using MOSTComputers.Services.ProductRegister.Validation.ProductImage.FirstImage;
using MOSTComputers.Services.ProductRegister.Validation.ProductImageFile;
using MOSTComputers.Services.ProductRegister.Validation.ProductProperty;
using MOSTComputers.Services.ProductRegister.Validation.ProductStatuses;
using MOSTComputers.Services.ProductRegister.Validation.ProductWorkStatuses;
using MOSTComputers.Services.ProductRegister.Validation.PromotionFileInfo;
using MOSTComputers.Services.ProductRegister.Validation.PromotionProductFileInfo;
using MOSTComputers.Services.ProductRegister.Validation.Promotions.Groups;
using ZiggyCreatures.Caching.Fusion;

namespace MOSTComputers.Services.ProductRegister.Configuration;
public static class ConfigureServices
{
    internal const string ProductServiceKey = "MOSTComputers.Services.ProductRegister.ProductServiceKey";
    internal const string ProductPropertyCrudServiceKey = "MOSTComputers.Services.ProductRegister.ProductPropertyServiceKey";
    internal const string ProductImageFileServiceKey = "MOSTComputers.Services.ProductRegister.ProductImageFileServiceKey";
    internal const string PromotionFileServiceKey = "MOSTComputers.Services.ProductRegister.PromotionFileServiceKey";
    internal const string PromotionProductFileServiceKey = "MOSTComputers.Services.ProductRegister.PromotionProductFileServiceKey";
    internal const string ProductWorkStatusesServiceKey = "MOSTComputers.Services.ProductRegister.ProductWorkStatusesServiceKey";

    public static IServiceCollection AddProductServices(this IServiceCollection services)
    {
        services.AddScoped<ProductMapper>();

        AddValidation(services);

        services.AddScoped<ITransactionExecuteService, TransactionExecuteService>();

        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ISubCategoryService, SubCategoryService>();

        services.AddScoped<IManufacturerService, ManufacturerService>();

        services.AddScoped<IProductImageCrudService, ProductImageCrudService>();
        services.AddScoped<IProductImageFileService, ProductImageFileService>();
        services.AddScoped<IProductImageAndFileService, ProductImageAndFileService>();
        services.AddScoped<IProductImageService, ProductImageService>();

        services.AddScoped<IProductCharacteristicService, ProductCharacteristicService>();

        services.AddScoped<IProductPropertyCrudService, ProductPropertyCrudService>();
        services.AddScoped<IProductPropertyService, ProductPropertyService>();

        services.AddScoped<IPromotionService, PromotionService>();

        services.AddScoped<IManufacturerToPromotionGroupRelationService, ManufacturerToPromotionGroupRelationService>();
        services.AddScoped<IGroupPromotionImageFileDataService, GroupPromotionImageFileDataService>();
        services.AddScoped<IGroupPromotionImageFileService, GroupPromotionImageFileService>();

        services.AddScoped<IPromotionFileService, PromotionFileService>();
        services.AddScoped<IPromotionProductFileInfoService, PromotionProductFileInfoService>();
        services.AddScoped<IPromotionProductFileService, PromotionProductFileService>();

        services.AddScoped<IProductImageAndPromotionFileSaveService, ProductImageAndPromotionFileSaveService>();

        services.AddScoped<IExchangeRateService, ExchangeRateService>();

        //services.AddScoped<IProductStatusesService, ProductStatusesService>();

        services.AddScoped<IProductWorkStatusesService, ProductWorkStatusesService>();
        services.AddScoped<IProductWorkStatusesWorkflowService, ProductWorkStatusesWorkflowService>();
        services.AddScoped<IProductWorkStatusesChangesUpsertService, ProductWorkStatusesChangesUpsertService>();

        services.AddKeyedScoped<IProductService, ProductService>(ProductServiceKey);
        services.AddScoped<IProductService, CachedProductService>();

        services.AddScoped<IProductToHtmlProductService, ProductToHtmlProductService>();

        services.AddScoped<IOriginalLocalChangesReadService, OriginalLocalChangesReadService>();

        services.AddScoped<IToDoLocalChangesService, ToDoLocalChangesService>();

        AddExternalXmlImportServices(services);

        return services;
    }

    private static void AddExternalXmlImportServices(IServiceCollection services)
    {
        services.AddScoped<IProductCharacteristicAndExternalXmlDataRelationService, ProductCharacteristicAndExternalXmlDataRelationService>();
    }

    public static IServiceCollection AddCachedProductServices(this IServiceCollection services)
    {
        services.AddScoped<ProductMapper>();

        AddValidation(services);

        services.AddScoped<ITransactionExecuteService, TransactionExecuteService>();

        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ISubCategoryService, SubCategoryService>();

        services.AddScoped<IManufacturerService, ManufacturerService>();

        services.AddKeyedScoped<IProductImageFileService, ProductImageFileService>(ProductImageFileServiceKey);
        services.AddScoped<IProductImageFileService, CachedProductImageFileService>(serviceProvider =>
        {
            return new(serviceProvider.GetRequiredKeyedService<IProductImageFileService>(ProductImageFileServiceKey),
                //serviceProvider.GetRequiredService<ICache<string>>(),
                serviceProvider.GetRequiredService<IFusionCache>());
        });

        services.AddScoped<IProductImageCrudService, ProductImageCrudService>();
        services.AddScoped<IProductImageAndFileService, ProductImageAndFileService>();
        services.AddScoped<IProductImageService, ProductImageService>();

        services.AddScoped<IProductCharacteristicService, ProductCharacteristicService>();

        services.AddKeyedScoped<IProductPropertyCrudService, ProductPropertyCrudService>(ProductPropertyCrudServiceKey);
        services.AddScoped<IProductPropertyCrudService, CachedProductPropertyCrudService>(serviceProvider =>
        {
            return new(serviceProvider.GetRequiredKeyedService<IProductPropertyCrudService>(ProductPropertyCrudServiceKey),
                serviceProvider.GetRequiredService<IProductRepository>(),
                //serviceProvider.GetRequiredService<ICache<string>>(),
                serviceProvider.GetRequiredService<IFusionCache>());
        });

        services.AddScoped<IProductPropertyService, ProductPropertyService>();

        services.AddScoped<IPromotionService, PromotionService>();

        services.AddScoped<IManufacturerToPromotionGroupRelationService, ManufacturerToPromotionGroupRelationService>();
        services.AddScoped<IGroupPromotionImageFileDataService, GroupPromotionImageFileDataService>();
        services.AddScoped<IGroupPromotionImageFileService, GroupPromotionImageFileService>();

        services.AddKeyedScoped<IPromotionFileService, PromotionFileService>(PromotionFileServiceKey);
        services.AddScoped<IPromotionFileService, CachedPromotionFileService>(serviceProvider =>
        {
            return new(serviceProvider.GetRequiredKeyedService<IPromotionFileService>(PromotionFileServiceKey),
                //serviceProvider.GetRequiredService<ICache<string>>(),
                serviceProvider.GetRequiredService<IFusionCache>());
        });

        services.AddKeyedScoped<IPromotionProductFileInfoService, PromotionProductFileInfoService>(PromotionProductFileServiceKey);
        services.AddScoped<IPromotionProductFileInfoService, CachedPromotionProductFileInfoService>(serviceProvider =>
        {
            return new(serviceProvider.GetRequiredKeyedService<IPromotionProductFileInfoService>(PromotionProductFileServiceKey),
                //serviceProvider.GetRequiredService<ICache<string>>(),
                serviceProvider.GetRequiredService<IFusionCache>());
        });

        services.AddScoped<IPromotionProductFileService, PromotionProductFileService>();

        services.AddScoped<IProductImageAndPromotionFileSaveService, ProductImageAndPromotionFileSaveService>();

        services.AddScoped<IExchangeRateService, ExchangeRateService>();

        //services.AddScoped<IProductStatusesService, ProductStatusesService>();

        services.AddKeyedScoped<IProductWorkStatusesService, ProductWorkStatusesService>(ProductWorkStatusesServiceKey);
        services.AddScoped<IProductWorkStatusesService, CachedProductWorkStatusesService>(serviceProvider =>
        {
            return new(serviceProvider.GetRequiredKeyedService<IProductWorkStatusesService>(ProductWorkStatusesServiceKey),
                //serviceProvider.GetRequiredService<IProductRepository>(),
                //serviceProvider.GetRequiredService<ICache<string>>()
                serviceProvider.GetRequiredService<IFusionCache>());
        });

        services.AddScoped<IProductWorkStatusesWorkflowService, ProductWorkStatusesWorkflowService>();
        services.AddScoped<IProductWorkStatusesChangesUpsertService, ProductWorkStatusesChangesUpsertService>();

        services.AddScoped<IProductGTINCodeService, ProductGTINCodeService>();
        services.AddScoped<IProductSerialNumberService, ProductSerialNumberService>();

        services.AddKeyedScoped<IProductService, ProductService>(ProductServiceKey);
        services.AddScoped<IProductService, CachedProductService>();

        services.AddScoped<IProductToHtmlProductService, ProductToHtmlProductService>();

        services.AddScoped<IOriginalLocalChangesReadService, OriginalLocalChangesReadService>();

        services.AddScoped<IToDoLocalChangesService, ToDoLocalChangesService>();

        AddExternalXmlImportServices(services);

        return services;
    }

    private static void AddValidation(IServiceCollection services)
    {
        services.AddScoped<IValidator<ServiceProductImageCreateRequest>, ProductImageCreateRequestValidator>();
        services.AddScoped<IValidator<ServiceProductImageUpdateRequest>, ProductImageUpdateRequestValidator>();
        services.AddScoped<IValidator<ProductImageUpsertRequest>, ProductImageUpsertRequestValidator>();
        services.AddScoped<IValidator<ProductImageForProductUpsertRequest>, ProductImageForProductUpsertRequestValidator>();

        services.AddScoped<IValidator<ProductImageWithFileCreateRequest>, ProductImageWithFileCreateRequestValidator>();
        services.AddScoped<IValidator<ProductImageWithFileUpdateRequest>, ProductImageWithFileUpdateRequestValidator>();
        services.AddScoped<IValidator<ProductImageWithFileUpsertRequest>, ProductImageWithFileUpsertRequestValidator>();
        services.AddScoped<IValidator<ProductImageWithFileForProductUpsertRequest>, ProductImageWithFileForProductUpsertRequestValidator>();

        services.AddScoped<IValidator<ServiceProductFirstImageCreateRequest>, ProductFirstImageCreateRequestValidator>();
        services.AddScoped<IValidator<ServiceProductFirstImageUpdateRequest>, ProductFirstImageUpdateRequestValidator>();
        services.AddScoped<IValidator<ServiceProductFirstImageUpsertRequest>, ProductFirstImageUpsertRequestValidator>();

        services.AddScoped<IValidator<ProductImageFileCreateRequest>, ProductImageFileCreateRequestValidator>();
        services.AddScoped<IValidator<ProductImageFileUpdateRequest>, ProductImageFileRequestValidator>();
        services.AddScoped<IValidator<ProductImageFileChangeRequest>, ProductImageFileUpdateRequestValidator>();
        services.AddScoped<IValidator<ProductImageFileRenameRequest>, ProductImageFileRenameRequestValidator>();

        services.AddScoped<IValidator<ServiceProductPropertyByCharacteristicIdCreateRequest>, ProductPropertyByCharacteristicIdCreateRequestValidator>();
        services.AddScoped<IValidator<ServiceProductPropertyByCharacteristicNameCreateRequest>, ProductPropertyByCharacteristicNameCreateRequestValidator>();
        services.AddScoped<IValidator<ProductPropertyUpdateRequest>, ProductPropertyUpdateRequestValidator>();

        services.AddScoped<IValidator<ProductStatusesCreateRequest>, ProductStatusesCreateRequestValidator>();
        services.AddScoped<IValidator<ProductStatusesUpdateRequest>, ProductStatusesUpdateRequestValidator>();

        services.AddScoped<IValidator<ManufacturerToPromotionGroupRelationUpsertRequest>, ManufacturerToPromotionGroupRelationUpsertRequestValidator>();

        services.AddScoped<IValidator<GroupPromotionImageFileDataCreateRequest>, GroupPromotionImageFileDataCreateRequestValidator>();
        services.AddScoped<IValidator<GroupPromotionImageFileDataUpdateRequest>, GroupPromotionImageFileDataUpdateRequestValidator>();

        services.AddScoped<IValidator<CreatePromotionFileRequest>, CreatePromotionFileRequestValidator>();
        services.AddScoped<IValidator<UpdatePromotionFileRequest>, UpdatePromotionFileRequestValidator>();

        services.AddScoped<IValidator<ServicePromotionProductFileCreateRequest>, ServicePromotionProductFileInfoCreateRequestValidator>();
        services.AddScoped<IValidator<ServicePromotionProductFileUpdateRequest>, ServicePromotionProductFileInfoUpdateRequestValidator>();

        services.AddScoped<IValidator<ServiceProductWorkStatusesCreateRequest>, ProductWorkStatusesCreateRequestValidator>();
        services.AddScoped<IValidator<ServiceProductWorkStatusesUpdateByIdRequest>, ProductWorkStatusesUpdateByIdRequestValidator>();
        services.AddScoped<IValidator<ServiceProductWorkStatusesUpdateByProductIdRequest>, ProductWorkStatusesUpdateByProductIdRequestValidator>();
        services.AddScoped<IValidator<ServiceProductWorkStatusesUpsertRequest>, ProductWorkStatusesUpsertRequestValidator>();

        services.AddScoped<IValidator<ServiceProductGTINCodeCreateRequest>, ServiceProductGTINCodeCreateRequestValidator>();
        services.AddScoped<IValidator<ServiceProductGTINCodeUpdateRequest>, ServiceProductGTINCodeUpdateRequestValidator>();
        services.AddScoped<IValidator<ServiceProductGTINCodeUpsertRequest>, ServiceProductGTINCodeUpsertRequestValidator>();

        services.AddScoped<IValidator<ProductSerialNumberCreateRequest>, ProductSerialNumberCreateRequestValidator>();

        AddValidationForExternalXmlImport(services);
    }

    private static void AddValidationForExternalXmlImport(IServiceCollection _)
    {
    }
}