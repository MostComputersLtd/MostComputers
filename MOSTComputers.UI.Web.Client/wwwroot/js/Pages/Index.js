const productListId = "productList";
const productDataDialogId = "productDataDialog";

const productDataDialogContentId = "productDataDialogContent"
const productDataDialogCarouselId = "productDataCarousel";

const groupPromotionsCarouselContainerId = "groupPromotionsCarouselContainer"
const groupPromotionsCarouselId = "groupPromotionsCarousel";

const searchInputId = "productSearchInput";
const categorySelectId = "productTableCategorySelect";
const manufacturerSelectId = "productTableManufacturerSelect";
const productStatusCheckboxId = "productTableAvaliableOnlyCheckbox";
const sortingButtonId = "productSearchSortingButton";
const sortingOptionsListId = "productSearchSortingOptionsList";
const downloadXmlButtonId = "downloadXmlButton";
const downloadXmlOptionsListId = "downloadXmlOptionsList";
const downloadXmlForSelectionButtonId = "downloadXmlForSelectionButton" 
const downloadXmlForAllProductsButtonId = "downloadXmlForAllProductsButton" 

const searchButtonId = "productSearchButton";
const searchButtonLoaderId = "productSearchButtonLoader";

const sortingButtonSelectedSortingMethodAttribute = "data-selected-sorting-method";

const loadingClass = "loading";

const optionsListOpenClass = "open";
const sortingListOptionSelectedClass = "selected-sorting-method";

const downloadXmlListOpenClass = "open";

const productListBottomIndicatorId = "productListBottomIndicator";

document.addEventListener("DOMContentLoaded", function () {
    const categorySelect = document.getElementById(categorySelectId);
    const manufacturerSelect = document.getElementById(manufacturerSelectId);
    const searchButton = document.getElementById(searchButtonId);
    const sortingButton = document.getElementById(sortingButtonId);
    const downloadXmlButton = document.getElementById(downloadXmlButtonId);
    const downloadXmlForSelectionButton = document.getElementById(downloadXmlForSelectionButtonId);
    const downloadXmlForAllProductsButton = document.getElementById(downloadXmlForAllProductsButtonId);
    
    const productDataDialogContent = document.getElementById(productDataDialogContentId);

    if (categorySelect) {
        categorySelect.addEventListener("change", onCategorySelectChanged);
    }

    if (manufacturerSelect) {
        manufacturerSelect.addEventListener("change", onManufacturerSelectChanged);
    }

    if (searchButton) {
        searchButton.addEventListener("click", onSearchButtonClick);
    }

    if (sortingButton) {
        sortingButton.addEventListener("click", toggleSortingList);
    }

    if (downloadXmlButton) {
        downloadXmlButton.addEventListener("click", toggleDownloadXmlList);
    }

    if (downloadXmlForSelectionButton) {
        downloadXmlForSelectionButton.addEventListener("click", downloadXmlForSelection)
    }

    if (downloadXmlForAllProductsButton) {
        downloadXmlForAllProductsButton.addEventListener("click", downloadXmlForAllProducts)
    }

    if (productDataDialogContent) {
        productDataDialogContent.addEventListener("click", function (e) {
            e.stopPropagation();
        });
    }

    const productListBottomIndicator = document.getElementById(productListBottomIndicatorId);

    addObserverToSearchMoreProductsWhenWeReachBottom(productListBottomIndicator);

    configureCarouselAutoSlideOnLoad(groupPromotionsCarouselId);

    const sizeLevel = getCurrentWindowSizeLevel();

    const carouselDisplayedItemsCount = getCarouselItemCountFromWindowSizeLevel(sizeLevel);

    resizeCarousel(groupPromotionsCarouselId, carouselDisplayedItemsCount);
});

window.addEventListener("resize", handleWindowResize);

document.addEventListener("visibilitychange", handleVisibilityChange);

document.addEventListener("click", hideSortingListIfClickedOutsideIt);
document.addEventListener("click", hideDownloadXmlListIfClickedOutsideIt);

let lastSizeLevel = getCurrentWindowSizeLevel();

function handleWindowResize() {
    const sizeLevel = getCurrentWindowSizeLevel();

    if (sizeLevel !== lastSizeLevel) {
        const carouselDisplayedItemsCount = getCarouselItemCountFromWindowSizeLevel(sizeLevel);

        resizeCarousel(groupPromotionsCarouselId, carouselDisplayedItemsCount);

        lastSizeLevel = sizeLevel;
    }
}

