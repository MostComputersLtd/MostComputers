using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.Caching.Configuration;
using MOSTComputers.Services.Caching.Services.Contracts;
using MOSTComputers.Services.Currencies;
using MOSTComputers.Services.Currencies.Contracts;
using MOSTComputers.Services.DataAccess.Documents.Configuration;
using MOSTComputers.Services.DataAccess.Products.Configuration;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Configuration;
using MOSTComputers.Services.Identity.Confuguration;
using MOSTComputers.Services.Identity.Models;
using MOSTComputers.Services.PDF.Configuration;
using MOSTComputers.Services.ProductImageFileManagement.Configuration;
using MOSTComputers.Services.ProductRegister.Configuration;
using MOSTComputers.Services.PromotionFileManagement.Configuration;
using MOSTComputers.Services.SearchStringOrigin.Configuration;
using MOSTComputers.Services.TransactionalFileManagement.Services;
using MOSTComputers.Services.TransactionalFileManagement.Services.Contracts;
using MOSTComputers.UI.Web.Blazor.Client.Localization;
using MOSTComputers.UI.Web.Blazor.Components;
using MOSTComputers.UI.Web.Blazor.Components._Tests;
using MOSTComputers.UI.Web.Blazor.Components.Account;
using MOSTComputers.UI.Web.Blazor.Endpoints.Documents;
using MOSTComputers.UI.Web.Blazor.Endpoints.Images;
using MOSTComputers.UI.Web.Blazor.Endpoints.Xml;
using MOSTComputers.UI.Web.Blazor.Logging;
using MOSTComputers.UI.Web.Blazor.Models.Configuration;
using MOSTComputers.UI.Web.Blazor.Services;
using MOSTComputers.UI.Web.Blazor.Services.ExternalXmlImport;
using MOSTComputers.UI.Web.Blazor.Services.ExternalXmlImport.Contracts;
using MOSTComputers.UI.Web.Blazor.Services.ProductEditor;
using MOSTComputers.UI.Web.Blazor.Services.ProductEditor.Contracts;
using MOSTComputers.UI.Web.Blazor.Services.Search;
using MOSTComputers.UI.Web.Blazor.Services.Search.Contracts;
using MOSTComputers.UI.Web.Blazor.Services.Xml;
using MOSTComputers.UI.Web.Blazor.Services.Xml.Cached;
using MOSTComputers.UI.Web.Blazor.Services.Xml.Contracts;
using Serilog;
using Serilog.Core;
using Serilog.Debugging;
using Serilog.Extensions.Logging;
using Serilog.Sinks.MSSqlServer;
using System.Collections.ObjectModel;
using System.Data;
using System.Net;
using System.Threading.RateLimiting;
using ZiggyCreatures.Caching.Fusion;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

const string productDBConnectionStringName = "MostDBNew";
const string readDBConnectionStringName = "MOST4WebDB";

string productDBConnectionString = builder.Configuration.GetConnectionString(productDBConnectionStringName)!;
string readDBConnectionString = builder.Configuration.GetConnectionString(readDBConnectionStringName)!;

const string errorLogsTableName = "ErrorLogs";

ColumnOptions columnOptions = new();

columnOptions.Store.Remove(StandardColumn.Properties);
columnOptions.Store.Remove(StandardColumn.MessageTemplate);
columnOptions.Store.Remove(StandardColumn.SpanId);
columnOptions.Store.Remove(StandardColumn.TraceId);

columnOptions.AdditionalColumns = new Collection<SqlColumn>
{
    new() { ColumnName = "ExceptionType", DataType = SqlDbType.NVarChar, DataLength = 256 },
    new() { ColumnName = "UserName", DataType = SqlDbType.NVarChar, DataLength = 256 },
    new() { ColumnName = "RequestId", DataType = SqlDbType.NVarChar, DataLength = 64 },
    new() { ColumnName = "Path", DataType = SqlDbType.NVarChar, DataLength = 256 },
    new() { ColumnName = "ApplicationName", DataType = SqlDbType.NVarChar, DataLength = 256 },
    new() { ColumnName = "MachineName", DataType = SqlDbType.NVarChar, DataLength = 64 },
    new() { ColumnName = "LoggerName", DataType = SqlDbType.NVarChar, DataLength = 256 },
    new() { ColumnName = "ConnectionId", DataType = SqlDbType.NVarChar, DataLength = 64 },
};

string? projectName = System.Reflection.Assembly.GetEntryAssembly()?.GetName().Name;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Error()
    .Filter.ByExcluding(
        logEvent => logEvent.Exception is JSDisconnectedException
            || logEvent.Exception is OperationCanceledException
            || logEvent.Exception is TaskCanceledException)
    .Enrich.FromLogContext()
    .Enrich.With(new LoggerNameEnricher("LoggerName"), new ExceptionTypeEnricher("ExceptionType"))
    .Enrich.WithProperty("ApplicationName", projectName ?? "MOSTComputers.UI.Web.Blazor")
    .Enrich.WithProperty("MachineName", Environment.MachineName)
    .WriteTo.MSSqlServer(
        connectionString: productDBConnectionString,
        sinkOptions: new MSSqlServerSinkOptions
        {
            TableName = errorLogsTableName,
            AutoCreateSqlTable = false,
            EnlistInTransaction = false,
        },
        columnOptions: columnOptions)
    .CreateLogger();

builder.Host.UseSerilog();

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
builder.Services.AddGroupPromotionXmlServices();
builder.Services.AddInvoiceXmlServices();
builder.Services.AddWarrantyCardXmlServices();

builder.Services.AddLegacyProductHtmlService();

builder.Services.AddNewProductHtmlService(productXslTemplateFilePath!);

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

string? promotionFileFolderFilePath = builder.Configuration.GetRequiredSection("Directories").GetValue<string>("PromotionFileDirectory");

