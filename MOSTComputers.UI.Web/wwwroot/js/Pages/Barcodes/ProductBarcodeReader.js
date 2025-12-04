const productIdInputElementId = "productBarcodeReaderProductIdInput";

const videoElementContainerId = "productBarcodeReaderVideoContainer";
const videoElementId = "productBarcodeReaderVideo";
const scanButtonElementId = "productBarcodeReaderScanButton";

const scanOverlayElementId = "productBarcodeReaderScanOverlay";

const gtinCodeResultsContainerElementId = "productBarcodeReaderGTINCodeResultsContainer";
const gtinCodeResultsContainerElementInsertItemElementIndex = "data-insert-element-index";

const gtinCodeResultsItemElementIdPrefix = "productBarcodeReaderGTINCodeResultsItem-";
const gtinCodeResultsItemElementName = "productBarcodeReaderGTINCodeResultsItem";

const gtinCodeSelectElementIdPrefix = "productBarcodeReaderGTINCodeSelect-";

async function decodeProductGTINCodeAndDisplayChanges(elementIndex = null)
{
    const addingNewItem = elementIndex == null;

    const codeType = getCodeTypeFromElementIndex(elementIndex);

    const device = await getVideoInputDevice();

    const videoElement = document.getElementById(videoElementId);

    const scanOverlayElement = document.getElementById(scanOverlayElementId);

    videoElement.srcObject = device;

    videoElement.play();

    scanOverlayElement.classList.add("started-scan");

    toggleElementDisplay(videoElementContainerId);

    const barcodeReader = getDefaultBarcodeReader();

    const productEANCode = await decodeBarcodeFromVideoRegion(barcodeReader, videoElement, 300, 300);

    toggleElementDisplay(videoElementContainerId);

    scanOverlayElement.classList.remove("started-scan");

    const enableSaveButton = codeType != null && !isNaN(codeType);

    if (addingNewItem)
    {
        elementIndex = getProductGTINCodeListItemInsertIndex();
    }

    await getProductGTINCodeListItemPartialAndDisplayAtIndex(elementIndex, productEANCode, codeType, enableSaveButton);

    if (addingNewItem)
    {
        const newInsertIndex = elementIndex + 1;

        setProductGTINCodeListItemInsertIndex(newInsertIndex);
    }
}

async function decodeProductGTINCodeFromFileAndDisplayChanges(fileInputElementId)
{
    const fileInputElement = document.getElementById(fileInputElementId);

    const firstImageFile = Array.from(fileInputElement.files).find(file => file.type.startsWith('image/'));

    const barcodeReader = getDefaultBarcodeReader();

    const productGTINCode = await decodeBarcodeFromImageUrl(barcodeReader, firstImageFile);

    const newElementIndex = getProductGTINCodeListItemInsertIndex();

    await getProductGTINCodeListItemPartialAndDisplayAtIndex(newElementIndex, productGTINCode, null, false);

    const newInsertIndex = newElementIndex + 1;

    setProductGTINCodeListItemInsertIndex(newInsertIndex);
}

async function getProductGTINCodeListItemPartialAndDisplayAtIndex(elementIndex, productGTINCode, codeType, enableSaveButton)
{
    const response = await getProductGTINCodeListItemPartial(elementIndex, productGTINCode, codeType, enableSaveButton);

    redirectIfResponseIsRedirected(response);

    if (!response.ok) return;

    const responseText = await response.text();

    upsertGTINCodeItemToList(responseText, elementIndex);
}

