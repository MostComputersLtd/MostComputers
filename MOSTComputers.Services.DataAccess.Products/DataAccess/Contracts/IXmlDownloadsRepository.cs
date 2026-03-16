using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Products.Models.Responses.XmlDownloads;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Contracts;
public interface IXmlDownloadsRepository
{
    Task<OneOf<Success, UnexpectedFailureResult>> InsertAsync(XmlDownloadData xmlDownloadData);
}