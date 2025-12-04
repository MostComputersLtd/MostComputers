using PuppeteerSharp;
using MOSTComputers.Services.PDF.Services.Contracts;

namespace MOSTComputers.Services.PDF.Services;
internal sealed class BrowserProviderService : IBrowserProviderService, IDisposable, IAsyncDisposable
{
    public BrowserProviderService(LaunchOptions launchOptions)
    {
        _launchOptions = launchOptions;
    }

    private IBrowser? _browser;

    private readonly SemaphoreSlim _semaphore = new(1, 1);

    private readonly LaunchOptions _launchOptions;

    private bool _disposed;

    public async Task<IBrowser> GetBrowserAsync()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (_browser is not null)
        {
            return _browser;
        }

        await _semaphore.WaitAsync();

        try
        {
            _browser ??= await LaunchBrowserAsync();

            return _browser;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async Task<IBrowser> LaunchBrowserAsync()
    {
        BrowserFetcher browserFetcher = new();

        await browserFetcher.DownloadAsync();

        return await Puppeteer.LaunchAsync(_launchOptions);
    }

    public void Dispose()
    {
        if (_disposed) return;

        if (_browser is not null)
        {
            _browser.Dispose();

            _browser = null;
        }

        _disposed = true;
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;

        if (_browser is not null)
        {
            await _browser.CloseAsync();
            await _browser.DisposeAsync();

            _browser = null;
        }

        _disposed = true;
    }
}