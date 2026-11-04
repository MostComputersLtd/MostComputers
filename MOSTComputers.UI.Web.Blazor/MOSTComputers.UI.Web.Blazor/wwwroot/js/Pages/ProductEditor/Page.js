import * as fetchWithAuthRedirect from "../../Authentication/FetchWithAuthRedirect.js";
import * as conversionUtils from "../../Common/ConversionUtils.js"
import * as tableOrdering from "./TableOrdering.js";
import * as productEditorImageFilePopup from "./ProductEditorImageFilePopup.js";
import * as promotionFilePopup from "./SingleProductEditorPopup/PromotionFilePopup.js";
import * as singleProductEditorPopup from "./SingleProductEditorPopup/SingleProductPopupEditor.js";

const productNewStatusSelectId = "productNewStatusSelect";
const userInputId = "userInput";
const searchButtonId = "productEditorSearchButton";
const categorySelectId = "categorySelect";
const manufacturerSelectId = "manufacturerSelect";
const productStatusSelectId = "productStatusSelect";
const promotionStatusSelectId = "promotionStatusSelect";
const resultsCountSelectId = "resultsCountSelect";

const itemsTableId = "productEditorItemsTable";

const itemsTablePlShowColumnHeaderOrderTextId = "productEditorItemsTablePlShowColumnHeaderOrderText";
const itemsTableProductWorkStatusColumnHeaderOrderTextId = "productEditorItemsTableProductWorkStatusColumnHeaderOrderText";
const itemsTableHasImagesColumnHeaderOrderTextId = "productEditorItemsTableHasImagesColumnHeaderOrderText";

const productXmlPopupContainerId = "productXmlPopupContainerId";
const productPromotionPopupContainerId = "productPromotionPopupContainer";
const productInfoPromotionPopupContainerId = "productInfoPromotionPopupContainer";
const productImagesPopupContainerId = "productImagesPopupContainer";
const productImageFilesPopupContainerId = "productImageFilesPopupContainer";
const productPropertiesPopupContainerId = "productPropertiesPopupContainer";
const promotionProductFilePopupContainerId = "promotionProductFilePopupContainer";
const productSearchStringPopupContainerId = "productSearchStringPopupContainer";
const singleProductEditorPopupContainerId = "singleProductEditorPopupContainer";

const singleProductEditorPopupJsonDataScriptId = "singleEditorData";

const itemName = "productEditorItem";

const itemPlShowTextName = "productEditorItemPlShowText"
const itemImagesCountTextName = "productEditorItemOriginalImagesCountText"
const itemProductIdTextName = "productEditorItemProductIdText";
const itemProductNameTextName = "productEditorItemProductNameText";
const itemProductSearchStringPartCountTextName = "productEditorItemProductSearchStringPartCountText";
const itemProductPidPromotionTextName = "productEditorItemProductPidPromotionText";
const itemProductRidPromotionTextName = "productEditorItemProductRidPromotionText";
const itemProductInfoPromotionTextName = "productEditorItemProductInfoPromotionText";
const itemProductPromotionFilesCountTextName = "productEditorItemProductPromotionFilesCountText";
const itemProductPropertiesCountTextName = "productEditorItemProductPropertiesCountText";
const itemProductImageFilesCountTextName = "productEditorItemProductImageFilesCountText";
const itemProductImagesCountTextName = "productEditorItemProductImagesCountText";

const imageFileButtonName = "imageFileButton";

const searchSelectDefaultOptionAttribute = "data-default-option";

const itemProductIdAttribute = "data-product-id";
const itemCategoryIdAttribute = "data-category-id";
const itemDisplayOrderAttribute = "data-display-order";

const itemPlShowTextIsProductHiddenAttribute = "data-is-product-hidden";
const itemImagesCountTextHasImagesAttribute = "data-has-images";
const itemTextHasLinkAttribute = "data-has-link";
const itemPromotionTextPromotionIdAttribute = "data-promotion-id";

const loadingClass = "loading";

const OrderingType = {
    ByProductId: "byProductId",
    ByPlShow: "byPlShow",
    ByImagesCount: "byImagesCount",
};

const ProductIdOrderingOption = {
    Largest: "largest",
    Smallest: "smallest",
}

const PlShowOrderingOption = {
    Hidden: "hidden",
}

