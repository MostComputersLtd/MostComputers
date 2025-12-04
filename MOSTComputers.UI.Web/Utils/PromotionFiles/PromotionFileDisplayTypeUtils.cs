using MOSTComputers.UI.Web.Models.PromotionFiles;
using static MOSTComputers.UI.Web.Utils.FilePathUtils;
using static MOSTComputers.Utils.Files.ContentTypeUtils;

namespace MOSTComputers.UI.Web.Utils.PromotionFiles;
internal static class PromotionFileDisplayTypeUtils
{
    private const string _imageContentTypePrefix = "image/";
    private const string _videoContentTypePrefix = "video/";
    private const string _audioContentTypePrefix = "audio/";

    private const string _pdfFileExtension = "pdf";

    private static readonly string[] _wordExtensions = new string[]
    {
        "docx",
        "doc",
        "dotm",
        "dotx",
    };

    private static readonly string[] _otherDocumentExtensions = new string[]
    {
        "ods",
        "txt",
    };

    private static readonly string[] _excelSpreadsheetExtensions = new string[]
    {
        "xlsx",
        "xls",
        "xlsm",
        "xltx",
        "xltm",
        "xlsb",
    };

    private static readonly string[] _otherSpreadsheetExtensions = new string[]
    {
        "csv",
        "tsv",
    };

    internal static PromotionFileDisplayTypeEnum GetPromotionFileDisplayTypeFromFileName(string fullFileName)
    {
        bool isElementAPdfDocument = DoesFileNameMatchExtension(fullFileName, _pdfFileExtension);

        if (isElementAPdfDocument) return PromotionFileDisplayTypeEnum.PdfDocument;

        bool isElementAWordDocument = DoesFileNameMatchAnyExtension(fullFileName, _wordExtensions);

        if (isElementAWordDocument) return PromotionFileDisplayTypeEnum.WordDocument;

        bool isElementADocument = DoesFileNameMatchAnyExtension(fullFileName, _otherDocumentExtensions);

        if (isElementADocument) return PromotionFileDisplayTypeEnum.OtherDocument;

        bool isElementAnExcelSpreadsheet = DoesFileNameMatchAnyExtension(fullFileName, _excelSpreadsheetExtensions);

        if (isElementAnExcelSpreadsheet) return PromotionFileDisplayTypeEnum.ExcelSpreadsheet;

        bool isElementASpreadsheet = DoesFileNameMatchAnyExtension(fullFileName, _otherSpreadsheetExtensions);

        if (isElementASpreadsheet) return PromotionFileDisplayTypeEnum.OtherSpreadsheet;

        string? contentTypeFromFileExtension = GetContentTypeFromExtension(fullFileName);

        if (contentTypeFromFileExtension is null)
        {
            return PromotionFileDisplayTypeEnum.Unhandled;
        }

        if (contentTypeFromFileExtension.StartsWith(_imageContentTypePrefix))
        {
            return PromotionFileDisplayTypeEnum.Image;
        }
        else if (contentTypeFromFileExtension.StartsWith(_videoContentTypePrefix))
        {
            return PromotionFileDisplayTypeEnum.Video;
        }
        else if (contentTypeFromFileExtension.StartsWith(_audioContentTypePrefix))
        {
            return PromotionFileDisplayTypeEnum.Audio;
        }

        return PromotionFileDisplayTypeEnum.Unhandled;
    }

    private static bool DoesFileNameMatchAnyExtension(string fullFileName, IEnumerable<string> extensions)
    {
        string? fileExtensionWithoutDot = GetFileExtensionWithoutDot(fullFileName);

        if (fileExtensionWithoutDot is null)
        {
            return false;
        }

        foreach (string extension in extensions)
        {
            if (extension.Equals(fileExtensionWithoutDot, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private static bool DoesFileNameMatchExtension(string fullFileName, string extension)
    {
        string? fileExtensionWithoutDot = GetFileExtensionWithoutDot(fullFileName);

        if (fileExtensionWithoutDot is null)
        {
            return false;
        }

        return extension.Equals(fileExtensionWithoutDot, StringComparison.OrdinalIgnoreCase);
    }
}