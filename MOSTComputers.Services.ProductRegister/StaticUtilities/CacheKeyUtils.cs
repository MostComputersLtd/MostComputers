namespace MOSTComputers.Services.ProductRegister.StaticUtilities;

public static class CacheKeyUtils
{
    internal static class Category
    {
        private const string _coreWord = "categories";

        internal const string GetAllKey = _coreWord;
        internal static string GetByIdKey(int id)
        {
            return $"{_coreWord}:{id}";
        }
    }

    internal static class Manifacturer
    {
        private const string _coreWord = "manifacturers";

        internal const string GetAllKey = _coreWord;
        internal static string GetByIdKey(int id)
        {
            return $"{_coreWord}:{id}";
        }
    }

    internal static class ProductCharacteristic
    {
        private const string _coreWord = "productCharacteristics";

        internal static string GetByCategoryIdKey(int categoryId)
        {
            return $"categories/{_coreWord}:{categoryId}";
        }

        internal static string GetByCategoryIdAndNameKey(int categoryId, string name)
        {
            return $"{_coreWord}:{categoryId};{name}";
        }
    }

    internal static class ProductImageFileNameInfo
    {
        private const string _coreWord = "productImageFileNameInfos";

        internal const string GetAllKey = _coreWord;
        internal static string GetByProductIdKey(int productId)
        {
            return $"products/{_coreWord}:{productId}";
        }
    }

    internal static class ProductImage
    {
        private const string _coreWord = "productImages";

        internal static string GetInAllImagesByIdKey(int id)
        {
            return $"{_coreWord}/all:{id}";
        }

        internal static string GetInAllImagesByProductIdKey(int productId)
        {
            return $"products/{_coreWord}/all:{productId}";
        }

        internal static string GetInFirstImagesByIdKey(int id)
        {
            return $"products/{_coreWord}/first:{id}";
        }
    }

    internal static class ProductProperty
    {
        private const string _coreWord = "productProperties";

        internal static string GetByProductIdKey(int productId)
        {
            return $"products/{_coreWord}:{productId}";
        }
    }

    internal static class ProductStatuses
    {
        private const string _coreWord = "productStatuses";

        internal static string GetByProductIdKey(int productId)
        {
            return $"products/{_coreWord}:{productId}";
        }
    }

    public static class ForProduct
    {
        private const string _coreWord = "products";

        internal static string GetByIdKey(int id)
        {
            return $"{_coreWord}:{id}";
        }

        internal static string GetBySearchStringKey(string searchString)
        {
            return $"{_coreWord}/searchString:{searchString}";
        }

        internal static string GetByNameKey(string name)
        {
            return $"{_coreWord}/name:{name}";
        }

        internal static string GetByOrderKey(int order)
        {
            return $"{_coreWord}/order:{order}";
        }

        internal static string GetBySearchStringAndOrderKey(string searchString, int order)
        {
            return $"{_coreWord}/searchString;order:{searchString};{order}";
        }

        internal static string GetByNameAndOrderKey(string name, int order)
        {
            return $"{_coreWord}/name;order:{name};{order}";
        }

        public static string GetUpdatedByIdKey(int id)
        {
            return $"{_coreWord}/updated:{id}";
        }
    }

    internal static class ProductImageSaveKeys
    {
        private const string _coreWord = "productImageSave";
        public static string GetProductImagesByProductIdKey(int productId)
        {
            return $"{_coreWord}/{productId}";
        }
    }
}