const ImagesCountOrderingOption = {
    WithImages: "withImages",
    WithoutImages: "withoutImages",
}

const orderingDefinitions = new Map([
    [OrderingType.ByProductId, new Set([ProductIdOrderingOption.Largest, ProductIdOrderingOption.Smallest])],
    [OrderingType.ByPlShow, new Set([PlShowOrderingOption.Hidden])],
    [OrderingType.ByImagesCount, new Set([ImagesCountOrderingOption.WithImages, ImagesCountOrderingOption.WithoutImages])]
]);

let itemsTableOrderingState = null;

if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", initEvents);
}
else {
    initEvents();
}

function initEvents() {

    window.addEventListener("keydown", searchProductsOnEnterKeyDown);

    console.log("Attached");

    Blazor.addEventListener("enhancedload", removeKeyDownEvent);
    window.addEventListener("beforeunload", removeKeyDownEvent);

    const searchButton = document.getElementById(searchButtonId);

    const categorySelect = document.getElementById(categorySelectId);

    searchButton.addEventListener("click", searchProducts);

    categorySelect.addEventListener("change", onCategorySelectChanged);
}

function removeKeyDownEvent() {

    window.removeEventListener("keydown", searchProductsOnEnterKeyDown);

    console.log("Removed");
}

function initItemEvents(item) {

    const itemProductIdText = item.querySelector(`[name="${itemProductIdTextName}"]`);
    const itemProductNameText = item.querySelector(`[name="${itemProductNameTextName}"]`);
    const itemProductSearchStringPartCountText = item.querySelector(`[name="${itemProductSearchStringPartCountTextName}"]`);
    const itemProductPidPromotionText = item.querySelector(`[name="${itemProductPidPromotionTextName}"]`);
    const itemProductRidPromotionText = item.querySelector(`[name="${itemProductRidPromotionTextName}"]`);
    const itemProductInfoPromotionText = item.querySelector(`[name="${itemProductInfoPromotionTextName}"]`);
    const itemProductPromotionFilesCountText = item.querySelector(`[name="${itemProductPromotionFilesCountTextName}"]`);
    const itemProductPropertiesCountText = item.querySelector(`[name="${itemProductPropertiesCountTextName}"]`);
    const itemProductImageFilesCountText = item.querySelector(`[name="${itemProductImageFilesCountTextName}"]`);
    const itemProductImagesCountText = item.querySelector(`[name="${itemProductImagesCountTextName}"]`);

    itemProductIdText.addEventListener("click", showProductXmlPopup);
    itemProductNameText.addEventListener("click", showSingleProductEditorPopup);
    itemProductSearchStringPartCountText.addEventListener("click", showProductSearchStringPopup);

    const isPidPromotionTextLinkActive = itemProductPidPromotionText.getAttribute(itemTextHasLinkAttribute);

    if (isPidPromotionTextLinkActive) {
        itemProductPidPromotionText.addEventListener("click", showProductPromotionPopup);
    }

    const isRidPromotionTextLinkActive = itemProductRidPromotionText.getAttribute(itemTextHasLinkAttribute);

    if (isRidPromotionTextLinkActive) {
        itemProductRidPromotionText.addEventListener("click", showProductPromotionPopup);
    }

    const isProductInfoPromotionTextLinkActive = itemProductInfoPromotionText.getAttribute(itemTextHasLinkAttribute);

    if (isProductInfoPromotionTextLinkActive) {
        itemProductInfoPromotionText.addEventListener("click", showProductInfoPromotionPopup);
    }

    itemProductPromotionFilesCountText.addEventListener("click", showPromotionProductFilePopup);
    itemProductPropertiesCountText.addEventListener("click", showProductPropertiesPopup);
    itemProductImageFilesCountText.addEventListener("click", showProductImageFilesPopup);
    itemProductImagesCountText.addEventListener("click", showProductImagesPopup);
}

