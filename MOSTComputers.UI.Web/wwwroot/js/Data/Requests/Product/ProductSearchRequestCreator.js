function getProductSearchRequestAsObject(
    productSearchData = null,
    categoryId = null,
    productStatus = null,
    productNewStatusesAsString = null,
    promotionStatus = null)
{
    const categoryIdInOptions = getIntOrNullFromString(categoryId);

    const productStatusInOptions = getIntOrNullFromString(productStatus);

    const productNewStatusesInOptions = getProductNewStatusesFromSelectElementValue(productNewStatusesAsString);

    const promotionStatusInOptions = getIntOrNullFromString(promotionStatus);

    return {
        UserInputString: productSearchData,
        CategoryId: categoryIdInOptions,
        ProductStatus: productStatusInOptions,
        ProductNewStatuses: productNewStatusesInOptions,
        PromotionSearchOptions: promotionStatusInOptions
    }
}