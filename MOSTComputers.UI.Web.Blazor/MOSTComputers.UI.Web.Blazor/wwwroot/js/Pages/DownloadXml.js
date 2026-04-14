const downloadAllUri = "api/product/xml/all";
const downloadAllWithPromotionsUri = "api/product/xml/promotions";
const downloadAllInCategoryUri = "api/product/xml/categoryId=";
const downloadAllByManufacturerUri = "api/product/xml/manufacturerId=";

const downloadWithCurrencyQueryValue = "currency=";

const downloadPromotionGroupXmlUri = "api/promotionGroup/xml/all";

const allowedCurrencies = ["EUR", "USD", "BGN"];

const timeFormatInFileNames = "yyyy_MM_dd_HH:mm:ss";

const downloadXmlPageId = "downloadXmlPage";
const downloadXmlOptionContainerName = "downloadXmlOptionContainer";
const downloadXmlLoaderId = "downloadXmlLoader";

const xmlCurrencySelectId = "xmlCurrencySelect";
const downloadXmlCategorySelectId = "downloadXmlCategorySelect";
const downloadXmlManufacturerSelectId = "downloadXmlManufacturerSelect";

const loadingClass = "loading";

async function onDownloadAllXmlButtonClick() {
    const selectedCurrency = getSelectedCurrency();

    const url = getDownloadAllXmlUrl(selectedCurrency);

    const fileNameDatePart = getFileNameDateFormatted();

    const fileName = `MOST_XML_(${fileNameDatePart})`;

    showOrHideAllXmlOptionContainers(false);

    try {
        await fetchAndDownloadXmlFile(url, fileName);
    }
    finally {
        showOrHideAllXmlOptionContainers(true);
    }
}

async function onCopyDownloadAllUrlButtonClick() {

    const selectedCurrency = getSelectedCurrency();
    
    const uri = getDownloadAllXmlUrl(selectedCurrency);

    await copyFullUrlToClipboardFromUri(uri);
}

function getDownloadAllXmlUrl(selectedCurrency) {
    let url;

    if (selectedCurrency != null) {
        url = `${downloadAllUri}?${downloadWithCurrencyQueryValue}${selectedCurrency}`;
    }
    else {
        url = `${downloadAllUri}`;
    }

    return url;
}

async function onDownloadXmlForCategoryButtonClick() {
    const categorySelect = document.getElementById(downloadXmlCategorySelectId);

    const selectedCategoryOption = categorySelect.options[categorySelect.selectedIndex];

    const selectedCategoryId = selectedCategoryOption.value;

    if (selectedCategoryId == null || isNaN(parseInt(selectedCategoryId))) return;

    const selectedCategoryName = selectedCategoryOption.text;

    const selectedCurrency = getSelectedCurrency();

    const url = getDownloadCategoryXmlUrl(selectedCategoryId, selectedCurrency);

    const fileNameDatePart = getFileNameDateFormatted();

    let fileName;

    if (selectedCategoryName != null && selectedCategoryName != "") {
        const sanitizedCategoryName = sanitizeFileName(selectedCategoryName);

        fileName = `MOST_XML_${sanitizedCategoryName}_(${fileNameDatePart})`;
    }
    else {
        fileName = `MOST_XML_(${fileNameDatePart})`;
    }

    showOrHideAllXmlOptionContainers(false);

    try {
        await fetchAndDownloadXmlFile(url, fileName);
    }
    finally {
        showOrHideAllXmlOptionContainers(true);
    }
}

async function onCopyDownloadXmlForCategoryUrlButtonClick() {
    const categorySelect = document.getElementById(downloadXmlCategorySelectId);

    const selectedCategoryId = categorySelect.value;

    if (selectedCategoryId == null || isNaN(parseInt(selectedCategoryId))) return;
    
    const selectedCurrency = getSelectedCurrency();

    const uri = getDownloadCategoryXmlUrl(selectedCategoryId, selectedCurrency);

    await copyFullUrlToClipboardFromUri(uri);
}

function getDownloadCategoryXmlUrl(selectedCategoryId, selectedCurrency) {
    let url;

    if (selectedCurrency != null) {
        url = `${downloadAllInCategoryUri}${selectedCategoryId}?${downloadWithCurrencyQueryValue}${selectedCurrency}`;
    }
    else {
        url = `${downloadAllInCategoryUri}${selectedCategoryId}`;
    }

    return url;
}

async function onDownloadXmlForManufacturerButtonClick() {

    const manufacturerSelect = document.getElementById(downloadXmlManufacturerSelectId);

    const selectedManufacturerOption = manufacturerSelect.options[manufacturerSelect.selectedIndex];

    const selectedManufacturerId = selectedManufacturerOption.value;

    if (selectedManufacturerId == null || isNaN(parseInt(selectedManufacturerId))) return;

    const selectedManufacturerName = selectedManufacturerOption.text;

    const selectedCurrency = getSelectedCurrency();

    const url = getDownloadManufacturerXmlUrl(selectedManufacturerId, selectedCurrency);

    const fileNameDatePart = getFileNameDateFormatted();

    let fileName;

    if (selectedManufacturerName != null && selectedManufacturerName != "") {
        const sanitizedManufacturerName = sanitizeFileName(selectedManufacturerName);

        fileName = `MOST_XML_${sanitizedManufacturerName}_(${fileNameDatePart})`;
    }
    else {
        fileName = `MOST_XML_(${fileNameDatePart})`;
    }

    showOrHideAllXmlOptionContainers(false);

    try {
        await fetchAndDownloadXmlFile(url, fileName);
    }
    finally {
        showOrHideAllXmlOptionContainers(true);
    }
}