async function onCategorySelectChanged(e) {

    const value = e.currentTarget.value;

    const categoryId = conversionUtils.getNumberOrNullFromString(value);

    const url = `api/components/productEditor/manufacturersInCategory?categoryId=${categoryId}`;

    const response = await fetch(url, {
        method: "GET",
        headers: {
            "Accept": "application/json"
        }
    });

    if (!response.ok) return;

    const data = await response.json();

    const manufacturerSelect = document.getElementById(manufacturerSelectId);

    const selectedManufacturerId = conversionUtils.getNumberOrNullFromString(manufacturerSelect.value);

    for (const option of manufacturerSelect.options) {

        if (!option.hasAttribute(searchSelectDefaultOptionAttribute)) {
            option.remove();
        }
    }

    for (const optionData of data) {

        const option = document.createElement("option");

        option.value = optionData.manufacturerId;
        option.text = optionData.manufacturerName;

        if (selectedManufacturerId == optionData.manufacturerId) {

            option.setAttribute("selected");
        }

        manufacturerSelect.add(option);
    }
}

async function searchProductsOnEnterKeyDown(e) {

    if (e.key !== 'Enter' || e.isComposing) return;

    console.log("EVENT");

    const searchButton = document.getElementById(searchButtonId);

    const element = document.activeElement;

    if (!element) return;

    const elementTag = element.tagName.toLowerCase();

    const nativeInteractiveElements = ['button', 'textarea', 'select', 'a'];
    const nativeInteractiveInputTypes = ['button', 'submit', 'checkbox', 'radio'];

    const isNativeInteractiveElement = nativeInteractiveElements.includes(elementTag);
    let isNativeInteractiveInput = false;

    if (elementTag === 'input') {

        const elementType = (element.getAttribute('type') || '').toLowerCase();

        isNativeInteractiveInput = nativeInteractiveInputTypes.includes(elementType);
    }

    const isContentEditable = element.isContentEditable;
    const isRoleButton = element.getAttribute('role') === 'button';

    if (isNativeInteractiveElement || isNativeInteractiveInput || isContentEditable || isRoleButton) {
        return;
    }

    const isThereAnyOpenDialog = Array.from(document.querySelectorAll('dialog')).some(
        dialogElement => dialogElement.open
    );

    if (isThereAnyOpenDialog) {
        return;
    }

    e.preventDefault();

    console.log("Prevented default");

    searchButton.focus();

    await searchProducts();
}

async function searchProducts() {

    const searchButton = document.getElementById(searchButtonId);

    const productNewStatusSelect = document.getElementById(productNewStatusSelectId);
    const userInput = document.getElementById(userInputId);
    const categorySelect = document.getElementById(categorySelectId);
    const manufacturerSelect = document.getElementById(manufacturerSelectId);
    const productStatusSelect = document.getElementById(productStatusSelectId);
    const promotionStatusSelect = document.getElementById(promotionStatusSelectId);
    const resultsCountSelect = document.getElementById(resultsCountSelectId);

    const productNewStatus = conversionUtils.getNumberOrNullFromString(productNewStatusSelect.value);
    const categoryId = conversionUtils.getNumberOrNullFromString(categorySelect.value);
    const manufacturerId = conversionUtils.getNumberOrNullFromString(manufacturerSelect.value);
    const productStatus = conversionUtils.getNumberOrNullFromString(productStatusSelect.value);
    const promotionStatus = conversionUtils.getNumberOrNullFromString(promotionStatusSelect.value);
    const resultsCount = conversionUtils.getNumberOrNullFromString(resultsCountSelect.value);

    const body = {
        productNewStatus: productNewStatus,
        userInput: userInput.value,
        categoryId: categoryId,
        manufacturerId: manufacturerId,
        productStatus: productStatus,
        promotionStatus: promotionStatus,
        resultsCount: resultsCount,
    };

    searchButton.classList.add(loadingClass);

    let response;

    try {
        const url = `api/components/productEditor/search`;

        response = await fetch(url, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Accept": "text/html"
            },
            body: JSON.stringify(body)
        });
    }
    finally {
        searchButton.classList.remove(loadingClass);
    } 

    if (response == null) return;

    if (!response.ok) return;

    const data = await response.text();

    const itemsTable = document.getElementById(itemsTableId);

    const itemsTableBody = itemsTable.querySelector("tbody");

    itemsTableBody.innerHTML = data;

    const items = itemsTableBody.querySelectorAll(`[name="${itemName}"]`);

    for (const item of items) {

        initItemEvents(item);
    }

    itemsTableOrderingState = new tableOrdering.SingleActiveTypeOrderingState(orderingDefinitions, null);
}

