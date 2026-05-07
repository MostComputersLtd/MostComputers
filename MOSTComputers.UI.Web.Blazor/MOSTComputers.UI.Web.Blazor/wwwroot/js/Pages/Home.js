const searchInputId = "productTableSearchInput";
const searchButtonId = "productTableSearchButton";
const searchButtonLoaderId = "productTableSearchLoader";
const categorySelectId = "productTableCategorySelect";
const manufacturerSelectId = "productTableManufacturerSelect";
const productStatusSelectId = "productTableProductStatusSelect";
const promotionStatusSelectId = "productTablePromotionStatusSelect";
const currencySelectId = "productTableCurrencySelect";
const resultsCountSelectId = "productTableResultsCountSelect";
const resultsShowImagesCheckboxId = "productTableResultsShowImagesCheckbox";
const orderSummaryLabelTextId = "productTableOrderSummaryLabelText";
const orderSummaryCheckboxId = "productTableOrderSummaryCheckbox";

const productsDataContainerId = "productsDataContainer";

const productRowName = "productTableItem";
const productPromotionTableItemIdPrefix = "productPromotionTableItem-"
const quantityInputIdPrefix = "quantityInput-";
const productImagesRowIdPrefix = "productImagesRow-";
const productImagesColumnPlaceholderIdPrefix = "productImagesColumnPlaceholder-";

const productXmlPopupContainerId = "productXmlPopupContainer";
const productDisplayPopupContainerId = "productDisplayPopupContainer";
const promotionGroupImagesPopupContainerId = "promotionGroupImagesPopupContainer";
const productSearchStringPartsPopupContainerId = "productSearchStringPartsPopupContainer";
const searchHelpPopupId = "searchHelpPopup";

const productDataDialogCarouselId = "productDataCarousel";

const productRowProductIdAttribute = "data-product-id";
const searchInputTextWithoutOrderSummaryAttribute = "data-search-without-order-summary";

const loadingClass = "loading";

document.addEventListener("visibilitychange", handleVisibilityChange);

function handleVisibilityChange() {
    const productDataDialogCarousel = document.getElementById(productDataDialogCarouselId);

    if (productDataDialogCarousel == null) return;

    if (document.hidden) {
        stopAutoSlide(productDataDialogCarouselId);
    }
    else {
        startAutoSlide(true, productDataDialogCarouselId);
    }
}

