import * as common from "./Common.js";
import * as promotionEditor from "./PromotionEditor.js";
import * as promotionGroupEditor from "./PromotionGroupEditor.js";

document.addEventListener("DOMContentLoaded", function ()
{
    const searchPromotionButton = document.getElementById(common.searchPromotionButtonId);
    const addPromotionButton = document.getElementById(common.addPromotionButtonId);
    const addPromotionGroupButton = document.getElementById(common.addPromotionGroupButtonId);

    searchPromotionButton.addEventListener("click", searchGroupPromotions);
    addPromotionButton.addEventListener("click", openGroupPromotionEditorPopup);
    addPromotionGroupButton.addEventListener("click", openPromotionGroupEditorPopup);
});

export async function searchGroupPromotions() {

    const searchOptions = getGroupPromotionSearchOptionsFromCurrentData();

    const response = await fetch("api/components/promotionGroups/search", {
        method: "POST",
        headers: {
            'Content-Type': "application/json",
            'Accept': "application/html",
            "RequestVerificationToken": document.getElementById(common.antiforgeryTokenInputId).value
        },
        body: JSON.stringify(searchOptions)
    });

    const searchResultHtml = await response.text();

    const promotionListContainer = document.getElementById(common.promotionListContainerId);

    promotionListContainer.innerHTML = searchResultHtml;

    const promotionListItems = [...promotionListContainer.querySelectorAll(`[name='${common.promotionListItemsName}']`)];

    for (const promotionListItem of promotionListItems) {

        promotionListItem.addEventListener("click", () => openPromotionGroupEditorPopupFromListItem(promotionListItem));
    }
}

function getGroupPromotionSearchOptionsFromCurrentData() {

    const searchInput = document.getElementById(common.promotionSearchInputId);
    const groupSelect = document.getElementById(common.groupSelectId);
    const onlyActiveCheckbox = document.getElementById(common.onlyActiveCheckboxId);

    const searchInputValue = searchInput.value;
    const groupSelectValue = common.getIntegerOrNullFromString(groupSelect.value);
    const activeOnlyValue = onlyActiveCheckbox.checked;

    return {
        SearchData: searchInputValue,
        PromotionGroupId: groupSelectValue,
        ActiveOnly: activeOnlyValue
    };
}

function openGroupPromotionEditorPopup() {
    promotionEditor.openPromotionEditorForPromotion(null);
}

function openPromotionGroupEditorPopup() {
    promotionGroupEditor.openPromotionGroupEditorPopup(null)
}

function openPromotionGroupEditorPopupFromListItem(promotionListItem) {

    const promotionIdAttribute = promotionListItem.getAttribute(common.promotionListItemPromotionIdAttribute);

    const promotionId = common.getIntegerOrNullFromString(promotionIdAttribute);

    promotionEditor.openPromotionEditorForPromotion(promotionId);
}