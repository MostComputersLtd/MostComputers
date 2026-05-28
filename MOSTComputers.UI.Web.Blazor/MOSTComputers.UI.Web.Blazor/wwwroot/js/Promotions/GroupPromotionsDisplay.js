const promotionGroupsListId = "promotionGroups";
const promotionImagesListId = "promotionImages";

//const userFocusedImageIdAttribute = "data-user-focused-image-id";
const userFocusedPromotionIdAttribute = "data-user-focused-promotion-id";

//function getPromotionGroupImageId(imageFileId) {
//    return `promotionGroupImage-${imageFileId}`;
//}

function getPromotionGroupId(promotionId) {
    return `groupPromotion-${promotionId}`;
}

const activePromotionImagesClass = "active";
const selectedPromotionGroupClass = "selected";

function scrollToUserFocusedElement() {

    const promotionImagesList = document.getElementById(promotionImagesListId);

    const userFocusedImageIdString = promotionImagesList.getAttribute(userFocusedPromotionIdAttribute);

    const userFocusedImageId = parseInt(userFocusedImageIdString);

    if (userFocusedImageId == null || isNaN(userFocusedImageId)) return;

    const elementToShowId = getPromotionGroupId(userFocusedImageId);

    const elementToShow = document.getElementById(elementToShowId);

    if (!elementToShow) return;

    const elementToShowContainer = elementToShow.parentElement;

    if (!elementToShowContainer.classList.contains(activePromotionImagesClass)) return;

    elementToShow.scrollIntoView({ behavior: "instant", block: "center" });
}

function changeActiveDisplayedGroup(newActiveImagesContainerId, newActiveGroupButtonId) {

    const promotionGroupsList = document.getElementById(promotionGroupsListId);
    const promotionImagesList = document.getElementById(promotionImagesListId);

    const newActiveImagesContainer = document.getElementById(newActiveImagesContainerId);
    const newActiveGroupButton = document.getElementById(newActiveGroupButtonId);

    const currentActiveImagesContainer = promotionImagesList.querySelector(`.${activePromotionImagesClass}`);
    const currentActiveGroupButton = promotionGroupsList.querySelector(`.${selectedPromotionGroupClass}`);

    if (currentActiveImagesContainer != null) {
        currentActiveImagesContainer.classList.remove(activePromotionImagesClass);
    }

    if (currentActiveGroupButton != null) {
        currentActiveGroupButton.classList.remove(selectedPromotionGroupClass);
    }

    newActiveImagesContainer.classList.add(activePromotionImagesClass);
    newActiveGroupButton.classList.add(selectedPromotionGroupClass);

    promotionImagesList.scrollTo(0, 0);
}