async function searchProductsAndDisplaySearchButtonLoader() {

    const orderSummaryCheckbox = document.getElementById(orderSummaryCheckboxId);
    const orderSummaryLabelText = document.getElementById(orderSummaryLabelTextId);

    const showImagesCheckbox = document.getElementById(resultsShowImagesCheckboxId);

    orderSummaryCheckbox.checked = false;
    orderSummaryLabelText.innerText = "Order Summary";
    showImagesCheckbox.checked = false;

    const searchButton = document.getElementById(searchButtonId);
    const searchButtonLoader = document.getElementById(searchButtonLoaderId);

    if (searchButton && searchButtonLoader) {
        searchButton.classList.add(loadingClass);
        searchButtonLoader.classList.add(loadingClass);

        searchButton.toggleAttribute("disabled", true);
    }

    await searchProductsAndReplaceInTableAsync();

    if (searchButton && searchButtonLoader) {
        searchButton.classList.remove(loadingClass);
        searchButtonLoader.classList.remove(loadingClass);

        searchButton.toggleAttribute("disabled", false);
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
}

async function setManufacturersForCategory() {

    const categoryIdString = document.getElementById(categorySelectId).value;

    const categoryId = getNumberOrNullFromString(categoryIdString);

    let url = `api/components/home/search/manufacturers`;

    if (categoryId != null) {
        url += `/${categoryId}`;
    }

    const response = await fetch(url,
    {
        method: "GET"
    });

    if (!response.ok) return;

    const data = await response.json();

    const manufacturerSelect = document.getElementById(manufacturerSelectId);

    for (var i = manufacturerSelect.options.length - 1; i >= 0; i--) {
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

        const indexOfNumberInId = productRowElement.id.indexOf("-") + 1;

        const productIdAsString = productRowElement.id.substring(indexOfNumberInId);

        const productPromotionTableItem = document.getElementById(productPromotionTableItemIdPrefix + productIdAsString);
        const productImagesRow = document.getElementById(productImagesRowIdPrefix + productIdAsString);
        const productImagesColumnPlaceholder = document.getElementById(productImagesColumnPlaceholderIdPrefix + productIdAsString);

        const willShowImages = showImages && productImagesRow.getElementsByTagName("img").length > 0;

        if (!willShowImages) {
            productPromotionTableItem.setAttribute("rowspan", "1");

            productImagesColumnPlaceholder.setAttribute("colspan", "4");

            productImagesRow.style.display = "none";

            continue;
        }

        productPromotionTableItem.setAttribute("rowspan", "2");

        productImagesColumnPlaceholder.setAttribute("colspan", "3");

        const unsetImages = [...productImagesRow.querySelectorAll("img[data-src]")];

        for (const image of unsetImages) {

            image.src = image.getAttribute("data-src");

            image.removeAttribute("data-src");
        }

        productImagesRow.style.display = "";
    }
}

function toggleOrderSummary() {
    const orderSummaryCheckbox = document.getElementById(orderSummaryCheckboxId);

    const showOrderSummary = orderSummaryCheckbox.checked === true;

    const orderSummaryLabelText = document.getElementById(orderSummaryLabelTextId);

    const searchInput = document.getElementById(searchInputId);

    const productRowElements = [...document.getElementsByName(productRowName)];

    if (!showOrderSummary) {
        searchInput.value = searchInput.getAttribute(searchInputTextWithoutOrderSummaryAttribute);

        orderSummaryLabelText.innerText = "Order Summary";

        for (const productRowElement of productRowElements) {

            productRowElement.style.display = "";
        }

        const showImagesCheckbox = document.getElementById(resultsShowImagesCheckboxId);

        const showImages = showImagesCheckbox.checked === true;

        if (showImages) {
            changeImagesDisplayForAll(productRowElements, true);
        }
        
        return;
    }

    orderSummaryLabelText.innerText = "Show Search";

    const shownProducts = hideProductsThatAreNotInSummaryAndChangeSearchInputValue(productRowElements, searchInput);

    const notShownProducts = productRowElements.filter(
        productRowElement => !shownProducts.includes(productRowElement));

    changeImagesDisplayForAll(notShownProducts, false);
}

function hideProductsThatAreNotInSummaryAndChangeSearchInputValue(productRowElements, searchInput) {
    
    const orderSummaryProductIds = [];
    const orderSummaryProductRows = [];

    for (const productRowElement of productRowElements) {

        const indexOfNumberInId = productRowElement.id.indexOf("-") + 1;

        const productIdAsString = productRowElement.id.substring(indexOfNumberInId);

        const quantityInput = document.getElementById(quantityInputIdPrefix + productIdAsString);

        const quantityInputValue = quantityInput.value;

        if (quantityInputValue == null || quantityInputValue === "") {
            productRowElement.style.display = "none";

            continue;
        }

        const quantity = parseInt(quantityInputValue);

        if (isNaN(quantity) || quantity <= 0) {
            productRowElement.style.display = "none";

            continue;
        }

        const productId = productRowElement.getAttribute(productRowProductIdAttribute);

        orderSummaryProductIds.push(productId);
        orderSummaryProductRows.push(productRowElement);
    }

    searchInput.setAttribute(searchInputTextWithoutOrderSummaryAttribute, searchInput.value);

    searchInput.value = orderSummaryProductIds.join(',');

    return orderSummaryProductRows;
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

    dialog.showModal();
}

async function showProductXmlPopup(productId) {
    const data = await getProductXmlPopupDataAsync(productId);

    const productXmlPopupContainer = document.getElementById(productXmlPopupContainerId);

    productXmlPopupContainer.innerHTML = data;

    const dialog = productXmlPopupContainer.querySelector("dialog");

    dialog.showModal();
}

async function showProductDisplayPopupAsync(productId) {
    const data = await getProductDisplayPopupDataAsync(productId);

    const productDisplayPopupContainer = document.getElementById(productDisplayPopupContainerId);

    productDisplayPopupContainer.innerHTML = data;

    const productDataDialog = productDisplayPopupContainer.querySelector("dialog");

    requestAnimationFrame(function ()
    {
        startAutoSlide(true, productDataDialogCarouselId);

        productDataDialog.addEventListener("close", function () {
            stopAutoSlide(productDataDialogCarouselId);
        });

        productDataDialog.addEventListener("cancel", function () {
            stopAutoSlide(productDataDialogCarouselId);
        });
    });

    productDataDialog.showModal();
}

async function showPromotionGroupImagesPopup(promotionGroupId, productId) {
    const data = await getPromotionGroupImagesPopupDataAsync(promotionGroupId, productId);

    const promotionGroupImagesPopupContainer = document.getElementById(promotionGroupImagesPopupContainerId);

    promotionGroupImagesPopupContainer.innerHTML = data;

    const dialog = promotionGroupImagesPopupContainer.querySelector("dialog");

    dialog.showModal();
}

async function showProductSearchStringPartsPopupAsync(productId) {
    const data = await getProductSearchStringPartsPopupDataAsync(productId);

    const productSearchStringPartsPopupContainer = document.getElementById(productSearchStringPartsPopupContainerId);

    productSearchStringPartsPopupContainer.innerHTML = data;

    const dialog = productSearchStringPartsPopupContainer.querySelector("dialog");

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

async function getProductDisplayPopupDataAsync(productId) {

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