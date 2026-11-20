import * as antiforgeryUtils from "../Common/AntiforgeryUtils.js";
import * as popup from "../Primitives/Popup.js";
import * as popupFullScreenButton from "../Primitives/PopupFullScreenButton.js";
import * as carouselModule from "../Primitives/Carousel.js";
import * as productData from "../Product/ProductData.js";
import * as groupPromotionsDisplay from "../Promotions/GroupPromotionsDisplay.js";
import * as urlUtils from "../Common/UrlUtils.js";

const showSearchHelpPopupButtonId = "showSearchHelpPopupButton";
const searchInputId = "productTableSearchInput";
const searchButtonId = "productTableSearchButton";
const categorySelectId = "productTableCategorySelect";
const manufacturerSelectId = "productTableManufacturerSelect";
const productStatusSelectId = "productTableProductStatusSelect";
const promotionStatusSelectId = "productTablePromotionStatusSelect";
const currencySelectId = "productTableCurrencySelect";
const resultsCountSelectId = "productTableResultsCountSelect";
const resultsShowImagesCheckboxId = "productTableResultsShowImagesCheckbox";
const orderSummaryLabelTextId = "productTableOrderSummaryLabelText";
const orderSummaryCheckboxId = "productTableOrderSummaryCheckbox";
const offerPageButtonId = "productTableOfferPageButton"

const productsDataContainerId = "productsDataContainer";

const unavailableProductsRowName = "productTableUnavailableProductsRow";

const productRowName = "productTableItem";

const productXmlPopupContainerId = "productXmlPopupContainer";
const productDataPopupContainerId = "productDataPopupContainer";
const promotionGroupImagesPopupContainerId = "promotionGroupImagesPopupContainer";
const productSearchStringPartsPopupContainerId = "productSearchStringPartsPopupContainer";
const searchHelpPopupId = "searchHelpPopup";

const productDataDialogCarouselId = "productDataCarousel";

const productPromotionTableItemName = "productPromotionTableItem"
const itemQuantityInputName = "quantityInput";
const itemShowProductXmlPopupButtonName = "showProductXmlPopupButton";
const itemShowProductDataPopupButtonName = "showProductDataPopupButton";
const itemShowPromotionGroupImagesPopupButtonName = "showPromotionGroupImagesPopupButton";
const itemShowProductSearchStringPartsPopupButtonName = "showProductSearchStringPartsPopupButton";
const productImagesColumnPlaceholderName = "productImagesColumnPlaceholder";

const searchInputTextWithoutOrderSummaryAttribute = "data-search-without-order-summary";

const productRowProductIdAttribute = "data-product-id";
const productRowIsExternalOrderSummaryItemAttribute = "data-is-external-order-summary-item";
const productRowShowPromotionGroupImagesPopupButtonPromotionGroupIdAttribute = "data-manufacturer-promotion-group-id";
const productRowImageDataSourceAttribute = "data-src";

const loadingClass = "loading";

const productsWithSelectedQuantities = new Map();

let isOrderSummaryFetchingExternalEntries = false;

if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", initEvents);
}
else {
    initEvents();
}

document.addEventListener("visibilitychange", onDocumentVisibilityChange);
window.addEventListener("resize", onWindowResize);

function initEvents() {

    const showSearchHelpPopupButton = document.getElementById(showSearchHelpPopupButtonId);
    const searchButton = document.getElementById(searchButtonId);
    const categorySelect = document.getElementById(categorySelectId);
    const manufacturerSelect = document.getElementById(manufacturerSelectId);
    const resultsShowImagesCheckbox = document.getElementById(resultsShowImagesCheckboxId);
    const orderSummaryCheckbox = document.getElementById(orderSummaryCheckboxId);
    const offerPageButton = document.getElementById(offerPageButtonId);

    showSearchHelpPopupButton.addEventListener("click", showSearchHelpPopup);
    searchButton.addEventListener("click", searchProductsAndDisplaySearchButtonLoader);
    categorySelect.addEventListener("change", setManufacturersForCategory);
    manufacturerSelect.addEventListener("change", searchProductsAndDisplaySearchButtonLoader);
    resultsShowImagesCheckbox.addEventListener("change", toggleShowImages);
    orderSummaryCheckbox.addEventListener("change", toggleOrderSummary);
    offerPageButton.addEventListener("click", openOfferPage);
}

