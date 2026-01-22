using Microsoft.Extensions.Options;
using MOSTComputers.Services.PDF.Services.Contracts;
using PuppeteerSharp;

namespace MOSTComputers.Services.PDF.Services;

internal sealed class BrowserProviderServiceLaunchOptions
{
    public required LaunchOptions LaunchOptions { get; init; }
    public required string BrowserUrl { get; init; }
}

internal sealed class BrowserProviderService : IBrowserProviderService, IDisposable, IAsyncDisposable
{
    public BrowserProviderService(IOptions<BrowserProviderServiceLaunchOptions> options)
    {
        _options = options.Value;
    }

    private IBrowser? _browser;

    private readonly SemaphoreSlim _semaphore = new(1, 1);

    private readonly BrowserProviderServiceLaunchOptions _options;

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

        //return await Puppeteer.LaunchAsync(_options.LaunchOptions);

        try
        {
            return await Puppeteer.ConnectAsync(new()
            {
                BrowserURL = _options.BrowserUrl,
            });
        }
        catch (TargetClosedException)
        {
            return await Puppeteer.LaunchAsync(_options.LaunchOptions);
        }
        catch (ProcessException)
        {
            return await Puppeteer.LaunchAsync(_options.LaunchOptions);
        }
    }

    public void Dispose()
    {
        if (_disposed) return;

        Console.WriteLine("Disposing BrowserProviderService...");

        _browser?.Dispose();

        _browser = null;

        _disposed = true;
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;

        Console.WriteLine("ASYNC Disposing BrowserProviderService...");

        if (_browser is not null)
        {
            await _browser.CloseAsync();
            await _browser.DisposeAsync();

            _browser = null;
        }

        _disposed = true;
    }
}