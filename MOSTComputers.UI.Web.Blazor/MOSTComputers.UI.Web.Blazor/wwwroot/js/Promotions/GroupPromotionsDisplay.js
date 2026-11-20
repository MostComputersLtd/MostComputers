const promotionGroupsListId = "promotionGroups";
const promotionImagesListId = "promotionImages";

const promotionGroupItemName = "promotionGroupItem";

const displayedGroupIdAttribute = "data-displayed-group-id";
const userFocusedPromotionIdAttribute = "data-user-focused-promotion-id";

const itemImageContainerIdAttribute = "data-image-container-id";

const activePromotionImagesClass = "active";
const selectedPromotionGroupClass = "selected";

function getPromotionElementId(promotionId, groupId = null) {

    if (groupId == null) {

        return `groupPromotion-default-${promotionId}`;
    }

    return `groupPromotion-${promotionId}`;
}

export function initEvents() {

    const promotionGroupsList = document.getElementById(promotionGroupsListId);

    const promotionGroupItems = promotionGroupsList.querySelectorAll(`[name='${promotionGroupItemName}']`);

    for (const promotionGroupItem of promotionGroupItems) {

        promotionGroupItem.addEventListener("click", changeActiveDisplayedGroup);
    }
}

export function scrollToUserFocusedElement() {

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

function changeActiveDisplayedGroup(e) {

    const newActiveGroupButton = e.currentTarget;

    const newActiveImagesContainerId = newActiveGroupButton.getAttribute(itemImageContainerIdAttribute);

    const newActiveImagesContainer = document.getElementById(newActiveImagesContainerId);

    const promotionGroupsList = document.getElementById(promotionGroupsListId);
    const promotionImagesList = document.getElementById(promotionImagesListId);

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
