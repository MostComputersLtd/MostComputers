import * as common from "./Common.js";
import * as promotionEditorRelatedProducts from "./PromotionEditorRelatedProducts.js";

const selectAllButtonId = "relatedProductsSelectAllButton";
const relatedProductsSelectAllButtonTextId = "relatedProductsSelectAllButtonText";

const relatedProductSearchResultInactiveClass = "related-product-search-result-inactive";
const relatedProductSearchResultInSelectionClass = "related-product-search-result-in-selection";

let existingRelatedProductIds = [];

const addedRelatedProductSearchResultsInSelectAll = [];

export function removeFromExistingProductIds(productId) {

    const indexOfId = existingRelatedProductIds.indexOf(productId);

    if (indexOfId < 0) {

        return;
    }

    existingRelatedProductIds.splice(indexOfId, 1);
}

let mouseX = 0;
let mouseY = 0;

let rangeSelectionStartIndex = -1;

let startSearchResultElement = null;
let currentSearchResultElement = null;

let elementIndexesThatWereHandledThisSelection = [];
let elementsThatWereHandledThisSelection = [];

let isHolding = false;

export async function openAddRelatedProductsToPromotionPopup(initialGroupId = null) {

    const response = await fetch(`api/components/promotionGroups/addRelatedProductsPopup?selectedPromotionGroupId=${initialGroupId}`,
    {
        method: "GET",
        headers: {
            "Accept": "application/html"
        }
    });

    if (!response.ok) return;

    const data = await response.text();

    const addRelatedProductsPopupContainer = document.getElementById(common.addRelatedProductsPopupContainerId);

    addRelatedProductsPopupContainer.innerHTML = data;

    const relatedProductSearchButton = document.getElementById(common.relatedProductSearchButtonId);

    relatedProductSearchButton.addEventListener("click", searchRelatedProductsAndDisplayResults);

    const selectAllButton = document.getElementById(selectAllButtonId);

    selectAllButton.addEventListener("click", onSelectAllButtonClicked);

    const addRelatedProductsPopup = document.getElementById(common.relatedProductsPopupId);

    addRelatedProductsPopup.showModal();
}

export async function searchRelatedProductsAndDisplayResults() {

    const searchData = getRelatedProductsSearchOptionsFromCurrentData();

    const relatedProductSearchButton = document.getElementById(common.relatedProductSearchButtonId);

    relatedProductSearchButton.classList.add(common.loadingClass);

    try
    {
        const response = await fetch(`api/components/promotionGroups/searchRelatedProducts`,
        {
            method: "POST",
            headers: {
                "Content-type": "application/json",
                "Accept": "application/html"
            },
            body: JSON.stringify(searchData)
        });

        if (!response.ok) return;

        const data = await response.text();

        const addRelatedProductSearchResultsContainer = document.getElementById(common.addRelatedProductSearchResultsContainerId);

        addRelatedProductSearchResultsContainer.innerHTML = data;

        const relatedProductSearchResultsContainer = document.getElementById(common.addRelatedProductSearchResultsContainerId);

        const relatedProductSearchResultsTable = document.getElementById(common.relatedProductSearchResultsTableId);

        const relatedProductSearchResultElements
            = relatedProductSearchResultsTable.querySelectorAll(`[name='${common.relatedProductSearchResultName}']`);

        changeStyleOfSearchResultsForAlreadyAddedProducts(relatedProductSearchResultElements, existingRelatedProductIds);

        addEventListenersToSearchResults(
            relatedProductSearchResultsContainer,
            relatedProductSearchResultsTable,
            relatedProductSearchResultElements);
    }
    finally {

        relatedProductSearchButton.classList.remove(common.loadingClass);
    }
}