function orderExistingProducts(orderType, orderOption) {

    if (itemsTableOrderingState.activeOrder != null) {

        clearItemsTableOldOrderVisuals(itemsTableOrderingState.activeOrder.type);
    }

    const itemsTable = document.getElementById(itemsTableId);

    const itemsTableBody = itemsTable.querySelector("tbody");

    const items = Array.from(itemsTableBody.children);

    sortByOptions(items, orderType, orderOption);

    items.forEach(item => itemsTableBody.appendChild(item));

    addItemsTableNewOrderVisuals(orderType, orderOption);
}

function clearItemsTableOldOrderVisuals(orderType) {

    switch (orderType) {

        case OrderingType.ByProductId:

            const itemsTableProductWorkStatusColumnHeaderOrderText = document.getElementById(itemsTableProductWorkStatusColumnHeaderOrderTextId);

            itemsTableProductWorkStatusColumnHeaderOrderText.textContent = "•";
            break;

        case OrderingType.ByPlShow:

            const itemsTablePlShowColumnHeaderOrderText = document.getElementById(itemsTablePlShowColumnHeaderOrderTextId);

            itemsTablePlShowColumnHeaderOrderText.textContent = "•";
            break;

        case OrderingType.ByImagesCount:

            const itemsTableHasImagesColumnHeaderOrderText = document.getElementById(itemsTableHasImagesColumnHeaderOrderTextId);

            itemsTableHasImagesColumnHeaderOrderText.textContent = "•";
            break;

        default:
            break;
    }
}

function addItemsTableNewOrderVisuals(orderType, orderOption) {

    switch (orderType) {

        case OrderingType.ByProductId:

            const itemsTableProductWorkStatusColumnHeaderOrderText = document.getElementById(itemsTableProductWorkStatusColumnHeaderOrderTextId);

            itemsTableProductWorkStatusColumnHeaderOrderText.textContent = "▼";
            break;

        case OrderingType.ByPlShow:

            const itemsTablePlShowColumnHeaderOrderText = document.getElementById(itemsTablePlShowColumnHeaderOrderTextId);

            itemsTablePlShowColumnHeaderOrderText.textContent = "▼";
            break;

        case OrderingType.ByImagesCount:

            const itemsTableHasImagesColumnHeaderOrderText = document.getElementById(itemsTableHasImagesColumnHeaderOrderTextId);

            if (orderOption === ImagesCountOrderingOption.WithImages) {
                itemsTableHasImagesColumnHeaderOrderText.textContent = "▲";
            }
            else if (orderOption === ImagesCountOrderingOption.WithoutImages) {
                itemsTableHasImagesColumnHeaderOrderText.textContent = "▼";
            }

            break;

        default:
            break;
    }
}

function sortByOptions(arr, type, option) {

    switch (type) {
        case OrderingType.ByProductId:

            arr.sort(sortByProductId);
            break;

        case OrderingType.ByPlShow:

            arr.sort((a, b) => sortByPlShow(a, b, option) || sortByDefaultOrder(a, b,/ ));
            break;

        case OrderingType.ByImagesCount:

            arr.sort((a, b) => sortByImagesCount(a, b, option) || sortByDefaultOrder(a, b,/ ));
            break;

        default:
            break;
    }
}

function sortByOrders(arr, sorts) {

    arr.sort((a, b) => {

        for (const sort of sorts) {

            const orderValue = sort(a, b);

            if (orderValue != null) {

                return orderValue;
            }
        }
    });
}

function sortByPlShow(a, b, option) {

    if (option == PlShowOrderingOption.Hidden) {

        const firstItemPlShowText = a.querySelector($`[name='${itemPlShowTextName}']`);
        const secondItemPlShowText = b.querySelector($`[name='${itemPlShowTextName}']`);

        const isFirstHidden = firstItemPlShowText.getAttribute(itemPlShowTextIsProductHiddenAttribute) === "true";
        const isSecondHidden = secondItemPlShowText.getAttribute(itemPlShowTextIsProductHiddenAttribute) === "true";

        if (isFirstHidden === isSecondHidden) return 0;

        return isFirstHidden ? -1 : 1;
    }
}

function sortByProductId(a, b) {

    const firstProductId = a.getAttribute(itemProductIdAttribute);
    const secondProductId = b.getAttribute(itemProductIdAttribute);

    return conversionUtils.getNumberOrNullFromString(secondProductId) - conversionUtils.getNumberOrNullFromString(firstProductId);
}