async function getProductGTINCodeListItemPartial(elementIndex, productGTINCode, codeType, enableSaveButton)
{
    var url = "/Barcodes/ProductBarcodeReader" + "?handler=GetGTINCodeListItemPartial"
        + "&elementIndex=" + elementIndex
        + "&gtinCode=" + productGTINCode;

    if (codeType != null && !isNaN(codeType))
    {
        url += "&selectedGtinCodeType=" + codeType;
    }

    if (enableSaveButton != null)
    {
        url += "&enableSaveButton=" + enableSaveButton;
    }

    return await fetch(url,
    {
        method: 'GET',
        headers:
        {
            'Content-Type': 'application/json',
            'RequestVerificationToken':
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    });
}

async function upsertProductGTINCodeAndDisplayChangesFromEditor(elementIndex, productGTINCode)
{
    const productIdInputElement = document.getElementById(productIdInputElementId);

    const productId = productIdInputElement ? parseInt(productIdInputElement.value) : null;

    const codeType = getCodeTypeFromElementIndex(elementIndex);

    await upsertProductGTINCodeAndDisplayChanges(elementIndex, productId, productGTINCode, codeType);
}

async function upsertProductGTINCodeAndDisplayChanges(elementIndex, productId, productGTINCode, codeType)
{
    const response = await upsertProductGTINCode(elementIndex, productId, productGTINCode, codeType);

    redirectIfResponseIsRedirected(response);

    if (!response.ok) return;

    const responseText = await response.text();

    upsertGTINCodeItemToList(responseText, elementIndex);
}

async function upsertProductGTINCode(elementIndex, productId, productGTINCode, codeType)
{
    var url = "/Barcodes/ProductBarcodeReader" + "?handler=UpsertGTINCodeToProduct"
        + "&elementIndex=" + elementIndex
        + "&gtinCode=" + productGTINCode;

    if (!isNaN(productId))
    {
        url += "&productId=" + productId;
    }

    if (!isNaN(codeType))
    {
        url += "&codeType=" + codeType;
    }

    return await fetch(url,
    {
        method: 'POST',
        headers:
        {
            'Content-Type': 'application/json',
            'RequestVerificationToken':
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    });
}

async function decodeProductSerialNumberAndDisplayChanges()
{
    const device = await getVideoInputDevice();

    const productSerialNumber = await decodeProductSerialNumberBarcodeAndPutOnVideo(device.deviceId, videoElementId);

    addProductSerialNumberAndDisplayChanges(productSerialNumber);
}

async function addProductSerialNumberAndDisplayChanges(productSerialNumber)
{
    const response = await addProductSerialNumber(productSerialNumber);

    redirectIfResponseIsRedirected(response);

    if (!response.ok) return;

    const responseText = await response.text();

    updateElementInnerHTML(gtinCodeResultsContainerElementId, responseText);
}

async function addProductSerialNumber(productSerialNumber)
{
    const url = "/Barcodes/ProductBarcodeReader" + "?handler=SerialNumber"
        + "&serialNumber=" + productSerialNumber;

    return await fetch(url,
    {
        method: 'POST',
        headers:
        {
            'Content-Type': 'application/json',
            'RequestVerificationToken':
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    });
}

async function deleteProductGTINCodeAndDisplayChanges(elementIndex)
{
    const productIdInputElement = document.getElementById(productIdInputElementId);

    const productId = productIdInputElement ? parseInt(productIdInputElement.value) : null;

    var codeType = getCodeTypeFromElementIndex(elementIndex);

    const response = await deleteProductGTINCode(productId, codeType);

    redirectIfResponseIsRedirected(response);

    if (response.ok || response.status == 404)
    {
        return;
    }

    const elementAtIndex = document.getElementById(gtinCodeResultsItemElementIdPrefix + elementIndex);

    if (!elementAtIndex) return;

    elementAtIndex.remove();
}

async function deleteProductGTINCode(productId, codeType)
{
    const url = "/Barcodes/ProductBarcodeReader" + "?handler=DeleteProductGTINCode"
        + "&productId=" + productId
        + "&codeType=" + codeType;

    return await fetch(url,
    {
        method: 'DELETE',
        headers:
        {
            'Content-Type': 'application/json',
            'RequestVerificationToken':
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    });
}

async function getVideoInputDevice()
{
    return await navigator.mediaDevices.getUserMedia({ video: true });
}

function getProductGTINCodeListItemInsertIndex()
{
    const gtinCodeResultsContainerElement = document.getElementById(gtinCodeResultsContainerElementId);

    const insertElementIndexAsString = gtinCodeResultsContainerElement.getAttribute(gtinCodeResultsContainerElementInsertItemElementIndex);

    const insertElementIndex = parseInt(insertElementIndexAsString);

    if (isNaN(insertElementIndex)) return 0;

    return insertElementIndex;
}

function getCodeTypeFromElementIndex(elementIndex)
{
    const codeType = null;

    if (elementIndex != null && !isNaN(elementIndex))
    {
        const gtinCodeSelectElement = document.getElementById(gtinCodeSelectElementIdPrefix + elementIndex);

        const codeTypeAsString = gtinCodeSelectElement?.options[gtinCodeSelectElement.selectedIndex].value;

        codeType = parseInt(codeTypeAsString);
    }

    return codeType;
}

function setProductGTINCodeListItemInsertIndex(newInsertIndex)
{
    const gtinCodeResultsContainerElement = document.getElementById(gtinCodeResultsContainerElementId);

    gtinCodeResultsContainerElement.setAttribute(gtinCodeResultsContainerElementInsertItemElementIndex, newInsertIndex);
}

function updateElementInnerHTML(elementId, newInnerHtml)
{
    if (!elementId) return;

    const element = document.getElementById(elementId);

    if (!element) return;

    element.innerHTML = newInnerHtml;
}

function upsertGTINCodeItemToList(gtinCodeListItemPartial, existingGTINCodeListItemIndex = null)
{
    const gtinCodeListItem = getElementFromText(gtinCodeListItemPartial);

    const gtinCodeResultsContainerElement = document.getElementById(gtinCodeResultsContainerElementId);

    var existingGTINCodeListItem = null;

    if (existingGTINCodeListItemIndex != null && !isNaN(existingGTINCodeListItemIndex))
    {
        existingGTINCodeListItem = document.getElementById(gtinCodeResultsItemElementIdPrefix + existingGTINCodeListItemIndex);
    }

    if (existingGTINCodeListItem)
    {
        gtinCodeResultsContainerElement.replaceChild(gtinCodeListItem, existingGTINCodeListItem);
    }
    else
    {
        gtinCodeResultsContainerElement.appendChild(gtinCodeListItem);
    }
}