function getRelatedProductsSearchOptionsFromCurrentData() {

    const relatedProductManufacturerSelect = document.getElementById(common.relatedProductManufacturerSelectId);
    const relatedProductSearchInput = document.getElementById(common.relatedProductSearchInputId);
    const relatedProductSearchAvaliableOnlyCheckbox = document.getElementById(common.relatedProductSearchAvaliableOnlyCheckboxId);

    const manufacturerId = common.getIntegerOrNullFromString(relatedProductManufacturerSelect.value);

    return {
        ManufacturerId: manufacturerId,
        SearchData: relatedProductSearchInput.value,
        AvailableOnly: relatedProductSearchAvaliableOnlyCheckbox.checked,
    };
}

function changeStyleOfSearchResultsForAlreadyAddedProducts(relatedProductSearchResultElements, alreadyAddedProductIds) {

    const alreadyAddedProductIdsCopy = [...alreadyAddedProductIds];

    for (const relatedProductSearchResultElement of relatedProductSearchResultElements) {

        if (alreadyAddedProductIdsCopy.length === 0) break;

        const productIdAttribute = relatedProductSearchResultElement.getAttribute(common.promotionRelatedSearchResultProductIdAttribute);

        const searchResultProductId = common.getIntegerOrNullFromString(productIdAttribute);

        for (let i = 0; i < alreadyAddedProductIdsCopy.length; i++) {

            const alreadyAddedProductId = alreadyAddedProductIdsCopy[i];

            if (searchResultProductId !== alreadyAddedProductId) continue;

            relatedProductSearchResultElement.classList.add(relatedProductSearchResultInactiveClass);

            alreadyAddedProductIdsCopy.splice(i, 1);

            break;
        }
    }
}

export function setExistingRelatedProductIdsFromCurrentData() {

    existingRelatedProductIds = getExistingRelatedProductIds();
}

function getExistingRelatedProductIds()
{
    const relatedProductIds = [];

    const promotionEditRelatedProductsTable = document.getElementById(common.promotionEditRelatedProductsTableId);

    const relatedProductElements = promotionEditRelatedProductsTable.querySelectorAll("tr");

    for (const relatedProductElement of relatedProductElements) {

        const relatedProductIdAsString = relatedProductElement.getAttribute(common.promotionRelatedProductIdAttribute);

        const relatedProductId = common.getIntegerOrNullFromString(relatedProductIdAsString);

        relatedProductIds.push(relatedProductId);
    }

    return relatedProductIds;
}

async function onSelectAllButtonClicked() {

    const relatedProductsSelectAllButtonText = document.getElementById(relatedProductsSelectAllButtonTextId);

    if (relatedProductsSelectAllButtonText.innerText === "Undo Changes") {

        undoSelectAll();

        relatedProductsSelectAllButtonText.innerText = "Select All";

        return;
    }

    await selectAllCurrentSearchResults();

    relatedProductsSelectAllButtonText.innerText = "Undo Changes";
}

function addEventListenersToSearchResults(
    relatedProductSearchResultsTableContainer,
    relatedProductSearchResultsTable,
    relatedProductSearchResultElements) {

    relatedProductSearchResultsTableContainer.addEventListener("wheel", e => {

        e.preventDefault();

        const step = 40;

        relatedProductSearchResultsTableContainer.scrollTop += Math.sign(e.deltaY) * step;
    }, { passive: false });

    relatedProductSearchResultsTable.addEventListener('mousedown', (e) => {

        if (e.button === 0) {

            mouseX = e.clientX;
            mouseY = e.clientY;

            currentSearchResultElement = getProductRelatedSearchResultAtLocation(mouseX, mouseY);

            startSearchResultElement = currentSearchResultElement;

            rangeSelectionStartIndex = getRelatedProductSearchResultIndex(currentSearchResultElement);

            elementIndexesThatWereHandledThisSelection.length = 0;
            elementsThatWereHandledThisSelection.length = 0;

            isHolding = true;
        }
    });

    document.addEventListener('mouseup', (e) => {

        if (isHolding && e.button === 0) {

            saveChangesFromSelection();

            isHolding = false;
        }
    });

    relatedProductSearchResultsTable.addEventListener('pointermove', (e) => {

        if (!isHolding) return;

        mouseX = e.clientX;
        mouseY = e.clientY;

        addSearchResultItemsOnMouseChanges();
    });


    relatedProductSearchResultsTableContainer.addEventListener('scroll', () => {

        if (!isHolding) return;

        addSearchResultItemsOnMouseChanges();
    });

    for (const searchResult of relatedProductSearchResultElements) {

        searchResult.addEventListener("click", () => toggleRelatedProductSelectionFromElement(searchResult));
    }
}

