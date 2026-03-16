const promotionGroupsListId = "promotionGroups";
const promotionImagesListId = "promotionImages";

const userFocusedImageIdAttribute = "data-user-focused-image-id";

function getPromotionGroupImageId(imageFileId) {
    return `promotionGroupImage-${imageFileId}`;
}

const activePromotionImagesClass = "active";
const selectedPromotionGroupClass = "selected";

window.addEventListener("pageshow", scrollToUserFocusedElement);
window.addEventListener("DOMContentLoaded", scrollToUserFocusedElement);

function scrollToUserFocusedElement() {

    const promotionImagesList = document.getElementById(promotionImagesListId);

    const userFocusedImageIdString = promotionImagesList.getAttribute(userFocusedImageIdAttribute);

    const userFocusedImageId = parseInt(userFocusedImageIdString);

    if (userFocusedImageId == null || isNaN(userFocusedImageId)) return;

    const elementToShowId = getPromotionGroupImageId(userFocusedImageId);

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