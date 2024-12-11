using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MOSTComputers.Services.Identity.Models;
using MOSTComputers.Services.Caching.Configuration;
using MOSTComputers.Services.Identity.Confuguration;
using MOSTComputers.Services.ProductRegister.Configuration;
using MOSTComputers.Services.SearchStringOrigin.Configuration;
using MOSTComputers.Services.LocalChangesHandling.Configuration;
using MOSTComputers.Services.ProductImageFileManagement.Configuration;
using MOSTComputers.Services.PDF.Configuration;
using MOSTComputers.UI.Web.RealWorkTesting.Authentication;
using MOSTComputers.UI.Web.RealWorkTesting.Models.Authentication;
using MOSTComputers.UI.Web.RealWorkTesting.Services;
using MOSTComputers.UI.Web.RealWorkTesting.Services.Contracts;
using MOSTComputers.UI.Web.RealWorkTesting.Validation.Authentication;
using MOSTComputers.UI.Web.RealWorkTesting.Services.Contracts.ExternalXmlImport;
using MOSTComputers.UI.Web.RealWorkTesting.Services.ExternalXmlImport;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

const string productDBConnectionStringName = "MostDBNew";
//const string identityDBConnectionStringName = "MOSTComputers.Services.Authentication";

builder.Services.AddHttpClient();

builder.Services.AddMemoryCachingServices();

builder.Services.AddCachedProductServices(builder.Configuration.GetConnectionString(productDBConnectionStringName)!);

builder.Services.AddSearchStringOriginService();

string? productImageFolderFilePath = builder.Configuration.GetRequiredSection("Directories").GetValue<string>("ProductImageDirectory");

if (!Path.IsPathFullyQualified(productImageFolderFilePath!))
{
    productImageFolderFilePath = Path.GetFullPath(productImageFolderFilePath!, builder.Environment.ContentRootPath);
}

builder.Services.AddProductImageFileManagement(productImageFolderFilePath!);

string? pdfInvoiceTemplateFilePath = builder.Configuration.GetRequiredSection("Files").GetValue<string>("PdfInvoiceTemplate");

if (!Path.IsPathFullyQualified(pdfInvoiceTemplateFilePath!))
{
    pdfInvoiceTemplateFilePath = Path.GetFullPath(pdfInvoiceTemplateFilePath!, builder.Environment.ContentRootPath);
}

string? htmlInvoiceTemplateFilePath = builder.Configuration.GetRequiredSection("Files").GetValue<string>("HtmlInvoiceTemplate");

if (!Path.IsPathFullyQualified(htmlInvoiceTemplateFilePath!))
{
    htmlInvoiceTemplateFilePath = Path.GetFullPath(htmlInvoiceTemplateFilePath!, builder.Environment.ContentRootPath);
}

// builder.Services.AddPdfInvoiceServices(pdfInvoiceTemplateFilePath!);
builder.Services.AddHtmlInvoiceServices(htmlInvoiceTemplateFilePath!);

builder.Services.TryAddSingleton<IProductXmlProvidingService, ProductXmlProvidingService>();

builder.Services.AddSingleton<IProductTableDataService, ProductTableDataService>();

builder.Services.AddLocalChangesHandlingBackgroundService();

builder.Services.AddScoped<IValidator<SignInRequest>, SignInRequestValidator>();
builder.Services.AddScoped<IValidator<LogInRequest>, LogInRequestValidator>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ApplicationScheme;
}
).AddIdentityCookies();

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
    options.ExpireTimeSpan = TimeSpan.FromMinutes(90);
    options.SlidingExpiration = true;

    options.LoginPath = "/Accounts/Login";
});

//builder.Services.AddCustomIdentity(builder.Configuration.GetConnectionString(identityDBConnectionStringName)!)
builder.Services.AddCustomIdentityWithPasswordsTableOnly(builder.Configuration.GetConnectionString(productDBConnectionStringName)!)
    .AddSignInManager()
    .AddDefaultTokenProviders();

//builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IAuthenticationService<PasswordsTableOnlyUser>, PasswordsTableOnlyAuthenticationService>();

builder.Services.AddSingleton<IUserEditorStorageService, UserEditorStorageService>();

builder.Services.AddScoped<IXmlProductToProductMappingService, XmlProductToProductMappingService>();

builder.Services.AddScoped<IImageComparisonDataService, ImageComparisonDataService>();

builder.Services.AddScoped<IProductXmlDataSaveService, ProductXmlDataSaveService>();


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

app.MapRazorPages();

app.Run();