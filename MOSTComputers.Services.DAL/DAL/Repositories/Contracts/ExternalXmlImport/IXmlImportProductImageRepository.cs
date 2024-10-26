using MOSTComputers.Models.Product.Models.ExternalXmlImport;
using MOSTComputers.Models.Product.Models.ExternalXmlImport.ProductImage;
using MOSTComputers.Models.Product.Models.Validation;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.DAL.DAL.Repositories.Contracts.ExternalXmlImport;
public interface IXmlImportProductImageRepository
{
    IEnumerable<XmlImportProductImage> GetAllFirstImagesForAllProducts();
    IEnumerable<XmlImportProductImage> GetAllInProduct(int productId);
    XmlImportProductImage? GetByIdInAllImages(int id);
    XmlImportProductImage? GetByProductIdInFirstImages(int productId);
    IEnumerable<XmlImportProductImage> GetFirstImagesForSelectionOfProducts(List<int> productIds);
    OneOf<int, UnexpectedFailureResult> UpsertInAllImages(XmlImportProductImageUpsertRequest createRequest);
    OneOf<int, UnexpectedFailureResult> InsertInAllImagesAndImageFileNameInfos(XmlImportProductImageUpsertRequest createRequest, int? displayOrder = null);
    OneOf<Success, UnexpectedFailureResult> InsertInFirstImages(XmlImportProductFirstImageCreateRequest createRequest);
    OneOf<Success, UnexpectedFailureResult> UpdateInAllImages(XmlImportProductImageUpdateRequest updateRequest);
    OneOf<Success, UnexpectedFailureResult> UpdateInFirstImages(XmlImportProductFirstImageUpdateRequest updateRequest);
    OneOf<bool, UnexpectedFailureResult> UpdateHtmlDataInAllImagesById(int imageId, string htmlData);
    OneOf<bool, UnexpectedFailureResult> UpdateHtmlDataInFirstAndAllImagesByProductId(int productId, string htmlData);
    OneOf<bool, UnexpectedFailureResult> UpdateHtmlDataInFirstImagesByProductId(int productId, string htmlData);
    bool DeleteAllWithSameProductIdInAllImages(int productId);
    bool DeleteInAllImagesAndImageFilePathInfosById(int id);
    bool DeleteInAllImagesById(int id);
    bool DeleteInFirstImagesByProductId(int id);
}