function handleVisibilityChange() {

    if (document.hidden) {
        TEMP__stopAutoSlide(groupPromotionsCarouselId);
    }
    else {
        TEMP__startAutoSlide(groupPromotionsCarouselId);
    }
}

const throttleSearchMoreTimeoutMilliseconds = 200;

let allowedToStartLoading = false;
let loadingMoreProducts = false;
let lastLoadStartTime = 0;

let currentPageNumber = 1;

const rootMarginForBottomOfPageInPixels = 300;

function addObserverToSearchMoreProductsWhenWeReachBottom(productListBottomIndicator) {

    //if (!("IntersectionObserver" in window)) {
        startObserveProductListBottomPolyfill(productListBottomIndicator);

        return;
    //}

    let options = {
        root: null,
        rootMargin: `0px 0px ${rootMarginForBottomOfPageInPixels}px 0px`,
        threshold: 0
    };

    let currentIntersectionObserver = new IntersectionObserver(searchMoreProductsWhenScrollToBottom, options);

    currentIntersectionObserver.observe(productListBottomIndicator);
}

async function searchMoreProductsWhenScrollToBottom(entries, observer) {
    if (loadingMoreProducts
        || !entries[0].isIntersecting
        || Date.now() - lastLoadStartTime < throttleSearchMoreTimeoutMilliseconds
        || !allowedToStartLoading)
    {
        return;
    }

    loadingMoreProducts = true;

    lastLoadStartTime = Date.now();

    currentPageNumber++;

    const targetElement = entries[0].target;

    observer.unobserve(targetElement);

    await searchProductsAndAddToTable(currentPageNumber);

    loadingMoreProducts = false;

    observer.observe(targetElement);
}

function startObserveProductListBottomPolyfill(productListBottomIndicator) {

    const check = trailingThrottle(async function () {

        const rect = productListBottomIndicator.getBoundingClientRect();

        if (rect.top > window.innerHeight + rootMarginForBottomOfPageInPixels
            || !allowedToStartLoading)
        {
            return;
        }

        window.removeEventListener("scroll", check);

        currentPageNumber++;

        await searchProductsAndAddToTable(currentPageNumber);

        window.addEventListener("scroll", check);

    }, rootMarginForBottomOfPageInPixels);

    window.addEventListener("scroll", check);

    check();
}

function trailingThrottle(callback, delay) {
    let lastCallTime = 0;
    let timer = null;

    return function (...args) {

        const currentTime = Date.now();

        const remainingTimeUntilCall = delay - (currentTime - lastCallTime);

        if (remainingTimeUntilCall <= 0) {

            lastCallTime = currentTime;

            callback.apply(this, args);

            return;
        }

        if (timer) return;

        timer = setTimeout(() => {

            lastCallTime = Date.now();

            timer = null;

            callback.apply(this, args);
        }, remainingTimeUntilCall);
    };
}

function getCurrentWindowSizeLevel() {

    if (window.innerWidth < 576) return 1;
    if (window.innerWidth < 1000) return 2;

    return 3;
}

function getCarouselItemCountFromWindowSizeLevel(windowSizeLevel) {
    switch (windowSizeLevel) {
        case 1: return 1; 
        case 2: return 2;

        case 3:
        default: return 3;
    }
}

function hideSortingListIfClickedOutsideIt(e) {
    const sortingOptionsList = document.getElementById(sortingOptionsListId);
    const sortingButton = document.getElementById(sortingButtonId);

    if (sortingOptionsList.contains(e.target) || sortingButton.contains(e.target)) return;

    sortingOptionsList.classList.remove(optionsListOpenClass);
}

function hideDownloadXmlListIfClickedOutsideIt(e) {
    const downloadXmlOptionsList = document.getElementById(downloadXmlOptionsListId);
    const downloadXmlButton = document.getElementById(downloadXmlButtonId);

    if (downloadXmlOptionsList.contains(e.target) || downloadXmlButton.contains(e.target)) return;

    downloadXmlOptionsList.classList.remove(optionsListOpenClass);
}

async function onCategorySelectChanged() {

    //await onSearchButtonClick();

    await setManufacturersForCategory();
}

async function onManufacturerSelectChanged() {

    await searchProductsAndReplaceInTable();
}

async function onSearchButtonClick() {
    await searchProductsAndDisplaySearchButtonLoader();
}

