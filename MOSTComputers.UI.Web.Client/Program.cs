using MOSTComputers.Services.Caching.Configuration;
using MOSTComputers.Services.Currencies;
using MOSTComputers.Services.Currencies.Contracts;
using MOSTComputers.Services.DataAccess.Products.Configuration;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Configuration;
using MOSTComputers.Services.ProductImageFileManagement.Configuration;
using MOSTComputers.Services.ProductRegister.Configuration;
using MOSTComputers.Services.ProductRegister.Services;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Services.ProductRegister.Services.ProductProperties;
using MOSTComputers.Services.ProductRegister.Services.ProductProperties.Contacts;
using MOSTComputers.Services.ProductRegister.Services.Products;
using MOSTComputers.Services.ProductRegister.Services.Products.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Promotions;
using MOSTComputers.Services.ProductRegister.Services.Promotions.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Promotions.Groups;
using MOSTComputers.Services.ProductRegister.Services.Promotions.Groups.Contracts;
using MOSTComputers.Services.PromotionFileManagement.Configuration;
using MOSTComputers.Services.SearchStringOrigin.Configuration;
using MOSTComputers.Services.TransactionalFileManagement.Services;
using MOSTComputers.Services.TransactionalFileManagement.Services.Contracts;
using MOSTComputers.UI.Web.Client.Endpoints.Images;
using MOSTComputers.UI.Web.Client.Endpoints.Xml;
using MOSTComputers.UI.Web.Client.Services;
using MOSTComputers.UI.Web.Client.Services.Xml;
using MOSTComputers.UI.Web.Client.Services.Xml.Contracts;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

const string localDBConnectionStringName = "MostDBNew";
const string most4WebDBConnectionStringName = "MOST4WebDB";

string productDBConnectionString = builder.Configuration.GetConnectionString(localDBConnectionStringName)!;
string most4WebDBConnectionString = builder.Configuration.GetConnectionString(most4WebDBConnectionStringName)!;

//builder.Services.AddHttpClient();

builder.Services.AddFusionCachingServices();

builder.Services.AddDataAccess(productDBConnectionString, most4WebDBConnectionString)
    .AddAllRepositories();

//builder.Services.AddProductServices();

builder.Services.AddCachedProductService();
builder.Services.AddCachedProductWorkStatusesService();
builder.Services.AddCachedCategoryService();
builder.Services.AddCachedManufacturerService();
builder.Services.AddCachedPromotionService();

builder.Services.AddScoped<ISubCategoryService, SubCategoryService>();
builder.Services.AddScoped<IProductCharacteristicService, ProductCharacteristicService>();
builder.Services.AddScoped<IProductPropertyCrudService, ProductPropertyCrudService>();

builder.Services.AddScoped<IExchangeRateService, ExchangeRateService>();
builder.Services.AddScoped<IManufacturerToPromotionGroupRelationService, ManufacturerToPromotionGroupRelationService>();

builder.Services.AddScoped<IOriginalLocalChangesReadService, OriginalLocalChangesReadService>();
builder.Services.AddScoped<IProductSearchService, ProductSearchService>();
builder.Services.AddScoped<IGroupPromotionImageFileService, GroupPromotionImageFileService>();
builder.Services.AddScoped<IGroupPromotionImageFileDataService, GroupPromotionImageFileDataService>();
builder.Services.AddScoped<ITransactionExecuteService, TransactionExecuteService>();

builder.Services.AddScoped<IProductToXmlService, ProductToXmlService>();

builder.Services.AddNewProductXmlServices();

builder.Services.AddSearchStringOriginService();

builder.Services.AddSingleton<IEnlistmentManager, EnlistmentManager>();

builder.Services.AddScoped<ITransactionalFileManager, TransactionalFileManager>(serviceProvider =>
{
    return new(serviceProvider.GetRequiredService<IEnlistmentManager>());
});

string? productImageFolderFilePath = builder.Configuration.GetRequiredSection("Directories").GetValue<string>("ProductImageDirectory");

if (!Path.IsPathFullyQualified(productImageFolderFilePath!))
{
    productImageFolderFilePath = Path.GetFullPath(productImageFolderFilePath!, builder.Environment.ContentRootPath);
}

builder.Services.AddProductImageFileManager(productImageFolderFilePath!);

string? groupPromotionFileFolderFilePath = builder.Configuration.GetRequiredSection("Directories").GetValue<string>("GroupPromotionFileDirectory");

if (!Path.IsPathFullyQualified(groupPromotionFileFolderFilePath!))
{
    groupPromotionFileFolderFilePath = Path.GetFullPath(groupPromotionFileFolderFilePath!, builder.Environment.ContentRootPath);
}

builder.Services.AddGroupPromotionFileManager(groupPromotionFileFolderFilePath!);

//string currentDirectory = AppDomain.CurrentDomain.BaseDirectory.Replace('\\', '/');

//string? productXslTemplateFilePath = builder.Configuration.GetRequiredSection("Files").GetValue<string>("ProductXslTemplate");

//if (!Path.IsPathFullyQualified(productXslTemplateFilePath!))
//{
//    productXslTemplateFilePath = Path.Combine(currentDirectory, productXslTemplateFilePath!);
//}

builder.Services.AddScoped<ICurrencyVATPercentageProvider, CurrencyVATPercentageProvider>();
builder.Services.AddScoped<ICurrencyVATService, CurrencyVATService>();
builder.Services.AddScoped<ICurrencyConversionService, CurrencyConversionService>();

builder.Services.AddScoped<ActivePromotionGroupsService>();

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.MapProductXmlEndpoints();
app.MapProductImageFileDataEndpoints();
app.MapGroupPromotionImageFileDataEndpoints();

app.Run();