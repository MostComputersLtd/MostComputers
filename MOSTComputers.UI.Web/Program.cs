using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MOSTComputers.Services.Caching.Configuration;
using MOSTComputers.Services.DataAccess.Documents.Configuration;
using MOSTComputers.Services.DataAccess.Products.Configuration;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Configuration;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Xml.Legacy.Contracts;
using MOSTComputers.Services.Identity.Confuguration;
using MOSTComputers.Services.Identity.Models;
using MOSTComputers.Services.PDF.Configuration;
using MOSTComputers.Services.ProductImageFileManagement.Configuration;
using MOSTComputers.Services.ProductRegister.Configuration;
using MOSTComputers.Services.PromotionFileManagement.Configuration;
using MOSTComputers.Services.SearchStringOrigin.Configuration;
using MOSTComputers.Services.TransactionalFileManagement.Services;
using MOSTComputers.Services.TransactionalFileManagement.Services.Contracts;
using MOSTComputers.UI.Web.Controllers;
using MOSTComputers.UI.Web.Models.Authentication;
using MOSTComputers.UI.Web.Models.Configuration;
using MOSTComputers.UI.Web.Services;
using MOSTComputers.UI.Web.Services.Authentication;
using MOSTComputers.UI.Web.Services.Contracts;
using MOSTComputers.UI.Web.Services.Data;
using MOSTComputers.UI.Web.Services.Data.Accounts;
using MOSTComputers.UI.Web.Services.Data.Accounts.Contracts;
using MOSTComputers.UI.Web.Services.Data.Contracts;
using MOSTComputers.UI.Web.Services.Data.PageDataStorage;
using MOSTComputers.UI.Web.Services.Data.PageDataStorage.Contracts;
using MOSTComputers.UI.Web.Services.Data.ProductEditor;
using MOSTComputers.UI.Web.Services.Data.ProductEditor.Contracts;
using MOSTComputers.UI.Web.Services.Data.Search;
using MOSTComputers.UI.Web.Services.Data.Search.Contracts;
using MOSTComputers.UI.Web.Services.Data.Xml;
using MOSTComputers.UI.Web.Services.Data.Xml.Contracts;
using MOSTComputers.UI.Web.Services.ExternalXmlImport;
using MOSTComputers.UI.Web.Services.ExternalXmlImport.Contracts;
using MOSTComputers.UI.Web.Validation.Authentication;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using System.Collections.ObjectModel;
using System.Data;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

const string productDBConnectionStringName = "MostDBNew";
const string readDBConnectionStringName = "MOST4WebDB";

string productDBConnectionString = builder.Configuration.GetConnectionString(productDBConnectionStringName)!;
string readDBConnectionString = builder.Configuration.GetConnectionString(readDBConnectionStringName)!;

builder.Services.AddHttpClient();
builder.Services.AddFusionCachingServices();

builder.Services.AddDataAccess(productDBConnectionString, readDBConnectionString)
    .AddAllRepositories();

//builder.Services.AddProductServices();

builder.Services.AddCachedProductServices();

builder.Services.AddDocumentRepositories(readDBConnectionString);

string currentDirectory = AppDomain.CurrentDomain.BaseDirectory.Replace('\\', '/');

string? productXslTemplateFilePath = builder.Configuration.GetRequiredSection("Files").GetValue<string>("ProductXslTemplate");

if (!Path.IsPathFullyQualified(productXslTemplateFilePath!))
{
    productXslTemplateFilePath = Path.Combine(currentDirectory, productXslTemplateFilePath!);
}

builder.Services.AddLegacyXmlServices();

builder.Services.AddNewProductXmlServices();

builder.Services.AddLegacyProductHtmlService();

builder.Services.AddNewProductHtmlService(productXslTemplateFilePath!);

builder.Services.AddSearchStringOriginService();

builder.Services.AddScoped<IEnlistmentManager, EnlistmentManager>();

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

string? promotionFileFolderFilePath = builder.Configuration.GetRequiredSection("Directories").GetValue<string>("PromotionFileDirectory");

