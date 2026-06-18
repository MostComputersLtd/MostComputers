const promotionGroupsListId = "promotionGroups";
const promotionImagesListId = "promotionImages";

const displayedGroupIdAttribute = "data-displayed-group-id";
const userFocusedPromotionIdAttribute = "data-user-focused-promotion-id";

function getPromotionElementId(promotionId, groupId = null) {

    if (groupId == null) {

        return `groupPromotion-default-${promotionId}`;
    }

    return `groupPromotion-${promotionId}`;
}

const activePromotionImagesClass = "active";
const selectedPromotionGroupClass = "selected";

window.addEventListener("pageshow", scrollToUserFocusedElement);
window.addEventListener("DOMContentLoaded", scrollToUserFocusedElement);

function scrollToUserFocusedElement() {

    const promotionImagesList = document.getElementById(promotionImagesListId);

    const displayedGroupIdString = promotionImagesList.getAttribute(displayedGroupIdAttribute);

    let displayedGroupId = parseInt(displayedGroupIdString);

    if (displayedGroupId == null || isNaN(displayedGroupId)) {

        displayedGroupId = null;
    }

    const userFocusedPromotionIdString = promotionImagesList.getAttribute(userFocusedPromotionIdAttribute);

    const userFocusedPromotionId = parseInt(userFocusedPromotionIdString);

    if (userFocusedPromotionId == null || isNaN(userFocusedPromotionId)) return;

    const elementToShowId = getPromotionElementId(userFocusedPromotionId, displayedGroupId);

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