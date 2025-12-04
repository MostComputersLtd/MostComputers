using PuppeteerSharp;

namespace MOSTComputers.Services.PDF.Services.Contracts;
internal interface IBrowserProviderService
{
    Task<IBrowser> GetBrowserAsync();
}