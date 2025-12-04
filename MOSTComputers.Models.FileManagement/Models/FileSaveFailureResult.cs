namespace MOSTComputers.Models.FileManagement.Models;

public class FileSaveFailureResult
{
    private readonly int _index;

    public NotSupportedContentTypeResult? UnsupportedContentType { get; }
    public InvalidContentResult? InvalidContent { get; }
    public TooLargeResult? TooLarge { get; }
    public UnexpectedResult? Unexpected { get; }

    private FileSaveFailureResult(NotSupportedContentTypeResult error)
    {
        UnsupportedContentType = error;
        _index = 0;
    }

    private FileSaveFailureResult(InvalidContentResult error)
    {
        InvalidContent = error;
        _index = 1;
    }

    private FileSaveFailureResult(TooLargeResult error)
    {
        TooLarge = error;
        _index = 2;
    }

    private FileSaveFailureResult(UnexpectedResult error)
    {
        Unexpected = error;
        _index = 3;
    }

    public static FileSaveFailureResult FromUnsupportedContentType(NotSupportedContentTypeResult error) => new(error);
    public static FileSaveFailureResult FromInvalidContent(InvalidContentResult error) => new(error);
    public static FileSaveFailureResult FromTooLarge(TooLargeResult error) => new(error);
    public static FileSaveFailureResult FromUnexpected(UnexpectedResult error) => new(error);

    public T Match<T>(
        Func<NotSupportedContentTypeResult, T> unknown,
        Func<InvalidContentResult, T> invalid,
        Func<TooLargeResult, T> tooLarge,
        Func<UnexpectedResult, T> unexpected)
    {
        return _index switch
        {
            0 => unknown(UnsupportedContentType!.Value),
            1 => invalid(InvalidContent!.Value),
            2 => tooLarge(TooLarge!.Value),
            3 => unexpected(Unexpected!.Value),
            _ => throw new InvalidOperationException("Invalid FileSaveErrorResult state")
        };
    }

    public bool IsUnsupportedContentType => _index == 0;
    public bool IsInvalidContent => _index == 1;
    public bool IsTooLarge => _index == 2;
    public bool IsUnexpected => _index == 3;
}

public record struct NotSupportedContentTypeResult(string? ContentType);
public record struct InvalidContentResult(string Reason);
public record struct TooLargeResult(long MaxSizeBytes, long ActualSizeBytes);
public record struct UnexpectedResult(string? ExceptionMessage);