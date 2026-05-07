using Microsoft.Extensions.FileProviders;
using System.Collections.Concurrent;

namespace MOSTComputers.UI.Web.Blazor.Services.BrowserResourceHashing;

public class WebResourceUrlsStorageService
{
    public WebResourceUrlsStorageService(
        WebResourceUrlsHashingService webResourceUrlsHashingService,
        IWebHostEnvironment webHostEnvironment)
    {
        _webResourceUrlsHashingService = webResourceUrlsHashingService;
        _webHostEnvironment = webHostEnvironment;
    }

    public ConcurrentDictionary<string, Lazy<string>> _cachedUrls = new();

    private readonly WebResourceUrlsHashingService _webResourceUrlsHashingService;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public string GetWebRootAssetUrlWithVersion(string url)
    {
        string version = GetWebRootAssetUrlVersion(url);

        return $"{url}?v={version}";
    }

    public string GetWebRootAssetUrlVersion(string url)
    {
        string normalizedUrl = RemoveStartingSlashes(url);

        Lazy<string> lazy = _cachedUrls.GetOrAdd(normalizedUrl, (url) =>
        {
            return new Lazy<string>(() =>
            {
                string fileUrl = Path.Combine(_webHostEnvironment.WebRootPath, url);

                return _webResourceUrlsHashingService.GetLocalFileHash(fileUrl);
            });
        });

        return lazy.Value;
    }

    public string GetBlazorAssetUrlWithVersion(string url)
    {
        string version = GetBlazorAssetUrlVersion(url);

        return $"{url}?v={version}";
    }

    public string GetBlazorAssetUrlVersion(string url)
    {
        string normalizedUrl = RemoveStartingSlashes(url);

        Lazy<string> lazy = _cachedUrls.GetOrAdd(normalizedUrl, (normalizedUrl) =>
        {
            return new Lazy<string>(() =>
            {
                IFileInfo fileInfo = _webHostEnvironment.WebRootFileProvider.GetFileInfo(normalizedUrl);

                using Stream stream = fileInfo.CreateReadStream();

                return _webResourceUrlsHashingService.GetLocalFileHash(stream);
            });
        });

        return lazy.Value;
    }

    private static string RemoveStartingSlashes(string path)
    {
        path = path.Trim();

        if (path.StartsWith("./"))
        {
            path = path[2..];
        }

        if (path.StartsWith('/'))
        {
            path = path[1..];
        }

        return path;
    }
}