function sortByImagesCount(a, b, option) {

    const firstItemImagesCountText = a.querySelector($`[name='${itemImagesCountTextName}']`);
    const secondItemImagesCountText = b.querySelector($`[name='${itemImagesCountTextName}']`);

    const firstHasImages = firstItemImagesCountText.getAttribute(itemImagesCountTextHasImagesAttribute) === "true";
    const secondHasImages = secondItemImagesCountText.getAttribute(itemImagesCountTextHasImagesAttribute) === "true";

    if (firstHasImages === secondHasImages) return 0;

    if (option === ImagesCountOrderingOption.WithImages) {
        return firstHasImages ? -1 : 1;
    }

    return firstHasImages ? 1 : -1;
}

function sortByDefaultOrder(a, b, orderByCategory) {

    if (orderByCategory) {

        const firstCategoryIdAttribute = a.getAttribute(itemCategoryIdAttribute);
        const secondCategoryIdAttribute = b.getAttribute(itemCategoryIdAttribute);

        const firstCategoryId = parseInt(firstCategoryIdAttribute);
        const secondCategoryId = parseInt(secondCategoryIdAttribute);

        const firstCategoryIdIsNaN = isNaN(firstDisplayOrder);

        if (firstCategoryIdIsNaN !== isNaN(secondDisplayOrder)) {

            return firstCategoryIdIsNaN ? 1 : -1;
        }
        else if (!firstCategoryIdIsNaN) {

            const firstCategoryIdNotDefault = firstCategoryId > 1;
            const secondCategoryIdNotDefault = secondCategoryId > 1;

            if (firstCategoryIdNotDefault != secondCategoryIdNotDefault) {

                return firstCategoryIdNotDefault ? 1 : -1;
            }

            const orderByCategory = firstCategoryId - secondCategoryId;

            if (orderByCategory != 0) return orderByCategory;
        }
    }

    const firstDisplayOrder = a.getAttribute(itemDisplayOrderAttribute);
    const secondDisplayOrder = b.getAttribute(itemDisplayOrderAttribute);

    const firstDisplayOrderIsNaN = isNaN(firstDisplayOrder);

    if (firstDisplayOrderIsNaN !== isNaN(secondDisplayOrder)) {

        return firstDisplayOrderIsNaN ? 1 : -1;
    }
    else if (!firstDisplayOrderIsNaN) {

        const orderByDisplayOrder = firstDisplayOrder - secondDisplayOrder;

        if (orderByDisplayOrder !== 0) return orderByDisplayOrder;
    }

    return sortByProductId(a, b);
}

async function showProductXmlPopup(e) {

    const productId = getProductIdFromItem(e.currentTarget);
    
    const url = `api/components/productEditor/productXmlDataPopup/${productId}`;

    const response = await fetch(url, {
        method: "GET",
    });

    if (!response.ok) return;

    const data = await response.text();

    const productXmlPopupContainer = document.getElementById(productXmlPopupContainerId);

    productXmlPopupContainer.innerHTML = data;

    const dialog = productXmlPopupContainer.querySelector("dialog");

    dialog.showModal();
}

async function showProductPromotionPopup() {

    const productId = getProductIdFromItem(e.currentTarget);

    const promotionId = e.currentTarget.getAttribute(itemPromotionTextPromotionIdAttribute);
    
    const url = `api/components/productEditor/productPromotionPopup/${productId}/${promotionId}`;

    const response = await fetch(url, {
        method: "GET",
    });

    if (!response.ok) return;

    const data = await response.text();

    const productPromotionPopupContainer = document.getElementById(productPromotionPopupContainerId);

    productPromotionPopupContainer.innerHTML = data;

    const dialog = productPromotionPopupContainer.querySelector("dialog");

    dialog.showModal();
}

async function showProductInfoPromotionPopup(e) {
    
    const productId = getProductIdFromItem(e.currentTarget);

    const url = `api/components/productEditor/productInfoPromotionPopup/${productId}`;

    const response = await fetch(url, {
        method: "GET",
    });

    if (!response.ok) return;

    const data = await response.text();

    const productInfoPromotionPopupContainer = document.getElementById(productInfoPromotionPopupContainerId);

    productInfoPromotionPopupContainer.innerHTML = data;

    const dialog = productInfoPromotionPopupContainer.querySelector("dialog");

    dialog.showModal();
}