async function saveChangesFromSelection() {

    if (elementsThatWereHandledThisSelection.length == 0) return;

    rangeSelectionStartIndex = -1;
    startSearchResultElement = null;
    currentSearchResultElement = null;

    elementIndexesThatWereHandledThisSelection.length = 0;

    await updateRelatedProductsSelection(elementsThatWereHandledThisSelection);

    for (const searchResult of elementsThatWereHandledThisSelection) {

        searchResult.classList.remove(relatedProductSearchResultInSelectionClass);
    }

    elementsThatWereHandledThisSelection.length = 0;
}

function addSearchResultItemsOnMouseChanges() {

    const newSearchResultElement = getProductRelatedSearchResultAtLocation(mouseX, mouseY);

    if (!newSearchResultElement) {

        return;
    }

    if (newSearchResultElement === currentSearchResultElement) {

        if (newSearchResultElement === startSearchResultElement
            && !elementIndexesThatWereHandledThisSelection.includes(rangeSelectionStartIndex)) {

            toggleRelatedProductSelectionWithoutChangingDataFromElement(newSearchResultElement);

            elementIndexesThatWereHandledThisSelection.push(rangeSelectionStartIndex);
            elementsThatWereHandledThisSelection.push(newSearchResultElement);

            newSearchResultElement.classList.add(relatedProductSearchResultInSelectionClass);
        }

        return;
    }

    currentSearchResultElement = newSearchResultElement;

    const relatedProductSearchResultsTable = document.getElementById(common.relatedProductSearchResultsTableId);

    const relatedProductSearchResultElements = [...relatedProductSearchResultsTable.querySelectorAll(`[name='${common.relatedProductSearchResultName}']`)];

    const newElementIndex = relatedProductSearchResultElements.indexOf(currentSearchResultElement);

    const min = Math.min(rangeSelectionStartIndex, newElementIndex);
    const max = Math.max(rangeSelectionStartIndex, newElementIndex);

    for (let i = min; i <= max; i++) {

        if (elementIndexesThatWereHandledThisSelection.includes(i)) continue;

        const elementToChangeSelection = relatedProductSearchResultElements[i];

        toggleRelatedProductSelectionWithoutChangingDataFromElement(elementToChangeSelection);

        elementToChangeSelection.classList.add(relatedProductSearchResultInSelectionClass);

        elementIndexesThatWereHandledThisSelection.push(i);
        elementsThatWereHandledThisSelection.push(elementToChangeSelection);
    }
}

function getProductRelatedSearchResultAtLocation(x, y) {

    const currentTouchedElement = document.elementFromPoint(x, y);

    return currentTouchedElement?.closest(`[name='${common.relatedProductSearchResultName}']`);
}

function getRelatedProductSearchResultIndex(searchResultElement) {

    const relatedProductSearchResultsTable = document.getElementById(common.relatedProductSearchResultsTableId);

    const relatedProductSearchResultElements = [...relatedProductSearchResultsTable.querySelectorAll(`[name='${common.relatedProductSearchResultName}']`)];

    return relatedProductSearchResultElements.indexOf(searchResultElement);
}

export async function toggleRelatedProductSelectionFromElement(searchResult) {

    const productIdAttribute = searchResult.getAttribute(common.promotionRelatedSearchResultProductIdAttribute);

    const searchResultProductId = common.getIntegerOrNullFromString(productIdAttribute);

    await toggleRelatedProductSelection(searchResultProductId, searchResult);
}

