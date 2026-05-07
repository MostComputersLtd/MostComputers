using Microsoft.AspNetCore.WebUtilities;
using System.Security.Cryptography;

namespace MOSTComputers.UI.Web.Blazor.Services.BrowserResourceHashing;

public class WebResourceUrlsHashingService
{
    public string GetLocalFileHash(string filePath)
    {
        using FileStream stream = File.OpenRead(filePath);

        return GetLocalFileHash(stream);
    }

    public string GetLocalFileHash(Stream stream)
    {
        using SHA256 sha256 = SHA256.Create();

        byte[] hash = sha256.ComputeHash(stream);
        string version = WebEncoders.Base64UrlEncode(hash);

        return version;
    }
}