function onDocumentVisibilityChange() {

    const productDataPopupContainer = document.getElementById(productDataPopupContainerId);

    const productDataDialog = productDataPopupContainer.querySelector("dialog");

    if (!productDataDialog) return;

    const productDataDialogCarousel = productDataDialog.querySelector(`[name='${carouselModule.carouselName}']`);

    if (!productDataDialogCarousel) return;

    productData.onDocumentVisibilityChange(productDataDialogCarousel);
}

async function onWindowResize() {

    const productDataPopupContainer = document.getElementById(productDataPopupContainerId);

    const productDataDialog = productDataPopupContainer.querySelector("dialog");

    if (!productDataDialog) return;

    const productDataDialogCarousel = productDataDialog.querySelector(`[name='${carouselModule.carouselName}']`);

    if (!productDataDialogCarousel) return;

    await productData.resizeHtmlContentInCarousel(productDataDialogCarousel);
}

function initItemEventsAndQuantityData(item) {

    const productIdAttribute = item.getAttribute(productRowProductIdAttribute);

    const productId = parseInt(productIdAttribute);

    const itemQuantityInput = item.querySelector(`[name='${itemQuantityInputName}']`);
    const itemShowProductXmlPopupButton = item.querySelector(`[name='${itemShowProductXmlPopupButtonName}']`);
    const itemShowProductDataPopupButton = item.querySelector(`[name='${itemShowProductDataPopupButtonName}']`);
    const itemShowPromotionGroupImagesPopupButton = item.querySelector(`[name='${itemShowPromotionGroupImagesPopupButtonName}']`);
    const itemShowProductSearchStringPartsPopupButton = item.querySelector(`[name='${itemShowProductSearchStringPartsPopupButtonName}']`);

    const existingListedQuantity = productsWithSelectedQuantities.get(productId);

    if (existingListedQuantity != null) {
        itemQuantityInput.value = existingListedQuantity;
    }

    itemQuantityInput.addEventListener("change", onItemQuantityInputChange);
    itemShowProductXmlPopupButton.addEventListener("click", showProductXmlPopupAsync);
    itemShowProductDataPopupButton.addEventListener("click", showProductDataPopupAsync);

    if (itemShowPromotionGroupImagesPopupButton) {
        itemShowPromotionGroupImagesPopupButton.addEventListener("click", showPromotionGroupImagesPopupAsync);
    }

    itemShowProductSearchStringPartsPopupButton.addEventListener("click", showProductSearchStringPartsPopupAsync);
}

async function searchProductsAndDisplaySearchButtonLoader() {

    const searchInput = document.getElementById(searchInputId);

    const orderSummaryCheckbox = document.getElementById(orderSummaryCheckboxId);
    const orderSummaryLabelText = document.getElementById(orderSummaryLabelTextId);

    const showImagesCheckbox = document.getElementById(resultsShowImagesCheckboxId);

    if (orderSummaryCheckbox.checked) {

        revertHeaderFromOrderSummary(searchInput, orderSummaryLabelText);

        orderSummaryCheckbox.checked = false;
    }

    const showImages = showImagesCheckbox.checked === true;

    const searchButton = document.getElementById(searchButtonId);

    searchButton.classList.add(loadingClass);

    try {
        await searchProductsAndReplaceInTableAsync();

        if (showImages) {

            const productRowElements = [...document.getElementsByName(productRowName)];

            changeImagesDisplayForAll(productRowElements, showImages);
        }
    }
    finally {
        searchButton.classList.remove(loadingClass);
    }
}

async function searchProductsAndReplaceInTableAsync() {

    const searchOptions = getProductSearchOptions();

    const url = "api/components/home/search";

    const response = await fetch(url, {
        method: "POST",
        headers: {
            'Content-Type': 'application/json',
            'Accept': 'application/html'
        },
        body: JSON.stringify(searchOptions)
    });

    if (!response.ok) return;

    const productsDataContainer = document.getElementById(productsDataContainerId);

    const data = await response.text();

    productsDataContainer.innerHTML = data;

    const productRows = document.getElementsByName(productRowName);

    for (const productRow of productRows) {

        initItemEventsAndQuantityData(productRow);
    }
}

