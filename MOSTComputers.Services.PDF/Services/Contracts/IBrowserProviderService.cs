using PuppeteerSharp;

namespace MOSTComputers.Services.PDF.Services.Contracts;
internal interface IBrowserProviderService: IDisposable, IAsyncDisposable
{
    Task<IBrowser> GetBrowserAsync();
}