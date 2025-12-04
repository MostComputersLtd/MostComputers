using MOSTComputers.Models.Product.Models;

namespace MOSTComputers.Services.ProductRegister.Utils.Caching;
internal class CacheKeyUtils
{
    internal static class ForProduct
    {
        private const string _coreWord = "products";

        internal static readonly string GetAllKey = _coreWord;

        internal static string GetAllWithStatusKey(ProductStatus productStatus)
        {
            string productStatusAsString = ((int)productStatus).ToString();

            return $"{_coreWord}:ByStatus:{productStatusAsString}";
        }

        internal static string GetAllInCategoryKey(int? categoryId)
        {
            string categoryIdAsString = categoryId?.ToString() ?? "null";

            return $"{_coreWord}:ByCategory:{categoryIdAsString}";
        }

        internal static string GetByIdKey(int id)
        {
            return $"{_coreWord}:ById:{id}";
        }
    }

    internal static class ForProductProperty
    {
        private const string _coreWord = "productProperties";

        internal static string GetAllInProductKey(int productId)
        {
            return $"{_coreWord}:ByProduct:{productId}";
        }

        internal static string GetCountOfAllInProductKey(int productId)
        {
            return $"{_coreWord}:Count:ByProduct:{productId}";
        }
        internal static string GetByProductAndCharacteristicIdKey(int productId, int characteristicId)
        {
            return $"{_coreWord}:ByProductAndCharacteristic:{productId}:{characteristicId}";
        }

        internal static string GetProductIdTag(int productId)
        {
            return $"{_coreWord}:Tag:ByProduct:{productId}";
        }

        internal static string GetCharacteristicIdTag(int characteristicId)
        {
            return $"{_coreWord}:Tag:ByCharacteristic:{characteristicId}";
        }
    }

    internal static class ForProductImageFile
    {
        private const string _coreWord = "productImageFiles";

        internal static string GetAllByProductIdKey(int productId)
        {
            return $"{_coreWord}:ByProduct:{productId}";
        }

        internal static string GetByIdKey(int id)
        {
            return $"{_coreWord}:ById:{id}";
        }

        internal static string GetByProductIdAndImageIdKey(int productId, int imageId)
        {
            return $"{_coreWord}:ByProductAndImage:{productId}:{imageId}";
        }
    }

    internal static class ForPromotionFile
    {
        private const string _coreWord = "promotionProductFiles";

        internal static string GetAllKey()
        {
            return _coreWord;
        }

        internal static string GetAllByActivityKey(bool active)
        {
            return $"{_coreWord}:ByActivity:{active}";
        }

        internal static string GetByIdKey(int id)
        {
            return $"{_coreWord}:ById:{id}";
        }
    }

    internal static class ForPromotionProductFile
    {
        private const string _coreWord = "promotionFiles";

        internal static string GetAllByProductIdKey(int productId)
        {
            return $"{_coreWord}:ByProduct:{productId}";
        }

        internal static string GetCountOfAllByProductIdKey(int productId)
        {
            return $"{_coreWord}:Count:ByProduct:{productId}";
        }

        internal static string GetByIdKey(int id)
        {
            return $"{_coreWord}:ById:{id}";
        }
    }

    internal static class ForProductWorkStatuses
    {
        private const string _coreWord = "productWorkStatuses";

        internal static string GetByIdKey(int id)
        {
            return $"{_coreWord}:ById:{id}";
        }

        internal static string GetByProductIdKey(int productId)
        {
            return $"{_coreWord}:ByProduct:{productId}";
        }

        internal static string GetIdTag(int id)
        {
            return $"{_coreWord}:Tag:ById:{id}";
        }

        internal static string GetProductIdTag(int productId)
        {
            return $"{_coreWord}:Tag:ByProduct:{productId}";
        }
    }
}