async function setManufacturersForCategory() {

    const categoryIdString = document.getElementById(categorySelectId).value;

    const categoryId = getNumberOrNullFromString(categoryIdString);

    let url = `api/components/home/search/manufacturers`;

    if (categoryId != null) {
        url += `/${categoryId}`;
    }

    const response = await fetch(url, {
        method: "GET"
    });

    if (!response.ok) return;

    const data = await response.json();

    const manufacturerSelect = document.getElementById(manufacturerSelectId);

    for (let i = manufacturerSelect.options.length - 1; i >= 0; i--) {

        if (manufacturerSelect.options[i].value === "") continue;

        manufacturerSelect.remove(i);
    }

    data.forEach(manufacturer => {

        const option = document.createElement("option");

        option.value = manufacturer.id;
        option.textContent = manufacturer.displayName;

        manufacturerSelect.appendChild(option);
    });
}

function toggleShowImages() {

    const showImagesCheckbox = document.getElementById(resultsShowImagesCheckboxId);

    const showImages = showImagesCheckbox.checked === true;
    
    const orderSummaryCheckbox = document.getElementById(orderSummaryCheckboxId);

    const showOrderSummary = orderSummaryCheckbox.checked === true;

    let productRowElements;

    if (!showOrderSummary) {
        productRowElements = [...document.getElementsByName(productRowName)];
    }
    else {
        productRowElements = [...document.getElementsByName(productRowName)];

        productRowElements = productRowElements.filter(element => element.style.display !== "none");
    }

    changeImagesDisplayForAll(productRowElements, showImages);
}

function changeImagesDisplayForAll(productRowElements, showImages) {

    for (const productRowElement of productRowElements) {

        const productPromotionTableItem = productRowElement.querySelector(`[name='${productPromotionTableItemName}']`);

        const productImagesRow = productRowElement.nextElementSibling;
        const productImagesColumnPlaceholder = productImagesRow.querySelector(`[name='${productImagesColumnPlaceholderName}']`);

        const images = productImagesRow.getElementsByTagName("img");

        const willShowImages = showImages && images.length > 0;

        if (!willShowImages) {

            productPromotionTableItem.setAttribute("rowspan", "1");

            productImagesColumnPlaceholder.setAttribute("colspan", "4");

            productImagesRow.style.display = "none";

            for (const image of images) {

                image.removeEventListener("dblclick", openImageUrlInNewWindow);
            }

            continue;
        }

        productPromotionTableItem.setAttribute("rowspan", "2");

        productImagesColumnPlaceholder.setAttribute("colspan", "3");

        for (const image of images) {

            if (image.hasAttribute(productRowImageDataSourceAttribute)) {

                image.src = image.getAttribute(productRowImageDataSourceAttribute);

                image.removeAttribute(productRowImageDataSourceAttribute);
            }

            image.addEventListener("dblclick", openImageUrlInNewWindow);
        }

        productImagesRow.style.display = "";
    }
}

function openImageUrlInNewWindow(e) {

    const imageUrl = e.currentTarget.src;

    urlUtils.openDataUrlInNewWindow(imageUrl);
}

