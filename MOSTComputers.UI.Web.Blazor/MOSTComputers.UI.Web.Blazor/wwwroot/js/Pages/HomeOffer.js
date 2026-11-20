const offerNumberInputId = "offerNumberInput";

const totalNetPriceTextId = "totalNetPriceText";
const totalVatPriceTextId = "totalVatPriceText";
const totalPriceTextId = "totalPriceText";

const additionalInfoInputId = "additionalInfoInput";

const saveButtonId = "saveButton";
const printButtonId = "printButton";

const itemName = "productOfferItem";
const itemQuantityInputName = "productQuantityInput";
const itemSinglePriceInputName = "productSinglePriceInput";
const itemTotalPriceTextName = "productTotalPriceText";

const itemProductIdAttribute = "data-product-id";

const singleItemPriceInputPreviousValueAttribute = "data-previous-value";

const loadingClass = "loading";

const defaultVATPercentage = 0.2;

if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", initEvents);
}
else {
    initEvents();
}

function initEvents() {

    // This is a workaround for avoiding form resubmission message when refreshing the page in the browser, but does not
    // implement any state persistance, so when you refresh the page has no items in the offer items grid.
    // history.replaceState(null, "", location.href);

    const saveButton = document.getElementById(saveButtonId);
    const printButton = document.getElementById(printButtonId);

    saveButton.addEventListener("click", onSaveButtonClicked);
    printButton.addEventListener("click", onPrintButtonClicked);

    const items = document.querySelectorAll(`[name="${itemName}"]`);

    for (const item of items) {

        const itemQuantityInput = item.querySelector(`[name="${itemQuantityInputName}"]`);
        const itemSinglePriceInput = item.querySelector(`[name="${itemSinglePriceInputName}"]`);

        itemQuantityInput.addEventListener("beforeinput", onIntegerOnlyInputBeforeInput);
        itemQuantityInput.addEventListener("input", onQuantityInputInput);

        itemSinglePriceInput.addEventListener("input", onSinglePriceInputInput);

        itemSinglePriceInput.setAttribute(singleItemPriceInputPreviousValueAttribute, itemSinglePriceInput.value);
    }
}

function onIntegerOnlyInputBeforeInput(e) {

    if (e.data === null) return;

    for (let i = 0; i < e.data.length; i++) {

        const charCode = e.data.charCodeAt(i);

        if (charCode < 48 || charCode > 57) {

            e.preventDefault();
            return;
        }
    }
}

function onQuantityInputInput(e) {

    const value = e.currentTarget.value;

    const quantity = parseInt(value);

    if (isNaN(quantity)) return;

    const item = e.currentTarget.closest(`[name="${itemName}"]`);

    const itemSinglePriceInput = item.querySelector(`[name="${itemSinglePriceInputName}"]`);
    const itemTotalPriceText = item.querySelector(`[name="${itemTotalPriceTextName}"]`);

    const singlePrice = parseFloat(itemSinglePriceInput.value);

    if (isNaN(singlePrice)) return;

    const totalPrice = quantity * singlePrice;

    itemTotalPriceText.textContent = totalPrice.toFixed(2);

    setTotalPriceTextsFromCurrentData();
}

function onSinglePriceInputInput(e) {

    const itemSinglePriceInput = e.currentTarget;

    const value = itemSinglePriceInput.value;

    const isValidCurrency = isValidCurrencyText(value);

    if (!isValidCurrency) {

        itemSinglePriceInput.value = itemSinglePriceInput.getAttribute(singleItemPriceInputPreviousValueAttribute);

        return;
    }

    const singlePrice = parseFloat(value);

    if (isNaN(singlePrice)) return;

    const item = e.currentTarget.closest(`[name="${itemName}"]`);

    const itemQuantityInput = item.querySelector(`[name="${itemQuantityInputName}"]`);
    const itemTotalPriceText = item.querySelector(`[name="${itemTotalPriceTextName}"]`);

    const quantity = parseInt(itemQuantityInput.value);

    if (isNaN(quantity)) return;

    const totalPrice = quantity * singlePrice;

    itemTotalPriceText.textContent = totalPrice.toFixed(2);

    itemSinglePriceInput.setAttribute(singleItemPriceInputPreviousValueAttribute, value);

    setTotalPriceTextsFromCurrentData();
}

function isValidCurrencyText(value) {

    let decimalPointFound = false;
    let decimalDigits = 0;

    for (let i = 0; i < value.length; i++) {

        const charCode = value.charCodeAt(i);

        if (charCode >= 48 && charCode <= 57) {

            if (decimalPointFound && ++decimalDigits > 2) {

                return false;
            }
        }
        else if (charCode === 46 && !decimalPointFound) {

            decimalPointFound = true;
        }
        else {
            return false;
        }
    }

    return true;
}