async function showProductImagesPopup(e) {
    
    const productId = getProductIdFromItem(e.currentTarget);

    const url = `api/components/productEditor/productImagesPopup/${productId}`;

    const response = await fetch(url, {
        method: "GET",
    });

    if (!response.ok) return;

    const data = await response.text();

    const productImagesPopupContainer = document.getElementById(productImagesPopupContainerId);

    productImagesPopupContainer.innerHTML = data;

    const dialog = productImagesPopupContainer.querySelector("dialog");

    dialog.showModal();
}

async function showProductImageFilesPopup(e) {

    const productId = getProductIdFromItem(e.currentTarget);

    const url = `api/components/productEditor/productImageFilesPopup/${productId}`;

    const response = await fetch(url, {
        method: "GET",
    });

    if (!response.ok) return;

    const data = await response.text();

    const productImageFilesPopupContainer = document.getElementById(productImageFilesPopupContainerId);

    productImageFilesPopupContainer.innerHTML = data;

    const imageFileButtons = [...productImageFilesPopupContainer.querySelectorAll($`[name='${imageFileButtonName}']`)];

    for (const imageFileButton of imageFileButtons) {

        imageFileButton.addEventListener("click", productEditorImageFilePopup.imageDataButtonOpenRoute);
    }

    const dialog = productImageFilesPopupContainer.querySelector("dialog");

    dialog.showModal();
}

async function showProductPropertiesPopup(e) {

    const productId = getProductIdFromItem(e.currentTarget);

    const url = `api/components/productEditor/productPropertiesPopup/${productId}`;

    const response = await fetch(url, {
        method: "GET",
    });

    if (!response.ok) return;

    const data = await response.text();

    const productPropertiesPopupContainer = document.getElementById(productPropertiesPopupContainerId);

    productPropertiesPopupContainer.innerHTML = data;

    const dialog = productPropertiesPopupContainer.querySelector("dialog");

    dialog.showModal();
}

async function showProductSearchStringPopup(e) {

    const productId = getProductIdFromItem(e.currentTarget);

    const url = `api/components/productEditor/productSearchStringPopup/${productId}`;

    const response = await fetch(url, {
        method: "GET",
    });

    if (!response.ok) return;

    const data = await response.text();

    const productSearchStringPopupContainer = document.getElementById(productSearchStringPopupContainerId);

    productSearchStringPopupContainer.innerHTML = data;

    const dialog = productSearchStringPopupContainer.querySelector("dialog");

    dialog.showModal();
}

async function showPromotionProductFilePopup(e) {

    const productId = getProductIdFromItem(e.currentTarget);

    const url = `api/components/productEditor/promotionFilePopup/${productId}`;

    const response = await fetch(url, {
        method: "GET",
    });

    if (!response.ok) return;

    const data = await response.text();

    const promotionProductFilePopupContainer = document.getElementById(promotionProductFilePopupContainerId);

    promotionProductFilePopupContainer.innerHTML = data;

    const dialog = promotionProductFilePopupContainer.querySelector("dialog");

    promotionFilePopup.initEventsForReadonlyInstance();

    dialog.showModal();
}

async function showSingleProductEditorPopup(e) {

    const productId = getProductIdFromItem(e.currentTarget);

    const url = `api/components/productEditor/single/${productId}`;

    const response = await fetch(url, {
        method: "GET",
    });

    if (!response.ok) return;

    const data = await response.text();

    const singleProductEditorPopupContainer = document.getElementById(singleProductEditorPopupContainerId);

    singleProductEditorPopupContainer.innerHTML = data;

    const jsonDataScriptElement = document.getElementById(singleProductEditorPopupJsonDataScriptId);

    const jsonData = JSON.parse(jsonDataScriptElement.textContent);

    singleProductEditorPopup.clearAll();
    singleProductEditorPopup.registerData(jsonData);

    jsonDataScriptElement.remove();

    const dialog = singleProductEditorPopupContainer.querySelector("dialog");

    singleProductEditorPopup.initEvents(dialog);

    dialog.showModal();
}

function onSingleProductEditorSaveSuccessful() {


}

function getProductIdFromItem(childElement) {

    const item = childElement.closest(`[name="${itemName}"]`);

    return item.getAttribute(itemProductIdAttribute);
}