if (!Path.IsPathFullyQualified(promotionFileFolderFilePath!))
{
    promotionFileFolderFilePath = Path.GetFullPath(promotionFileFolderFilePath!, builder.Environment.ContentRootPath);
}

builder.Services.AddPromotionFileManager(promotionFileFolderFilePath!);

string? htmlInvoiceTemplateFilePath = builder.Configuration.GetRequiredSection("Files").GetValue<string>("HtmlInvoiceTemplate");

if (!Path.IsPathFullyQualified(htmlInvoiceTemplateFilePath!))
{
    htmlInvoiceTemplateFilePath = Path.Combine(currentDirectory, htmlInvoiceTemplateFilePath!);
}

string? htmlWarrantyCardWithoutPricesTemplateFilePath = builder.Configuration.GetRequiredSection("Files").GetValue<string>("HtmlWarrantyCardWithoutPricesTemplate");

if (!Path.IsPathFullyQualified(htmlWarrantyCardWithoutPricesTemplateFilePath!))
{
    htmlWarrantyCardWithoutPricesTemplateFilePath = Path.Combine(currentDirectory, htmlWarrantyCardWithoutPricesTemplateFilePath!);
}

builder.Services.AddPdfInvoiceGeneratorFromDataServices(htmlInvoiceTemplateFilePath!);
builder.Services.AddPdfWarrantyCardWithoutPricesGeneratorFromDataServices(htmlWarrantyCardWithoutPricesTemplateFilePath!);

builder.Services.AddScoped<IPromotionFileEditorDataService, PromotionFileEditorDataService>();

builder.Services.AddScoped<IProductSearchService, ProductSearchService>();

builder.Services.AddScoped<IImageExtensionFromContentTypeService, ImageExtensionFromContentTypeService>();
builder.Services.AddScoped<IProductEditorProductDataService, ProductEditorProductDataService>();

IConfigurationSection legacyPricelistOptionsConfigurationSection = builder.Configuration
    .GetRequiredSection("ExternalDataPaths")
    .GetRequiredSection("LegacyPricelistSite");

builder.Services.Configure<LegacyPricelistSiteOptions>(legacyPricelistOptionsConfigurationSection);

builder.Services.TryAddSingleton<IProductXmlProvidingService, ProductXmlProvidingService>();

LegacyPricelistSiteOptions legacyPricelistSiteOptions = legacyPricelistOptionsConfigurationSection
    .Get<LegacyPricelistSiteOptions>()!;

builder.Services.AddScoped<IProductLegacyXmlReadService, ProductLegacyXmlReadService>(x =>
{
    return new(legacyPricelistSiteOptions.LegacyProductsXmlEndpointPath, x.GetRequiredService<IHttpClientFactory>());
});

builder.Services.AddScoped<ILegacyProductXmlFromProductDataService, LegacyProductXmlFromProductDataService>();
builder.Services.AddScoped<IProductXmlFromProductDataService, ProductXmlFromProductDataService>();

builder.Services.TryAddSingleton<IProductCharacteristicRelationsStorageService, ProductCharacteristicRelationsStorageService>();


builder.Services.AddScoped<IValidator<SignInRequest>, SignInRequestValidator>();
builder.Services.AddScoped<IValidator<LogInRequest>, LogInRequestValidator>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ApplicationScheme;
})
    .AddIdentityCookies();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 1;
    options.Password.RequiredUniqueChars = 1;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;

    options.Cookie.IsEssential = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.SlidingExpiration = true;

    options.LoginPath = "/Accounts/Login";
});

builder.Services.AddCustomIdentityWithPasswordsTableOnly(productDBConnectionString)
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<IAuthenticationService<PasswordsTableOnlyUser>, PasswordsTableOnlyAuthenticationService>();

builder.Services.AddScoped<IUserDisplayDataService, UserDisplayDataService>();

builder.Services.AddScoped<IPropertyEditorCharacteristicsService, PropertyEditorCharacteristicsService>();

builder.Services.AddScoped<IPartialViewRenderService, PartialViewRenderService>();

builder.Services.AddControllers();
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

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapRazorPages();

app.Run();