async function toggleOrderSummary() {

    const orderSummaryCheckbox = document.getElementById(orderSummaryCheckboxId);

    const showOrderSummary = orderSummaryCheckbox.checked === true;

    const orderSummaryLabelText = document.getElementById(orderSummaryLabelTextId);

    const searchInput = document.getElementById(searchInputId);

    const showImagesCheckbox = document.getElementById(resultsShowImagesCheckboxId);

    const showImages = showImagesCheckbox.checked === true;

    const productsDataContainer = document.getElementById(productsDataContainerId);

    let productsDataTableBody = productsDataContainer.querySelector("tbody");

    let productRowElements;
    let tableDoesNotExist;

    if (productsDataTableBody) {

        productRowElements = productsDataTableBody.querySelectorAll(`[name='${productRowName}']`);
        tableDoesNotExist = false;
    }
    else {
        productRowElements = [];
        tableDoesNotExist = true;
    }

    if (isOrderSummaryFetchingExternalEntries && !showOrderSummary) {

        isOrderSummaryFetchingExternalEntries = true;
    }
    else {
        const productRowElementProductIds = new Map();

        for (const productRowElement of productRowElements) {

            const productIdAttribute = productRowElement.getAttribute(productRowProductIdAttribute);

            const productId = parseInt(productIdAttribute);

            productRowElementProductIds.set(productId, productRowElement);
        }

        for (const [productId, _] of productsWithSelectedQuantities) {

            if (!productRowElementProductIds.has(productId)) {

                isOrderSummaryFetchingExternalEntries = true;

                break;
            }

            isOrderSummaryFetchingExternalEntries = false;
        }
    }

    if (!tableDoesNotExist) {

        const unavailableProductsRow = productsDataTableBody.querySelector(`[name='${unavailableProductsRowName}']`);

        if (unavailableProductsRow) {

            unavailableProductsRow.remove();
        }
    }

    if (!showOrderSummary) {

        revertHeaderFromOrderSummary(searchInput, orderSummaryLabelText);

        const productRowElementsToChangeImages = [];

        if (isOrderSummaryFetchingExternalEntries) {

            for (const productRowElement of productRowElements) {

                const isExternalOrderSummaryItemAttribute = productRowElement.getAttribute(productRowIsExternalOrderSummaryItemAttribute);

                if (isExternalOrderSummaryItemAttribute === "true") {

                    productRowElement.nextElementSibling.remove();
                    productRowElement.remove();

                    continue;
                }

                productRowElement.style.display = "";

                productRowElementsToChangeImages.push(productRowElement);
            }
        }
        else {
            for (const productRowElement of productRowElements) {

                productRowElement.style.display = "";
                productRowElementsToChangeImages.push(productRowElement);
            }
        }

        if (showImages) {
            changeImagesDisplayForAll(productRowElementsToChangeImages, true);
        }

        return;
    }

    const orderSummaryProductIds = [];

    if (isOrderSummaryFetchingExternalEntries) {

        const currencySelect = document.getElementById(currencySelectId);

        const currency = getNumberOrNullFromString(currencySelect.value);

        const productQuantities = []; 

        for (const [productId, quantity] of productsWithSelectedQuantities) {

            const productQuantityData = {
                id: productId,
                quantity: quantity,
            };

            productQuantities.push(productQuantityData);
        }

        const body = {
            productQuantities: productQuantities,
            currency: currency,
            showEntireTable: tableDoesNotExist,
        };

        const url = "api/components/home/orderSummary";

        const response = await fetch(url, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Accept": "text/html, application/problem+json",
                [antiforgeryUtils.antiforgeryHeaderName]: antiforgeryUtils.getAntiforgeryTokenValue()
            },
            body: JSON.stringify(body)
        });

        if (!response.ok) return;

        const data = await response.text();

        if (!tableDoesNotExist) {

            for (const productRowElement of productRowElements) {

                productRowElement.style.display = "none";
            } 

            productsDataTableBody.insertAdjacentHTML("afterbegin", data);
        }
        else {

            productsDataContainer.innerHTML = data;

            productsDataTableBody = productsDataContainer.querySelector("tbody"); 
        }

        const newProductRows = productsDataTableBody.querySelectorAll(
            `[name='${productRowName}'][${productRowIsExternalOrderSummaryItemAttribute}='true']`);

        for (const productRow of newProductRows) {

            initItemEventsAndQuantityData(productRow);

            const productIdAttribute = productRow.getAttribute(productRowProductIdAttribute);

            const productId = parseInt(productIdAttribute);

            orderSummaryProductIds.push(productId);
        }

        for (const [productId, _] of productsWithSelectedQuantities) {

            if (!orderSummaryProductIds.includes(productId)) {

                productsWithSelectedQuantities.delete(productId);
            }
        }

        changeImagesDisplayForAll(productRowElements, false);

        if (showImages) {
            changeImagesDisplayForAll(newProductRows, true);
        }
    }
    else {
        const notShownProducts = [];

        for (const productRowElement of productRowElements) {

            const quantityInput = productRowElement.querySelector(`[name='${itemQuantityInputName}']`);

            const quantity = parseInt(quantityInput.value);

            if (isNaN(quantity) || quantity <= 0) {

                productRowElement.style.display = "none";

                notShownProducts.push(productRowElement);

                continue;
            }

            const productId = productRowElement.getAttribute(productRowProductIdAttribute);

            orderSummaryProductIds.push(productId);
        }

        changeImagesDisplayForAll(notShownProducts, false);
    }

    searchInput.setAttribute(searchInputTextWithoutOrderSummaryAttribute, searchInput.value);

    searchInput.value = orderSummaryProductIds.join(',');

    orderSummaryLabelText.innerText = "Show Search"; 
}