async function toggleRelatedProductSelection(productId, relatedProductSearchResultElement) {

    const promotionEditRelatedProductsTable = document.getElementById(common.promotionEditRelatedProductsTableId);

    const relatedProductElements = promotionEditRelatedProductsTable.querySelectorAll("tr");

    for (const relatedProductElement of relatedProductElements) {

        const relatedProductIdAsString = relatedProductElement.getAttribute(common.promotionRelatedProductIdAttribute);

        const relatedProductId = common.getIntegerOrNullFromString(relatedProductIdAsString);

        if (relatedProductId === productId)
        {
            promotionEditorRelatedProducts.removeItem(relatedProductElement);

            removeFromExistingProductIds(productId);

            if (relatedProductSearchResultElement != null) {

                relatedProductSearchResultElement.classList.remove(relatedProductSearchResultInactiveClass);
            } 

            return;
        }
    }

    existingRelatedProductIds.push(productId);

    const response = await fetch(`api/components/promotionGroups/relatedProduct/${productId}`,
    {
        method: "GET",
        headers: {
            "Accept": "application/html"
        }
    });

    if (!response.ok) return;

    const data = await response.text();

    const promotionEditRelatedProductsTableBody = promotionEditRelatedProductsTable.querySelector('tbody');

    promotionEditRelatedProductsTableBody.insertAdjacentHTML("beforeend", data);

    promotionEditorRelatedProducts.attachEvents(promotionEditRelatedProductsTableBody.lastElementChild);

    if (relatedProductSearchResultElement != null) {

        relatedProductSearchResultElement.classList.add(relatedProductSearchResultInactiveClass);
    }
}

async function updateRelatedProductsSelection(relatedProductSearchResultElements) {

    const productSearchResultIdsInPromotion = [];
    const productSearchResultIdsNotInPromotion = [];

    for (const searchResult of relatedProductSearchResultElements) {

        let isIdPartOfPromotion = false;

        const productIdAttribute = searchResult.getAttribute(common.promotionRelatedSearchResultProductIdAttribute);

        const searchResultProductId = common.getIntegerOrNullFromString(productIdAttribute);

        for (const existingProductId of existingRelatedProductIds) {

            if (searchResultProductId !== existingProductId) continue;

            isIdPartOfPromotion = true;

            break;
        }

        if (isIdPartOfPromotion) {

            productSearchResultIdsInPromotion.push(searchResultProductId);
        }
        else {

            productSearchResultIdsNotInPromotion.push(searchResultProductId);
        }
    }

    const promotionEditRelatedProductsTable = document.getElementById(common.promotionEditRelatedProductsTableId);

    const relatedProductElements = promotionEditRelatedProductsTable.querySelectorAll("tr");

    for (const relatedProductElement of relatedProductElements) {

        const relatedProductIdAsString = relatedProductElement.getAttribute(common.promotionRelatedProductIdAttribute);

        const relatedProductId = common.getIntegerOrNullFromString(relatedProductIdAsString);

        if (!productSearchResultIdsNotInPromotion.includes(relatedProductId)) continue;

        promotionEditorRelatedProducts.removeItem(relatedProductElement);

        relatedProductElement.remove();
    }
   
    await addRelatedProductsToPromotionEditor(promotionEditRelatedProductsTable, productSearchResultIdsInPromotion);
}

async function addRelatedProductsToPromotionEditor(promotionEditRelatedProductsTable, productIds) {

    const response = await fetch(`api/components/promotionGroups/relatedProduct`,
    {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Accept": "application/html"
        },
        body: JSON.stringify(productIds)
    });

    if (!response.ok) return;

    const data = await response.text();

    const promotionEditRelatedProductsTableBody = promotionEditRelatedProductsTable.querySelector('tbody');

    const promotionEditRelatedProductsTableBodyLastChild = promotionEditRelatedProductsTableBody.lastElementChild;

    promotionEditRelatedProductsTableBody.insertAdjacentHTML("beforeend", data);

    let newRelatedProductElement;

    if (promotionEditRelatedProductsTableBodyLastChild) {

        newRelatedProductElement = promotionEditRelatedProductsTableBodyLastChild.nextElementSibling;
    }
    else {

        newRelatedProductElement = promotionEditRelatedProductsTableBody.firstElementChild;
    }

    while (newRelatedProductElement) {

        promotionEditorRelatedProducts.attachEvents(newRelatedProductElement);

        newRelatedProductElement = newRelatedProductElement.nextElementSibling;
    }
}

