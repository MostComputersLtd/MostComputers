export const promotionSearchInputId = "groupPromotionSearchInput";
export const groupSelectId = "promotionGroupSelect";
export const onlyActiveCheckboxId = "groupPromotionOnlyActivePromotionsCheckbox";

export const searchPromotionButtonId = "searchPromotionButton";
export const addPromotionButtonId = "addPromotionButton";
export const addPromotionGroupButtonId = "addPromotionGroupButton";

export const promotionListContainerId = "promotionListContainer";

export const promotionEditPanelContainerId = "promotionEditPanelContainer";
export const promotionEditPanelId = "promotionEditPanel";

export const savePromotionButtonId = "savePromotionButton";
export const editPromotionGroupButtonId = "editPromotionGroupButton";
export const resetPromotionGroupEditorButtonId = "resetPromotionGroupEditorButton";
export const promotionImagesAddFileButtonId = "promotionImagesAddFileButton";
export const relatedProductsPopupOpenButtonId = "relatedProductsPopupOpenButton";

export const promotionNameId = "promotionName";
export const promotionStartDateId = "promotionStartDate";
export const promotionExpirationDateId = "promotionExpirationDate";
export const promotionDisplayOrderId = "promotionDisplayOrder";
export const promotionGroupSelectId = "promotionGroupEditorSelect";
export const promotionDisabledId = "promotionDisabled";
export const promotionRestrictedId = "promotionRestricted";
export const promotionMemberDefaultId = "promotionMemberDefault";
export const promotionDefaultPriorityId = "promotionDefaultPriority";
export const promotionHtmlId = "promotionHtml";

export const promotionImagesInputId = "promotionImagesInput";
export const promotionImagesListId = "promotionImagesList";

export function getImageListItemIdFromIndex(imageIndex) {
    return `promotionEditImageListItem-${imageIndex}`;
}

export function getImageElementIdFromIndex(imageIndex) {
    return `promotionEditImage-${imageIndex}`;
}

export function getImageListItemDeleteButtonIdFromIndex(imageIndex) {
    return `promotionEditImageDeleteButton-${imageIndex}`;
}

export const promotionGroupEditorPopupContainerId = "promotionGroupEditorPopupContainer";

export const promotionGroupEditorPopupId = "promotionGroupEditorPopup";

export const promotionGroupSaveButtonId = "promotionGroupSaveButton";
export const promotionGroupNameInputId = "promotionGroupName";
export const promotionGroupDisplayOrderInputId = "promotionGroupDisplayOrder";
export const promotionGroupLogoImageDisplayId = "promotionGroupLogoImageDisplay";
export const promotionGroupLogoChangeButtonId = "promotionGroupLogoChangeButton";
export const promotionGroupLogoInputId = "promotionGroupLogoInput";
export const promotionGroupEditLogoImageContainerId = "promotionGroupEditLogoImageContainer";
export const promotionGroupImageDeleteButtonId = "promotionGroupImageDeleteButton";

export const addRelatedProductsPopupContainerId = "addRelatedProductsPopupContainer";

export const relatedProductsPopupId = "relatedProductsPopup";
export const relatedProductManufacturerSelectId = "relatedProductManufacturerSelect";
export const relatedProductSearchInputId = "relatedProductSearchInput";
export const relatedProductSearchAvaliableOnlyCheckboxId = "relatedProductSearchAvaliableOnlyCheckbox";
export const relatedProductSearchButtonId = "relatedProductSearchButton";
export const addRelatedProductSearchResultsContainerId = "relatedProductSearchResultsContainer";
export const relatedProductSearchResultsTableId = "relatedProductSearchResultsTable";
export const promotionEditRelatedProductsTableId = "promotionEditRelatedProductsTable";

export const antiforgeryTokenInputId = "__RequestVerificationToken";

export const promotionListItemsName = "promotionListItem";

export const promotionImageListItemsName = "promotionEditImageListItem";
export const promotionImagesName = "promotionEditImage";
export const promotionImageDeleteButtonsName = "promotionEditImageDeleteButton";
export const htmlContentImageSpanName = "groupPromotionEditorHtmlContentImageSpan";

export const relatedProductRemoveButtonName = "promotionEditRelatedProductRemoveButton";

export const relatedProductSearchResultName = "relatedProductSearchResult";

export const promotionListItemPromotionIdAttribute = "data-promotion-id";

export const promotionEditorPromotionIdAttribute = "data-current-promotion-id";
export const promotionEditorPromotionGroupIdAttribute = "data-current-promotion-group-id";

export const promotionEditorImageIdAttribute = "data-current-promotion-image-id";
export const promotionEditorImageIndexAttribute = "data-current-promotion-image-index";

export const htmlContentImageSpanImageIndexAttribute = "data-promotion-image-index-in-span";
export const htmlContentImageSpanImageIdAttribute = "data-promotion-image-id-in-span";

export const promotionRelatedProductIdAttribute = "data-promotion-edit-related-product-id";
export const promotionRelatedSearchResultProductIdAttribute = "data-related-product-search-result-product-id";

export const loadingClass = "loading";

export const htmlContentImageSpanClass = "promotion-edit-html-content-image-span";

export function getNumberOrNull(numberValue) {
    return typeof numberValue === "number" ? numberValue : null;
}

export function getIntegerOrNullFromString(stringValue) {

    if (stringValue == null || stringValue === "") return null;

    var output = null;

    const parsedNumber = parseInt(stringValue);

    if (!isNaN(parsedNumber)) {
        output = parsedNumber;
    }

    return output;
}