function revertHeaderFromOrderSummary(searchInput, orderSummaryLabelText) {

    searchInput.value = searchInput.getAttribute(searchInputTextWithoutOrderSummaryAttribute);

    orderSummaryLabelText.innerText = "Order";
}

function openOfferPage() {

    const productQuantities = [];

    for (const [productId, quantity] of productsWithSelectedQuantities) {

        const productQuantityData = {
            id: productId,
            quantity: quantity,
        };

        productQuantities.push(productQuantityData);
    }

    const form = document.createElement("form");

    form.name = "HomeOrder";

    form.method = "POST";
    form.action = "/home/offer";
    form.target = "_blank";

    const formNameInput = document.createElement("input");
    formNameInput.type = "hidden";
    formNameInput.name = "_handler";
    formNameInput.value = "HomeOrder";

    const input = document.createElement("input");

    input.type = "hidden";
    input.name = "payload";
    input.value = JSON.stringify(productQuantities);

    const antiforgeryTokenInput = document.createElement("input");

    antiforgeryTokenInput.type = "hidden";
    antiforgeryTokenInput.name = antiforgeryUtils.antiforgeryInputName;
    antiforgeryTokenInput.value = antiforgeryUtils.getAntiforgeryTokenValue();

    form.appendChild(formNameInput);
    form.appendChild(input);
    form.appendChild(antiforgeryTokenInput);

    document.body.appendChild(form);

    form.submit();
    form.remove();
}

function getProductSearchOptions(pageNumber = null) {

    const searchInput = document.getElementById(searchInputId);
    const categorySelect = document.getElementById(categorySelectId);
    const manufacturerSelect = document.getElementById(manufacturerSelectId);
    const productStatusSelect = document.getElementById(productStatusSelectId);
    const promotionStatusSelect = document.getElementById(promotionStatusSelectId);
    const currencySelect = document.getElementById(currencySelectId);
    const resultsCountSelect = document.getElementById(resultsCountSelectId);

    const searchData = searchInput.value;

    const categoryId = getNumberOrNullFromString(categorySelect.value);
    const manufacturerId = getNumberOrNullFromString(manufacturerSelect.value);
    const productStatus = getNumberOrNullFromString(productStatusSelect.value);
    const promotionStatus = getNumberOrNullFromString(promotionStatusSelect.value);
    const currency = getNumberOrNullFromString(currencySelect.value);
    const resultsCount = getNumberOrNullFromString(resultsCountSelect.value);

    return {
        SearchData: searchData,
        CategoryId: categoryId,
        ManufacturerId: manufacturerId,
        ProductStatusSearchOptions: productStatus,
        PromotionSearchOptions: promotionStatus,
        Currency: currency,
        MaxSearchResults: resultsCount
    };
}

function showSearchHelpPopup() {

    const dialog = document.getElementById(searchHelpPopupId);

    popup.attachCloseEvents(dialog);

    dialog.showModal();
}

function onItemQuantityInputChange(e) {

    const quantityInput = e.currentTarget;

    const item = quantityInput.closest(`[name='${productRowName}']`);

    const productIdAttribute = item.getAttribute(productRowProductIdAttribute);

    const productId = parseInt(productIdAttribute);

    const quantity = parseInt(quantityInput.value);

    if (isNaN(quantity) || quantity <= 0) {

        productsWithSelectedQuantities.delete(productId)

        return;
    }

    productsWithSelectedQuantities.set(productId, quantity);
}

async function showProductXmlPopupAsync(e) {

    const item = e.currentTarget.closest(`[name='${productRowName}']`);

    const productId = item.getAttribute(productRowProductIdAttribute);
    
    const data = await getProductXmlPopupDataAsync(productId);

    const productXmlPopupContainer = document.getElementById(productXmlPopupContainerId);

    productXmlPopupContainer.innerHTML = data;

    const dialog = productXmlPopupContainer.querySelector("dialog");

    popup.attachCloseEvents(dialog);

    dialog.showModal();
}