async function toggleRelatedProductSelectionWithoutChangingDataFromElement(elementToChangeSelection) {

    const searchResultProductId = getSearchResultProductId(elementToChangeSelection);

    await toggleRelatedProductSelectionWithoutChangingData(searchResultProductId, elementToChangeSelection);
}

async function toggleRelatedProductSelectionWithoutChangingData(productId, relatedProductSearchResultElement) {

    for (const relatedProductId of existingRelatedProductIds) {

        if (relatedProductId === productId)
        {
            deselectSearchResult(relatedProductSearchResultElement, productId);

            return;
        }
    }

    selectSearchResult(relatedProductSearchResultElement, productId);
}

function selectSearchResult(searchResult, productId) {

    existingRelatedProductIds.push(productId);

    searchResult.classList.add(relatedProductSearchResultInactiveClass);
}

function deselectSearchResult(searchResult, productId) {

    removeFromExistingProductIds(productId);

    searchResult.classList.remove(relatedProductSearchResultInactiveClass);
}

async function selectAllCurrentSearchResults() {

    const relatedProductSearchResultsTable = document.getElementById(common.relatedProductSearchResultsTableId);

    if (!relatedProductSearchResultsTable) return;

    const relatedProductSearchResultElements
        = [...relatedProductSearchResultsTable.querySelectorAll(`[name='${common.relatedProductSearchResultName}']`)];

    if (relatedProductSearchResultElements.length == 0) return;

    const addedRelatedProductIds = [];

    for (const searchResult of relatedProductSearchResultElements) {

        const searchResultProductId = getSearchResultProductId(searchResult);

        if (existingRelatedProductIds.includes(searchResultProductId)) continue;

        selectSearchResult(searchResult, searchResultProductId);

        addedRelatedProductIds.push(searchResultProductId);
        addedRelatedProductSearchResultsInSelectAll.push(searchResult);
    }

    const promotionEditRelatedProductsTable = document.getElementById(common.promotionEditRelatedProductsTableId);

    await addRelatedProductsToPromotionEditor(promotionEditRelatedProductsTable, addedRelatedProductIds);
}

function undoSelectAll() {

    const promotionEditRelatedProductsTable = document.getElementById(common.promotionEditRelatedProductsTableId);

    const relatedProductElements = promotionEditRelatedProductsTable.querySelectorAll("tr");

    const relatedProductElementsToIdsMap = {};

    for (const relatedProductElement of relatedProductElements) {

        const relatedProductIdAsString = relatedProductElement.getAttribute(common.promotionRelatedProductIdAttribute);

        const relatedProductId = common.getIntegerOrNullFromString(relatedProductIdAsString);

        relatedProductElementsToIdsMap[relatedProductId] = relatedProductElement;
    }

    for (const searchResult of addedRelatedProductSearchResultsInSelectAll) {

        const searchResultProductId = getSearchResultProductId(searchResult);

        deselectSearchResult(searchResult, searchResultProductId);

        const relatedProductElement = relatedProductElementsToIdsMap[searchResultProductId];

        if (relatedProductElement) {

            promotionEditorRelatedProducts.removeItem(relatedProductElement);
        }
    }
}

function getSearchResultProductId(searchResultElement) {

    const productIdAttribute = searchResultElement.getAttribute(common.promotionRelatedSearchResultProductIdAttribute);

    return common.getIntegerOrNullFromString(productIdAttribute);
}
