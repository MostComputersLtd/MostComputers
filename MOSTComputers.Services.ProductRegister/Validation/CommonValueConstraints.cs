namespace MOSTComputers.Services.ProductRegister.Validation;
internal static class CommonValueConstraints
{
    internal const int CreateUserNameMaxLength = 256;
    internal const int LastUpdateUserNameMaxLength = 256;

    internal static class ProductImageFileNameInfoConstraints
    {
        internal const int FileNameMaxLength = 50;
    }

    internal static class GroupPromotionImageFileDataConstraints
    {
        internal const int FileNameMaxLength = 50;
    }

    internal static class PromotionFileInfoConstraints
    {
        internal const int NameMaxLength = 50;
        internal const int FileNameMaxLength = 200;
        internal const int DescriptionMaxLength = 1000;
        internal const int RelatedProductsStringMaxLength = 1000;

        internal const int CreateUserNameMaxLength = 256;
        internal const int LastUpdateUserNameMaxLength = 256;
    }

    internal static class PromotionProductFileInfoConstraints
    {
        internal const int CreateUserNameMaxLength = 256;
        internal const int LastUpdateUserNameMaxLength = 256;
    }

    internal static class ProductGTINCodeConstraints
    {
        internal const int CodeTypeAsTextMaxLength = 20;

        internal const int ValueMaxLength = 14;

        internal const int CreateUserNameMaxLength = 256;
        internal const int LastUpdateUserNameMaxLength = 256;
    }

    internal static class ProductSerialNumberConstraints
    {
        internal const int SerialNumberMaxLength = 255;
    }
}