async function showProductDataPopupAsync(e) {

    const item = e.currentTarget.closest(`[name='${productRowName}']`);

    const productId = item.getAttribute(productRowProductIdAttribute);

    const data = await getProductDataPopupDataAsync(productId);

    const productDataPopupContainer = document.getElementById(productDataPopupContainerId);

    productDataPopupContainer.innerHTML = data;

    const productDataDialog = productDataPopupContainer.querySelector("dialog");

    popup.attachCloseEvents(productDataDialog, onProductDataDialogClose);
    popupFullScreenButton.attachFullScreenButtonEvent(productDataDialog);

    const productDataDialogCarousel = productDataDialog.querySelector(`[name='${carouselModule.carouselName}']`);

    await productData.initEventsAndCarousel(productDataDialogCarousel);

    productDataDialog.showModal();
}

function onProductDataDialogClose(e) {

    const productDataDialog = e.currentTarget;

    const carousel = productDataDialog.querySelector(`[name='${carouselModule.carouselName}']`);

    if (carousel) {

        carouselModule.stopAutoSlide(carousel);
    }
}

async function showPromotionGroupImagesPopupAsync(e) {

    const itemShowPromotionGroupImagesPopupButton = e.currentTarget;

    const item = itemShowPromotionGroupImagesPopupButton.closest(`[name='${productRowName}']`);

    const promotionGroupId = itemShowPromotionGroupImagesPopupButton
        .getAttribute(productRowShowPromotionGroupImagesPopupButtonPromotionGroupIdAttribute);

    const productId = item.getAttribute(productRowProductIdAttribute);

    const data = await getPromotionGroupImagesPopupDataAsync(promotionGroupId, productId);

    const promotionGroupImagesPopupContainer = document.getElementById(promotionGroupImagesPopupContainerId);

    promotionGroupImagesPopupContainer.innerHTML = data;

    const dialog = promotionGroupImagesPopupContainer.querySelector("dialog");

    popup.attachCloseEvents(dialog);
    groupPromotionsDisplay.initEvents();

    dialog.showModal();
}

async function showProductSearchStringPartsPopupAsync(e) {

    const item = e.currentTarget.closest(`[name='${productRowName}']`);

    const productId = item.getAttribute(productRowProductIdAttribute);

    const data = await getProductSearchStringPartsPopupDataAsync(productId);

    const productSearchStringPartsPopupContainer = document.getElementById(productSearchStringPartsPopupContainerId);

    productSearchStringPartsPopupContainer.innerHTML = data;

    const dialog = productSearchStringPartsPopupContainer.querySelector("dialog");

    popup.attachCloseEvents(dialog);

    dialog.showModal();
}

async function getProductXmlPopupDataAsync(productId) {

    const url = `api/components/home/productXmlDataPopup/${productId}`;

    const response = await fetch(url, {
        method: "GET",
    });

    if (!response.ok) return null;

    return await response.text();
}

async function getProductDataPopupDataAsync(productId) {

    const url = `api/components/home/productDataPopup/${productId}`;

    const response = await fetch(url, {
        method: "GET",
    });

    if (!response.ok) return null;

    return await response.text();
}

async function getPromotionGroupImagesPopupDataAsync(promotionGroupId, productId) {

    const url = `api/components/home/productGroupPromotionPopup/${promotionGroupId}?productId=${productId}`;

    const response = await fetch(url, {
        method: "GET",
    });

    if (!response.ok) return null;

    return await response.text();
}

async function getProductSearchStringPartsPopupDataAsync(productId) {

    const url = `api/components/home/productSearchStringPopup/${productId}`;

    const response = await fetch(url, {
        method: "GET",
    });

    if (!response.ok) return null;

    return await response.text();
}

function getNumberOrNull(numberValue) {
    return typeof numberValue === "number" ? numberValue : null;
}

function getNumberOrNullFromString(stringValue) {

    if (stringValue == null || stringValue === "") return null;

    var output = null;

    const parsedNumber = parseInt(stringValue);

    if (!isNaN(parsedNumber)) {
        output = parsedNumber;
    }

    return output;
}
