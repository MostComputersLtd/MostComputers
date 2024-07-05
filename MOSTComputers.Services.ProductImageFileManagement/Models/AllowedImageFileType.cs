using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;

namespace MOSTComputers.Services.ProductImageFileManagement.Models;

// This is an enum like object
public class AllowedImageFileType
{
    private AllowedImageFileType(int value, string fileExtension, Func<IImageEncoder> getEncoderFunc)
    {
        Value = value;
        FileExtension = fileExtension;
        _getEncoderFunc = getEncoderFunc;
    }

    public int Value { get; }
    public string FileExtension { get; }

    private readonly Func<IImageEncoder> _getEncoderFunc;

    public readonly static AllowedImageFileType JPG = new(0, "jpg", () => new JpegEncoder());
    public readonly static AllowedImageFileType JPEG = new(1, "jpeg", () => new JpegEncoder());
    public readonly static AllowedImageFileType PNG = new(2, "png", () => new PngEncoder());

    internal IImageEncoder GetImageEncoder()
    {
        return _getEncoderFunc();
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;

        if (obj is not AllowedImageFileType allowedImageFileType) return false;

        return Value == allowedImageFileType.Value;
    }

    public override int GetHashCode()
    {
        return Value;
    }

    public static bool operator ==(AllowedImageFileType allowedImageFileType, object? obj)
    {
        return allowedImageFileType.Equals(obj);
    }

    public static bool operator !=(AllowedImageFileType allowedImageFileType, object? obj)
    {
        return !allowedImageFileType.Equals(obj);
    }
}