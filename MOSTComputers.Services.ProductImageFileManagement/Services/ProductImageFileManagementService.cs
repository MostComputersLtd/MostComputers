using MOSTComputers.Services.ProductImageFileManagement.Models;
using OneOf;
using OneOf.Types;
using SixLabors.ImageSharp;

using static MOSTComputers.Services.ProductImageFileManagement.Utils.ProductImageFileManagementUtils;

namespace MOSTComputers.Services.ProductImageFileManagement.Services;

internal sealed class ProductImageFileManagementService : IProductImageFileManagementService
{
    public ProductImageFileManagementService(string imageDirectoryFullPath)
    {
        _imageDirectoryPath = imageDirectoryFullPath;
    }

    private readonly string _imageDirectoryPath;

    public OneOf<string[], DirectoryNotFoundResult> GetAllImageFileNames()
    {
        if (!Directory.Exists(_imageDirectoryPath))
        {
            return new DirectoryNotFoundResult();
        }

        return Directory.GetFiles(_imageDirectoryPath);
    }

    public OneOf<byte[], FileDoesntExistResult, NotSupportedFileTypeResult> GetImage(string fullFileName)
    {
        string fileExtension = GetFileExtensionWithoutDot(fullFileName);

        AllowedImageFileType? allowedImageFileTypeData = GetAllowedImageFileTypeFromFileExtension(fileExtension);

        if (allowedImageFileTypeData is null)
        {
            return new NotSupportedFileTypeResult() { FileExtension = fileExtension };
        }

        string filePath = Path.Combine(_imageDirectoryPath, fullFileName);

        if (!File.Exists(filePath))
        {
            return new FileDoesntExistResult() { FileName = fullFileName };
        }

        using Image image = Image.Load(filePath);

        using MemoryStream memoryStream = new();

        image.Save(memoryStream, allowedImageFileTypeData.GetImageEncoder());

        return memoryStream.ToArray();
    }

    public async Task<OneOf<byte[], FileDoesntExistResult, NotSupportedFileTypeResult>> GetImageAsync(string fullFileName)
    {
        string fileExtension = GetFileExtensionWithoutDot(fullFileName);

        AllowedImageFileType? allowedImageFileTypeData = GetAllowedImageFileTypeFromFileExtension(fileExtension);

        if (allowedImageFileTypeData is null)
        {
            return new NotSupportedFileTypeResult() { FileExtension = fileExtension };
        }

        string filePath = Path.Combine(_imageDirectoryPath, fullFileName);

        if (!File.Exists(filePath))
        {
            return new FileDoesntExistResult() { FileName = fullFileName };
        }

        using Image image = await Image.LoadAsync(filePath);

        using MemoryStream memoryStream = new();

        await image.SaveAsync(memoryStream, allowedImageFileTypeData.GetImageEncoder());

        return memoryStream.ToArray();
    }

    public bool CheckIfImageFileExists(string fullFileName)
    {
        string filePath = Path.Combine(_imageDirectoryPath, fullFileName);

        return File.Exists(filePath);
    }

    public OneOf<Success, FileDoesntExistResult, FileAlreadyExistsResult> RenameImageFile(
        string fileNameWithoutExtension,
        string newFileNameWithoutExtension,
        AllowedImageFileType allowedImageFileType)
    {
        string fullFileName = $"{fileNameWithoutExtension}.{allowedImageFileType.FileExtension}";

        string filePath = Path.Combine(_imageDirectoryPath, fullFileName);

        if (!File.Exists(filePath)) return new FileDoesntExistResult() { FileName = fullFileName };

        string newFullFileName = $"{newFileNameWithoutExtension}.{allowedImageFileType.FileExtension}";

        string newFilePath = Path.Combine(_imageDirectoryPath, newFullFileName);

        if (File.Exists(newFilePath)) return new FileAlreadyExistsResult() { FileName = newFullFileName };

        File.Move(filePath, newFilePath);

        return new Success();
    }

    public async Task<OneOf<Success, DirectoryNotFoundResult, FileAlreadyExistsResult>> AddImageAsync(
        string fileNameWithoutExtension, byte[] imageData, AllowedImageFileType imageFileType)
    {
        if (!Directory.Exists(_imageDirectoryPath)) return new DirectoryNotFoundResult();

        string fullFileName = $"{fileNameWithoutExtension}.{imageFileType.FileExtension}";

        string filePath = Path.Combine(_imageDirectoryPath, fullFileName);

        if (File.Exists(filePath)) return new FileAlreadyExistsResult() { FileName = fullFileName };

        using MemoryStream memoryStream = new(imageData);

        using Image image = await Image.LoadAsync(memoryStream);

        await image.SaveAsync(filePath);

        return new Success();
    }

    public async Task<OneOf<Success, DirectoryNotFoundResult, FileDoesntExistResult>> UpdateImageAsync(
        string fileNameWithoutExtension, byte[] imageData, AllowedImageFileType imageFileType)
    {
        if (!Directory.Exists(_imageDirectoryPath)) return new DirectoryNotFoundResult();

        string fullFileName = $"{fileNameWithoutExtension}.{imageFileType.FileExtension}";

        string filePath = Path.Combine(_imageDirectoryPath, fullFileName);

        if (!File.Exists(filePath)) return new FileDoesntExistResult() { FileName = fullFileName };

        using MemoryStream memoryStream = new(imageData);

        using Image image = await Image.LoadAsync(memoryStream);

        await image.SaveAsync(filePath);

        return new Success();
    }

    public async Task<OneOf<Success, DirectoryNotFoundResult>> AddOrUpdateImageAsync(
        string fileNameWithoutExtension, byte[] imageData, AllowedImageFileType imageFileType)
    {
        if (!Directory.Exists(_imageDirectoryPath)) return new DirectoryNotFoundResult();

        string fullFileName = $"{fileNameWithoutExtension}.{imageFileType.FileExtension}";

        string filePath = Path.Combine(_imageDirectoryPath, fullFileName);

        using MemoryStream memoryStream = new(imageData);

        using Image image = await Image.LoadAsync(memoryStream);

        await image.SaveAsync(filePath);

        return new Success();
    }

    public OneOf<Success, FileDoesntExistResult> DeleteFile(string fullFileName)
    {
        string filePath = Path.Combine(_imageDirectoryPath, fullFileName);

        if (!File.Exists(filePath))
        {
            return new FileDoesntExistResult() { FileName = fullFileName };
        }

        File.Delete(filePath);

        return new Success();
    }

    public async Task<OneOf<Success, FileDoesntExistResult>> DeleteFileAsync(string fullFileName)
    {
        string filePath = Path.Combine(_imageDirectoryPath, fullFileName);

        if (!File.Exists(filePath))
        {
            return new FileDoesntExistResult() { FileName = fullFileName };
        }

        await Task.Run(() => File.Delete(filePath));

        return new Success();
    }
}