function setTotalPriceTextsFromCurrentData() {

    const totalNetPriceText = document.getElementById(totalNetPriceTextId);
    const totalVatPriceText = document.getElementById(totalVatPriceTextId);
    const totalPriceText = document.getElementById(totalPriceTextId);

    const items = document.querySelectorAll(`[name="${itemName}"]`);

    let totalNetPrice = 0;
    let totalVatPrice = 0;
    let totalPrice = 0;

    for (const item of items) {

        const itemQuantityInput = item.querySelector(`[name="${itemQuantityInputName}"]`);
        const itemSinglePriceInput = item.querySelector(`[name="${itemSinglePriceInputName}"]`);

        const quantity = parseInt(itemQuantityInput.value);
        const singlePrice = parseFloat(itemSinglePriceInput.value);

        if (isNaN(quantity) || isNaN(singlePrice)) return;

        const itemPrice = quantity * singlePrice;

        const itemVatPrice = calculateLineLevelVAT(singlePrice, quantity, defaultVATPercentage);

        const itemTotalPrice = itemPrice + itemVatPrice;

        totalNetPrice += itemPrice;
        totalVatPrice += itemVatPrice;
        totalPrice += itemTotalPrice;
    }

    totalNetPriceText.textContent = totalNetPrice.toFixed(2);
    totalVatPriceText.textContent = totalVatPrice.toFixed(2);
    totalPriceText.textContent = totalPrice.toFixed(2);
}

function calculateLineLevelVAT(unitPrice, quantity, vatRate) {

    const lineNet = unitPrice * quantity;
    const vatLine = roundStandard(lineNet * vatRate);

    return vatLine;
}

function roundStandard(value) {
    return Math.sign(value) * Math.round(Math.abs(value) * 100) / 100;
}

async function onSaveButtonClicked(e) {

    const copyButton = e.currentTarget;

    try {
        copyButton.classList.add(loadingClass);

        const requestData = getHomeOfferPdfRequestData();

        const file = await getHomeOfferPdfAsync(requestData);

        if (file == null) return;

        const filename = generateHomeOfferPdfFilename(requestData.offerNumber);

        await downloadPdfFile(file, filename);
    }
    finally {
        copyButton.classList.remove(loadingClass);
    }
}

async function downloadPdfFile(blob, fileName) {

    const link = document.createElement('a');

    link.href = URL.createObjectURL(blob);

    link.download = fileName || 'MOST_DOWNLOAD';

    link.target = "_blank";
    link.rel = "noopener noreferrer";

    document.body.appendChild(link);

    link.click();
    link.remove();

    URL.revokeObjectURL(link.href);

    return true;
}

async function onPrintButtonClicked(e) {

    const printButton = e.currentTarget;

    try {
        printButton.classList.add(loadingClass);

        const file = await getHomeOfferPdfFromCurrentData();

        if (file == null) return;

        printDocumentViaIframe(file);
    }
    finally {
        printButton.classList.remove(loadingClass);
    }
}

function printDocumentViaIframe(blob) {

    const iframe = document.createElement("iframe");

    iframe.style.display = "none";

    document.body.appendChild(iframe);

    const objectUrl = URL.createObjectURL(blob);

    iframe.addEventListener("load", () => {

        iframe.contentWindow.print();
    },
        { once: true });

    iframe.src = objectUrl;

    iframe.contentWindow.addEventListener("afterprint", () => {

        URL.revokeObjectURL(objectUrl);
        iframe.remove();
    },
        { once: true });
}

function generateHomeOfferPdfFilename(offerNumber) {

    let filename = "MOST";

    if (offerNumber != null) {

        filename += `_${offerNumber}`;
    }

    return filename + ".pdf";
}

async function getHomeOfferPdfFromCurrentData() {

    const requestData = getHomeOfferPdfRequestData();

    return await getHomeOfferPdfAsync(requestData);
}

async function getHomeOfferPdfAsync(requestData) {

    const url = "api/components/home/offer/pdf";

    const response = await fetch(url, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Accept": "application/pdf, application/problem+json"
        },
        body: JSON.stringify(requestData)
    });

    if (!response.ok) return null;

    return await response.blob();
}

function getHomeOfferPdfRequestData() {

    const offerNumberInput = document.getElementById(offerNumberInputId);
    const additionalInfoInput = document.getElementById(additionalInfoInputId);

    let offerNumber = parseInt(offerNumberInput.value);

    if (isNaN(offerNumber)) {

        offerNumber = null;
    }

    const items = document.querySelectorAll(`[name="${itemName}"]`);

    const offerItemDatas = [];

    for (const item of items) {

        const productIdAttribute = item.getAttribute(itemProductIdAttribute);

        const itemQuantityInput = item.querySelector(`[name="${itemQuantityInputName}"]`);
        const itemSinglePriceInput = item.querySelector(`[name="${itemSinglePriceInputName}"]`);

        const quantity = parseInt(itemQuantityInput.value);
        const singlePrice = parseFloat(itemSinglePriceInput.value);

        if (isNaN(quantity) || isNaN(singlePrice)) return null;

        const offerItemData = {
            id: parseInt(productIdAttribute),
            quantity: quantity,
            pricePerItemEur: singlePrice,
        };

        offerItemDatas.push(offerItemData);
    }

    return {
        offerNumber: offerNumber,
        items: offerItemDatas,
        additionalInfo: additionalInfoInput.value,
    };
}
