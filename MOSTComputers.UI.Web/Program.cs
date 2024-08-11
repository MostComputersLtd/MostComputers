using MOSTComputers.Services.ProductRegister.Configuration;
using MOSTComputers.Services.SearchStringOrigin.Configuration;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Configuration;
using MOSTComputers.UI.Web.Services;
using MOSTComputers.UI.Web.Services.Contracts;
using MOSTComputers.Services.Caching.Configuration;
using MOSTComputers.Services.Identity.Confuguration;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using MOSTComputers.UI.Web.Authentication;
using FluentValidation;
using MOSTComputers.UI.Web.Validation.Authentication;
using MOSTComputers.UI.Web.Models.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddMemoryCachingServices();

builder.Services.AddCachedProductServices(builder.Configuration.GetConnectionString("MostDBNew")!);

builder.Services.AddHttpClient();

builder.Services.AddXmlDeserializer();

builder.Services.AddScoped<IProductXmlToProductMappingService, ProductXmlToProductMappingService>();
builder.Services.AddScoped<IProductXmlToCreateRequestMappingService, ProductXmlToCreateRequestMappingService>();
builder.Services.AddScoped<IProductXmlToProductDisplayMappingService, ProductXmlToProductDisplayMappingService>();

builder.Services.AddSearchStringOriginService();

builder.Services.AddScoped<IValidator<SignInRequest>, SignInRequestValidator>();
builder.Services.AddScoped<IValidator<LogInRequest>, LogInRequestValidator>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ApplicationScheme;
}).AddIdentityCookies();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 5;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Accounts/Login";
});

builder.Services.AddCustomIdentity(builder.Configuration.GetConnectionString("MOSTComputers.Services.Authentication")!)
    .AddSignInManager();

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

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