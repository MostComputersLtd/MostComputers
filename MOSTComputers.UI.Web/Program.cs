using MOSTComputers.Services.ProductRegister.Configuration;
using MOSTComputers.Services.SearchStringOrigin.Configuration;
using MOSTComputers.Services.XMLDataOperations.Configuration;
using MOSTComputers.UI.Web.Services;
using MOSTComputers.UI.Web.Services.Contracts;
using MOSTComputers.Services.Caching.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddMemoryCachingServices();

builder.Services.AddCachedProductServices(builder.Configuration.GetConnectionString("MostDBNew")!);

builder.Services.AddHttpClient();

builder.Services.AddXmlDeserializeService();

builder.Services.AddScoped<IProductXmlToProductMappingService, ProductXmlToProductMappingService>();
builder.Services.AddScoped<IProductXmlToCreateRequestMappingService, ProductXmlToCreateRequestMappingService>();
builder.Services.AddScoped<IProductXmlToProductDisplayMappingService, ProductXmlToProductDisplayMappingService>();

builder.Services.AddSearchStringOriginService();

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

app.Run();