async function searchProductsAndDisplaySearchButtonLoader(pageNumber = null) {
    const searchButton = document.getElementById(searchButtonId);
    const searchButtonLoader = document.getElementById(searchButtonLoaderId);

    if (searchButton && searchButtonLoader) {
        searchButton.classList.add(loadingClass);
        searchButtonLoader.classList.add(loadingClass);

        searchButton.toggleAttribute("disabled", true);
    }

    await searchProductsAndReplaceInTable(pageNumber);

    if (searchButton && searchButtonLoader) {
        searchButton.classList.remove(loadingClass);
        searchButtonLoader.classList.remove(loadingClass);

        searchButton.toggleAttribute("disabled", false);
    }
}

function toggleSortingList() {
    const sortingOptionsList = document.getElementById(sortingOptionsListId);

    if (sortingOptionsList.classList.contains(optionsListOpenClass)) {
        sortingOptionsList.classList.remove(optionsListOpenClass);

        return;
    }

    sortingOptionsList.classList.add(optionsListOpenClass);
}

function toggleDownloadXmlList() {
    const downloadXmlOptionsList = document.getElementById(downloadXmlOptionsListId);

    if (downloadXmlOptionsList.classList.contains(optionsListOpenClass)) {
        downloadXmlOptionsList.classList.remove(optionsListOpenClass);

        return;
    }

    downloadXmlOptionsList.classList.add(optionsListOpenClass);
}

async function setSortingMethodAndSearchProducts(sortingMethod, selectedOptionElementId = null) {
    const sortingButton = document.getElementById(sortingButtonId);

    sortingButton.setAttribute(sortingButtonSelectedSortingMethodAttribute, sortingMethod);

    const selectedOptionsList = document.getElementById(sortingOptionsListId);

    const currentSelectedOptionElement = selectedOptionsList.querySelector(`li.${sortingListOptionSelectedClass}`);

    if (currentSelectedOptionElement != null) {
        currentSelectedOptionElement.classList.remove(sortingListOptionSelectedClass)
    }

    if (selectedOptionElementId != null) {
        const selectedOptionElement = document.getElementById(selectedOptionElementId);

        selectedOptionElement.classList.add(sortingListOptionSelectedClass);
    }

    await searchProductsAndDisplaySearchButtonLoader();

    downloadXmlOptionsList.classList.remove(optionsListOpenClass);
}

async function downloadXmlForSelection() {
    var uri = "/api/product/xml";

    const searchOptions = getProductSearchOptions();

    const xmlSearchOptions = getProductXmlSearchOptions(
        searchOptions.SearchData, searchOptions.CategoryId, searchOptions.ManufacturerId, searchOptions.AvailableOnly);

    const downloadXmlForSelectionButton = document.getElementById(downloadXmlForSelectionButtonId);

    if (downloadXmlForSelectionButton) {
        downloadXmlForSelectionButton.classList.add(loadingClass);
    }

    try {
        const response = await fetch(uri, {
            method: "POST",
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/xml',
            },
            body: JSON.stringify(xmlSearchOptions)
        });

        if (response.ok) {
            const currentDate = new Date(Date.now());

            const fileName = "Products_" + getFileNameDateTimeFormatted(currentDate);

            await downloadFile(response, fileName);
        }
    }
    finally {
        downloadXmlForSelectionButton.classList.remove(loadingClass);
    }
}

async function downloadXmlForAllProducts() {
    const uri = "/api/product/xml/all";

    const downloadXmlForAllProductsButton = document.getElementById(downloadXmlForAllProductsButtonId);

    if (downloadXmlForAllProductsButton) {
        downloadXmlForAllProductsButton.classList.add(loadingClass);
    }

    try {
        const response = await fetch(uri, {
            method: "GET",
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/xml',
            }
        });

        if (response.ok) {
            const currentDate = new Date(Date.now());

            const fileName = "AllProducts_" + getFileNameDateTimeFormatted(currentDate);

            await downloadFile(response, fileName);
        }
    }
    finally {
        downloadXmlForAllProductsButton.classList.remove(loadingClass);
    }
}

async function downloadFile(response, fileName) {
    const blob = await response.blob();

    const temporaryLink = document.createElement('a');

    temporaryLink.href = URL.createObjectURL(blob);

    temporaryLink.download = fileName;

    temporaryLink.target = "_blank";
    temporaryLink.rel = "noopener";

    document.body.appendChild(temporaryLink);

    temporaryLink.click();
    temporaryLink.remove();

    URL.revokeObjectURL(temporaryLink.href);
}

