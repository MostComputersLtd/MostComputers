using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Services.ProductImageFileManagement.Models;
using MOSTComputers.Services.ProductImageFileManagement.RollbackableOperations;
using MOSTComputers.Services.TransactionalFileManagement.Services.Contracts;
using OneOf;
using OneOf.Types;
using SixLabors.ImageSharp;

using static MOSTComputers.Services.ProductImageFileManagement.Utils.ProductImageFileManagementUtils;

namespace MOSTComputers.Services.ProductImageFileManagement.Services;

internal sealed class ProductImageFileManagementService : IProductImageFileManagementService
{
    public ProductImageFileManagementService(string imageDirectoryFullPath,
        ITransactionalFileManager transactionalFileManager)
    {
        _imageDirectoryPath = imageDirectoryFullPath;
        _transactionalFileManager = transactionalFileManager;
    }

    private readonly string _imageDirectoryPath;
    private readonly ITransactionalFileManager _transactionalFileManager;

    public OneOf<string[], DirectoryNotFoundResult> GetAllImageFileNames()
    {
        if (!Directory.Exists(_imageDirectoryPath))
        {
            return new DirectoryNotFoundResult();
        }

        return Directory.GetFiles(_imageDirectoryPath);
    }

    public List<byte[]> GetAllStartingWith(string startingChars)
    {
        if (string.IsNullOrEmpty(startingChars)) return new();

        IEnumerable<string> fileNames = Directory.EnumerateFiles(_imageDirectoryPath, $"{startingChars}*");

        if (!fileNames.Any()) return new();

        List<byte[]> output = new();

        foreach (string fileName in fileNames)
        {
            var filePath = Path.Combine(_imageDirectoryPath, fileName);

            byte[] fileData = File.ReadAllBytes(filePath);

            output.Add(fileData);
        }

        return output;
    }

    public List<byte[]> GetAllStartingWithProductId(int productId)
    {
        if (productId <= 0) return new();

        string startingChars = $"{productId}-";

        IEnumerable<string> fileNames = Directory.EnumerateFiles(_imageDirectoryPath, $"{startingChars}*");

        if (!fileNames.Any()) return new();

        List<byte[]> output = new();

        foreach (string fileName in fileNames)
        {
            var filePath = Path.Combine(_imageDirectoryPath, fileName);

            byte[] fileData = File.ReadAllBytes(filePath);

            output.Add(fileData);
        }

        return output;
    }

    public byte[]? GetByImageId(int imageId)
    {
        if (imageId <= 0) return null;

        string startingChars = $"{imageId}.";

        string? fileName = Directory.EnumerateFiles(_imageDirectoryPath, $"{startingChars}*")
            .FirstOrDefault();

        if (fileName is null) return null;

        var filePath = Path.Combine(_imageDirectoryPath, fileName);

        byte[] fileData = File.ReadAllBytes(filePath);

        return fileData;
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
        RenameImageFileOperation renameImageFileOperation = new(_imageDirectoryPath, fileNameWithoutExtension, newFileNameWithoutExtension, allowedImageFileType);
        
        return _transactionalFileManager.EnlistOrExecuteOperation(renameImageFileOperation);

        //string fullFileName = $"{fileNameWithoutExtension}.{allowedImageFileType.FileExtension}";

        //string filePath = Path.Combine(_imageDirectoryPath, fullFileName);

        //if (!File.Exists(filePath)) return new FileDoesntExistResult() { FileName = fullFileName };

        //string newFullFileName = $"{newFileNameWithoutExtension}.{allowedImageFileType.FileExtension}";

        //string newFilePath = Path.Combine(_imageDirectoryPath, newFullFileName);

        //if (File.Exists(newFilePath)) return new FileAlreadyExistsResult() { FileName = newFullFileName };

        //File.Move(filePath, newFilePath);

        //return new Success();
    }

    public async Task<OneOf<Success, DirectoryNotFoundResult, FileAlreadyExistsResult>> AddImageAsync(
        string fileNameWithoutExtension, byte[] imageData, AllowedImageFileType imageFileType)
    {
        CreateImageFileAsyncOperation createImageFileAsyncOperation = new(_imageDirectoryPath, fileNameWithoutExtension, imageData, imageFileType);

        return await _transactionalFileManager.EnlistOrExecuteOperationAsync(createImageFileAsyncOperation);

        //if (!Directory.Exists(_imageDirectoryPath)) return new DirectoryNotFoundResult();

        //string fullFileName = $"{fileNameWithoutExtension}.{imageFileType.FileExtension}";

        //string filePath = Path.Combine(_imageDirectoryPath, fullFileName);

        //if (File.Exists(filePath)) return new FileAlreadyExistsResult() { FileName = fullFileName };

        //using MemoryStream memoryStream = new(imageData);

        //using Image image = await Image.LoadAsync(memoryStream);

        //await image.SaveAsync(filePath);

        //return new Success();
    }

    public async Task<OneOf<Success, DirectoryNotFoundResult, FileDoesntExistResult>> UpdateImageAsync(
        string fileNameWithoutExtension, byte[] imageData, AllowedImageFileType imageFileType)
    {
        UpdateImageFileAsyncOperation updateImageFileAsyncOperation = new(_imageDirectoryPath, fileNameWithoutExtension, imageData, imageFileType);

        return await _transactionalFileManager.EnlistOrExecuteOperationAsync(updateImageFileAsyncOperation);

        //if (!Directory.Exists(_imageDirectoryPath)) return new DirectoryNotFoundResult();

        //string fullFileName = $"{fileNameWithoutExtension}.{imageFileType.FileExtension}";

        //string filePath = Path.Combine(_imageDirectoryPath, fullFileName);

        //if (!File.Exists(filePath)) return new FileDoesntExistResult() { FileName = fullFileName };

        //using MemoryStream memoryStream = new(imageData);

        //using Image image = await Image.LoadAsync(memoryStream);

        //await image.SaveAsync(filePath);

        //return new Success();
    }

    public async Task<OneOf<Success, DirectoryNotFoundResult>> AddOrUpdateImageAsync(
        string fileNameWithoutExtension, byte[] imageData, AllowedImageFileType imageFileType)
    {
        AddOrUpdateImageFileAsyncOperation addOrUpdateImageFileAsyncOperation = new(_imageDirectoryPath, fileNameWithoutExtension, imageData, imageFileType);

        return await _transactionalFileManager.EnlistOrExecuteOperationAsync(addOrUpdateImageFileAsyncOperation);

        //if (!Directory.Exists(_imageDirectoryPath)) return new DirectoryNotFoundResult();

        //string fullFileName = $"{fileNameWithoutExtension}.{imageFileType.FileExtension}";

        //string filePath = Path.Combine(_imageDirectoryPath, fullFileName);

        //using MemoryStream memoryStream = new(imageData);

        //using Image image = await Image.LoadAsync(memoryStream);

        //await image.SaveAsync(filePath);

        //return new Success();
    }

    public OneOf<Success, FileDoesntExistResult> DeleteFile(string fullFileName)
    {
        DeleteImageFileOperation deleteImageFileOperation = new(_imageDirectoryPath, fullFileName);

        return _transactionalFileManager.EnlistOrExecuteOperation(deleteImageFileOperation);

        //string filePath = Path.Combine(_imageDirectoryPath, fullFileName);

        //if (!File.Exists(filePath))
        //{
        //    return new FileDoesntExistResult() { FileName = fullFileName };
        //}

        //File.Delete(filePath);

        //return new Success();
    }
}