if (!Path.IsPathFullyQualified(promotionFileFolderFilePath!))
{
    promotionFileFolderFilePath = Path.GetFullPath(promotionFileFolderFilePath!, builder.Environment.ContentRootPath);
}

builder.Services.AddPromotionFileManager(promotionFileFolderFilePath!);

string? groupPromotionFileFolderFilePath = builder.Configuration.GetRequiredSection("Directories").GetValue<string>("GroupPromotionFileDirectory");

if (!Path.IsPathFullyQualified(groupPromotionFileFolderFilePath!))
{
    groupPromotionFileFolderFilePath = Path.GetFullPath(groupPromotionFileFolderFilePath!, builder.Environment.ContentRootPath);
}

builder.Services.AddGroupPromotionFileManager(groupPromotionFileFolderFilePath!);

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

builder.Services.AddScoped<IProductSearchService, ProductSearchService>();

IConfigurationSection legacyPricelistOptionsConfigurationSection = builder.Configuration
    .GetRequiredSection("ExternalDataPaths")
    .GetRequiredSection("LegacyPricelistSite");

builder.Services.Configure<LegacyPricelistSiteOptions>(legacyPricelistOptionsConfigurationSection);

builder.Services.TryAddSingleton<IProductXmlProvidingService, ProductXmlProvidingService>();

builder.Services.TryAddKeyedScoped<IProductToXmlService, ProductToXmlService>("MOSTComputers.UI.Web.Blazor.ProductToXmlService");
builder.Services.AddScoped<IProductToXmlService, CachedProductToXmlService>(context =>
{
    IProductToXmlService innerService = context.GetRequiredKeyedService<IProductToXmlService>("MOSTComputers.UI.Web.Blazor.ProductToXmlService");
    //ICache<string> cache = context.GetRequiredService<ICache<string>>();
    IFusionCache fusionCache = context.GetRequiredService<IFusionCache>();

    return new CachedProductToXmlService(
        innerService,
        //cache,
        fusionCache);
});

builder.Services.TryAddScoped<IInvoiceToXmlService, InvoiceToXmlService>();
builder.Services.TryAddScoped<IWarrantyCardToXmlService, WarrantyCardToXmlService>();

builder.Services.TryAddScoped<IProductEditorDataService, ProductEditorDataService>();

builder.Services.AddScoped<ICurrencyConversionService, CurrencyConversionService>();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddCircuitOptions(options =>
    {
        options.DetailedErrors = true;
    })
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
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

    options.LoginPath = "/account/login";
});

builder.Services.AddCustomIdentityWithPasswordsTableOnly(productDBConnectionString)
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<PasswordsTableOnlyUser>, IdentityNoOpEmailSender>();

//builder.Services.Configure<ForwardedHeadersOptions>(options =>
//{
//    options.ForwardedHeaders =
//        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

//    options.KnownNetworks.Clear();
//    options.KnownProxies.Clear();

//    options.KnownProxies.Add(IPAddress.Parse("192.168.4.1"));
//});

//builder.Services.AddRateLimiter(options =>
//{
//    options.AddPolicy("getFullProductXmlPolicy", httpContext =>
//    {
//        string remoteIpAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

//        return RateLimitPartition.GetFixedWindowLimiter(
//            partitionKey: remoteIpAddress,
//            factory: partition => new FixedWindowRateLimiterOptions
//            {
//                PermitLimit = 20,
//                Window = TimeSpan.FromMinutes(1),
//                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
//                QueueLimit = 0,
//                AutoReplenishment = true,
//            });
//    });
//});

builder.Services.AddLocalization();

// TEMPORARY
builder.Services.AddSingleton<_Temp_TransferDataTests.SaveService>();

builder.Services.AddScoped<ValidationMessagesLocalizer>();

WebApplication app = builder.Build();

app.UseSerilogRequestLogging(options =>
{
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("UserName", httpContext.User?.Identity?.Name);
        diagnosticContext.Set("RequestId", httpContext.TraceIdentifier);
        diagnosticContext.Set("Path", httpContext.Request.Path);
    };
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseForwardedHeaders();

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseRequestLocalization(options => 
{
    string defaultCulture = "en-US";

    string[] supportedCultures = [defaultCulture];

    options.SetDefaultCulture(defaultCulture);
    options.AddSupportedCultures(supportedCultures);
    options.AddSupportedUICultures(supportedCultures);
});

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(MOSTComputers.UI.Web.Blazor.Client._Imports).Assembly);

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.MapProductXmlEndpoints();
app.MapPromotionGroupXmlEndpoints();
app.MapInvoiceXmlEndpoints();
app.MapWarrantyCardXmlEndpoints();

app.MapPdfInvoiceDataEndpoints();
app.MapPdfWarrantyCardDataWithoutPricesEndpoints();

app.MapProductImageDataEndpoints();
app.MapProductImageFileDataEndpoints();
app.MapGroupPromotionImageFileDataEndpoints();
app.MapPromotionFileDataEndpoints();

//app.MapGet("/debug/ip", (HttpContext context) =>
//{
//    var remoteIp = context.Connection.RemoteIpAddress?.ToString();
//    var forwardedFor = context.Request.Headers["X-Forwarded-For"].ToString();
//    var forwardedProto = context.Request.Headers["X-Forwarded-Proto"].ToString();

//    return new
//    {
//        RemoteIpAddress = remoteIp,
//        XForwardedFor = forwardedFor,
//        XForwardedProto = forwardedProto
//    };
//});
//{"remoteIpAddress":"192.168.4.1","xForwardedFor":"","xForwardedProto":""}
//{"remoteIpAddress":"192.168.4.1","xForwardedFor":"","xForwardedProto":""}

app.Run();