function getFileNameDateTimeFormatted(date) {

    return date.getFullYear() + '_' +
        addZeroToStartOfSingleDigitValues(date.getMonth() + 1) + '_' +
        addZeroToStartOfSingleDigitValues(date.getDate()) + '_' +
        addZeroToStartOfSingleDigitValues(date.getHours()) + ':' +
        addZeroToStartOfSingleDigitValues(date.getMinutes()) + ':' +
        addZeroToStartOfSingleDigitValues(date.getSeconds());
}

function addZeroToStartOfSingleDigitValues(str) {
    return String(str).padStart(2, "0");
}

async function searchProductsAndReplaceInTable(pageNumber = null) {

    allowedToStartLoading = false;

    const productsTableHtml = await searchProducts(pageNumber);

    const productList = document.getElementById(productListId);

    if (!productList) {
        return;
    }

    productList.innerHTML = productsTableHtml;

    currentPageNumber = 1;
    allowedToStartLoading = true;
}

async function searchProductsAndAddToTable(pageNumber = null) {
    const productsHtml = await searchProducts(pageNumber);

    const productList = document.getElementById(productListId);

    if (!productList) {
        return;
    }

    productList.innerHTML += productsHtml;
}

async function searchProducts(pageNumber = null) {
    const url = "/Index" + "?handler=SearchProducts";

    const searchOptions = getProductSearchOptions(pageNumber);

    const response = await fetch(url, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "RequestVerificationToken": getAntiforgeryTokenValue()
        },
        body: JSON.stringify(searchOptions)
    });

    if (!response.ok) {
        return null;
    }

    const productsTableHtml = await response.text();

    return productsTableHtml;
}

async function setManufacturersForCategory() {
    const categoryIdString = document.getElementById(categorySelectId).value;

    const categoryId = getNumberOrNullFromString(categoryIdString);

    const response = await fetch("/Index" + `?handler=GetManufacturersForCategory&categoryId=${categoryId}`,
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

function getProductXmlSearchOptions(searchData, categoryId, manufacturerId, selectAvailableProductsOnly) {
    return {
        SearchData: searchData,
        CategoryId: categoryId,
        ManufacturerId: manufacturerId,
        AvailableOnly: selectAvailableProductsOnly,
    }
}

function getProductSearchOptions(pageNumber = null) {
    const searchInput = document.getElementById(searchInputId);
    const categorySelect = document.getElementById(categorySelectId);
    const manufacturerSelect = document.getElementById(manufacturerSelectId);
    const productStatusCheckbox = document.getElementById(productStatusCheckboxId);
    const sortingButton = document.getElementById(sortingButtonId);

    const searchData = searchInput.value;

    const categoryIdString = categorySelect.value;
    const manufacturerIdString = manufacturerSelect.value;
    const selectAvailableProductsOnlyString = productStatusCheckbox.checked;
    const sortingMethodString = sortingButton.getAttribute(sortingButtonSelectedSortingMethodAttribute);

    const categoryId = getNumberOrNullFromString(categoryIdString);
    const manufacturerId = getNumberOrNullFromString(manufacturerIdString);
    const selectAvailableProductsOnly = selectAvailableProductsOnlyString === true;
    const sortingMethod = getNumberOrNullFromString(sortingMethodString);
    const pageNumberParsed = getNumberOrNull(pageNumber);

    return {
        PageNumber: pageNumberParsed,
        SearchData: searchData,
        CategoryId: categoryId,
        ManufacturerId: manufacturerId,
        AvailableOnly: selectAvailableProductsOnly,
        SortingMethod: sortingMethod,
    };
}

async function openProductDataDialogWithData(productId) {
    const productDataDialog = document.getElementById(productDataDialogId);

    if (!productDataDialog) return;

    await setContentOfProductDataDialog(productId);

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

function closeProductDataDialog()
{
    const productDataDialog = document.getElementById(productDataDialogId);

    if (!productDataDialog) return;

    stopAutoSlide(productDataDialogCarouselId);

    productDataDialog.close();
}

async function setContentOfProductDataDialog(productId) {

    const response = await fetch("/Index" + `?handler=GetProductData&productId=${productId}`,
    {
        method: "GET",
        headers: {
            "Content-Type": "application/json"
        }
    });

    if (!response.ok) return;

    const productDataHtml = await response.text();

    const productDataDialogContent = document.getElementById(productDataDialogContentId);

    if (productDataDialogContent) {
        productDataDialogContent.innerHTML = productDataHtml;
    }
}

function getAntiforgeryTokenValue() {
    return document.querySelector('input[type = "hidden"][name = "__RequestVerificationToken"]').value;
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