async function onCopyDownloadXmlForManufacturerUrlButtonClick() {

    const manufacturerSelect = document.getElementById(downloadXmlManufacturerSelectId);

    const selectedManufacturerId = manufacturerSelect.value;

    if (selectedManufacturerId == null || isNaN(parseInt(selectedManufacturerId))) return;
    
    const selectedCurrency = getSelectedCurrency();

    const uri = getDownloadManufacturerXmlUrl(selectedManufacturerId, selectedCurrency);

    await copyFullUrlToClipboardFromUri(uri);
}

function getDownloadManufacturerXmlUrl(selectedManufacturerId, selectedCurrency) {
    let url;

    if (selectedCurrency != null) {
        url = `${downloadAllByManufacturerUri}${selectedManufacturerId}?${downloadWithCurrencyQueryValue}${selectedCurrency}`;
    }
    else {
        url = `${downloadAllByManufacturerUri}${selectedManufacturerId}`;
    }

    return url;
}

async function onDownloadXmlWithPromotionsButtonClick() {
    const selectedCurrency = getSelectedCurrency();

    const url = getDownloadAllWithPromotionsXmlUrl(selectedCurrency);

    const fileNameDatePart = getFileNameDateFormatted();

    const fileName = `MOST_XML_ALL_PROMOTIONS_(${fileNameDatePart})`;

    showOrHideAllXmlOptionContainers(false);

    try {
        await fetchAndDownloadXmlFile(url, fileName);
    }
    finally {
        showOrHideAllXmlOptionContainers(true);
    }
}

async function onCopyDownloadXmlWithPromotionsUrlButtonClick() {

    const selectedCurrency = getSelectedCurrency();

    const uri = getDownloadAllWithPromotionsXmlUrl(selectedCurrency);

    await copyFullUrlToClipboardFromUri(uri);
}

function getDownloadAllWithPromotionsXmlUrl(selectedCurrency) {

    let url;

    if (selectedCurrency != null) {
        url = `${downloadAllWithPromotionsUri}?${downloadWithCurrencyQueryValue}${selectedCurrency}`;
    }
    else {
        url = `${downloadAllWithPromotionsUri}`;
    }

    return url;
}

async function onDownloadPromotionGroupXmlButtonClick() {

    const url = `${downloadPromotionGroupXmlUri}`;

    const fileNameDatePart = getFileNameDateFormatted();

    const fileName = `MOST_XML_PROMOTION_GROUPS_(${fileNameDatePart})`;

    showOrHideAllXmlOptionContainers(false);

    try {
        await fetchAndDownloadXmlFile(url, fileName);
    }
    finally {
        showOrHideAllXmlOptionContainers(true);
    }
}

async function onCopyDownloadPromotionGroupXmlUrlButtonClick() {

    const uri = `${downloadPromotionGroupXmlUri}`;

    await copyFullUrlToClipboardFromUri(uri);
}

async function copyFullUrlToClipboardFromUri(uri) {
    const baseUrl = window.location.protocol + "//" + window.location.host;

    const url = `${baseUrl}/${uri}`;

    await copyTextToClipboard(url);
}

async function copyTextToClipboard(text)
{
    await navigator.clipboard.writeText(text);
}

function getSelectedCurrency() {
    const xmlCurrencySelect = document.getElementById(xmlCurrencySelectId);

    const value = xmlCurrencySelect.value.toUpperCase();

    if (value == null || value == "" || value == "Base currency") {
        return null;
    }

    if (!allowedCurrencies.includes(value)) {
        return null;
    }

    return value;
}

function getFileNameDateFormatted() {
    const now = new Date();

    const year = now.getFullYear();
    const month = String(now.getMonth() + 1).padStart(2, '0');

    const day = String(now.getDate()).padStart(2, '0');

    const hours = String(now.getHours()).padStart(2, '0');
    const minutes = String(now.getMinutes()).padStart(2, '0');
    const seconds = String(now.getSeconds()).padStart(2, '0');

    return `${year}_${month}_${day}_${hours}:${minutes}:${seconds}`;
}

function showOrHideAllXmlOptionContainers(shouldBeVisible) {
    const page = document.getElementById(downloadXmlPageId);

    const loader = document.getElementById(downloadXmlLoaderId);

    const optionContainers = [...page.querySelectorAll(`[name="${downloadXmlOptionContainerName}"]`)];

    if (shouldBeVisible) {
        loader.classList.remove(loadingClass);

        for (const container of optionContainers) {
            container.classList.remove(loadingClass);
        }

        return;
    }

    loader.classList.add(loadingClass);
    
    for (const container of optionContainers) {
        container.classList.add(loadingClass);
    }
}

function sanitizeFileName(name) {
    const invalidChars = new Set([
        '<', '>', ':', '"', '/', '\\', '|', '?', '*'
    ]);

    let result = '';

    for (const ch of name) {

        const code = ch.codePointAt(0);

        if (code >= 0 && code <= 31) continue;

        if (invalidChars.has(ch)) continue;

        result += ch;
    }

    result = result.trim();

    return result;
}

async function fetchAndDownloadXmlFile(url, fileName) {
    const response = await fetch(url);

    if (!response.ok) return false;

    return await downloadXmlFile(response, fileName);
}

async function fetchAndDownloadXmlFileWithBody(url, fileName, requestMethod = "POST", requestBodyAsJson = null) {

    const response = await fetch(url,
    {
        method: requestMethod,
        headers: {
            'Content-Type': 'application/json'
        },
        body: (requestBodyAsJson == null) ? null : requestBodyAsJson
    });

    if (!response.ok) return;

    await downloadXmlFile(response, fileName);
}

async function downloadXmlFile(response, fileName) {
    const blob = await response.blob();

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