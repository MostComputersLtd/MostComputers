const showProductMainDataKey = "index_showProductMainData";

const productTableHideFirstRowsCheckboxId = "productTableHideFirstRowsCheckboxInput";
window.addEventListener("DOMContentLoaded", function ()
{
    var showProductMainData = this.sessionStorage.getItem(showProductMainDataKey);

    if (showProductMainData == null)
    {
        showProductMainData = true;
    }

    var shouldHide = (showProductMainData === "false");

    toggleFirstRows(shouldHide);

    var productTableHideFirstRowsCheckboxInput = document.getElementById(productTableHideFirstRowsCheckboxId);

    if (productTableHideFirstRowsCheckboxInput == null) return;

    productTableHideFirstRowsCheckboxInput.checked = shouldHide;
});

window.addEventListener("beforeunload", function ()
{
    var productTableHideFirstRowsCheckboxInput = document.getElementById(productTableHideFirstRowsCheckboxId);

    if (productTableHideFirstRowsCheckboxInput == null) return;

    this.sessionStorage.setItem(showProductMainDataKey, !productTableHideFirstRowsCheckboxInput.checked);
})

function toggleFirstRowsBasedOnCachedData()
{
    var showMainProductDataString = sessionStorage.getItem(showProductMainDataKey);

    if (showMainProductDataString == null)
    {
        showMainProductDataString = "true";
    }

    var shouldHide = (showMainProductDataString === "false");

    toggleFirstRows(shouldHide);
}

function toggleFirstRows(shouldHide)
{
    var elementsToHide = document.getElementsByClassName("hide-on-button-press");

    for (var i = 0; i < elementsToHide.length; i++)
    {
        elementsToHide[i].style.display = shouldHide ? "none" : "";
    }

    this.sessionStorage.setItem(showProductMainDataKey, !shouldHide);
}

function addNewProduct(productTableBodyId, htmlElementId, tableIndex, productHighestIdPlusOneLabelId)
{
    const url = "/Index/" + "?handler=AddNewProduct" + "&htmlElementId=" + htmlElementId + "&tableIndex=" + tableIndex;

    $.ajax({
        type: "POST",
        url: url,
        contentType: "application/json",
        data: null,
        headers: {
            RequestVerificationToken:
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    })
        .done(function (result)
        {
            if (productTableBodyId == null || result == null) return;

            var productTableBody = document.getElementById(productTableBodyId);

            if (productTableBody == null) return;

            productTableBody.innerHTML = result + productTableBody.innerHTML;

            toggleFirstRowsBasedOnCachedData();

            var productHighestIdPlusOneLabel = document.getElementById(productHighestIdPlusOneLabelId);

            if (productHighestIdPlusOneLabel == null) return;

            productHighestIdPlusOneLabel.textContent = parseInt(productHighestIdPlusOneLabel) + 1;
        })
        .fail(function (jqXHR, textStatus)
        {
        });
}

function addNewProductWithExistingProductData(
    productIdInputId,
    productHighestIdPlusOneLabelId,
    productTableBodyId,
    htmlElementId,
    tableIndex,
    productIdHiddenTableIndexInputId,
    productDataInputsAndSelectsIds)
{
    var productIdInput = document.getElementById(productIdInputId);
    var productHighestIdLabel = document.getElementById(productHighestIdPlusOneLabelId);
    var productIdHiddenTableIndexInput = document.getElementById(productIdHiddenTableIndexInputId);

    if (productIdInput == null
        || productIdInput.value == null
        || isNaN(parseInt(productIdInput.value))
        || productHighestIdLabel == null
        || productHighestIdLabel.textContent == null
        || isNaN(parseInt(productHighestIdLabel.textContent))
        || productIdHiddenTableIndexInput == null
        || productIdHiddenTableIndexInput.value == null
        || isNaN(parseInt(productIdHiddenTableIndexInput.value))
        || productDataInputsAndSelectsIds.constructor !== Array) return;

    var productToCopyFromTableIndex = parseInt(productIdHiddenTableIndexInput.value);

    productDataInputsAndSelectsIds = productDataInputsAndSelectsIds.map((elementId) =>
    {
        var indexOfSuffixBeforeTableIndex = elementId.indexOf('-');

        return elementId.slice(0, indexOfSuffixBeforeTableIndex + 1) + productToCopyFromTableIndex.toString() + elementId.slice(indexOfSuffixBeforeTableIndex + 1);
    });

    var productId = productIdInput.value;

    const highestId = parseInt(productHighestIdLabel.textContent.trim()) - 1;

    const url = "/Index" + "?handler=AddNewProductWithExistingProductData"
        + "&productToCopyFromId=" + productId
        + "&highestId=" + highestId
        + "&htmlElementId=" + htmlElementId
        + "&tableIndex=" + tableIndex;

    var productDisplayData = fetchProductLatestData(productId, productDataInputsAndSelectsIds);

    fetch(url, {
        method: "POST",
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken':
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
        body: JSON.stringify(productDisplayData)
    })
        .then(async function (response)
        {
            if (!response.ok) return;

            var result = await response.text();

            if (productTableBodyId == null || result == null) return;

            var productTableBody = document.getElementById(productTableBodyId);

            if (productTableBody == null) return;

            productTableBody.innerHTML = result + productTableBody.innerHTML;

            toggleFirstRowsBasedOnCachedData();

            productHighestIdLabel.textContent = highestId + 2;
        });
}

function getOnlyProductWithHighestId(
    productTableBodyId,
    htmlElementId,
    tableIndex,
    productAddWithExistingIdHiddenInputId = null,
    productIdHiddenInputId = null,
    productIdHiddenTableIndexInputId = null)
{
    const url = "/Index" + "?handler=GetOnlyProductWithHighestId" + "&htmlElementId=" + htmlElementId + "&tableIndex=" + tableIndex;

    $.ajax({
        type: "POST",
        url: url,
        contentType: "application/json",
        data: null,
        headers: {
            RequestVerificationToken: $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    })
        .done(function (result)
        {
            if (productTableBodyId == null || result == null) return;

            var productTableBody = document.getElementById(productTableBodyId);

            if (productTableBody == null) return;

            productTableBody.innerHTML = result;

            toggleFirstRowsBasedOnCachedData();

            if (productAddWithExistingIdHiddenInputId == null
                || productIdHiddenInputId == null) return;

            var productAddWithExistingIdHiddenInput = document.getElementById(productAddWithExistingIdHiddenInputId);

            var productIdHiddenInput = document.getElementById(productIdHiddenInputId);

            if (productAddWithExistingIdHiddenInput == null
                || productIdHiddenInput == null
                || isNaN(parseInt(productIdHiddenInput.value))) return;

            productAddWithExistingIdHiddenInput.value = productIdHiddenInput.value;

            var productIdHiddenTableIndexInput = document.getElementById(productIdHiddenTableIndexInputId);

            if (productIdHiddenTableIndexInput == null) return;

            productIdHiddenTableIndexInput.value = tableIndex;
        })
        .fail(function (jqXHR, textStatus)
        {
        });
}

function getOnlyProductWithId(
    productIdInputId,
    productTableBodyId,
    htmlElementId,
    tableIndex,
    productAddWithExistingIdHiddenInputId = null,
    productIdHiddenTableIndexInputId = null)
{
    var productIdInput = document.getElementById(productIdInputId);

    if (productIdInput == null
        || productIdInput.value == null) return;

    var productId = parseInt(productIdInput.value);

    if (isNaN(productId)) return;

    const url = "/Index" + "?handler=GetOnlyProductWithId" + "&productId=" + productId + "&htmlElementId=" + htmlElementId + "&tableIndex=" + tableIndex;

    $.ajax({
        type: "POST",
        url: url,
        contentType: "application/json",
        data: null,
        headers: {
            RequestVerificationToken: $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    })
        .done(function (result)
        {
            if (productTableBodyId == null || result == null) return;

            var productTableBody = document.getElementById(productTableBodyId);

            if (productTableBody == null) return;

            productTableBody.innerHTML = result;

            toggleFirstRowsBasedOnCachedData();

            if (productAddWithExistingIdHiddenInputId == null) return;

            var productAddWithExistingIdHiddenInput = document.getElementById(productAddWithExistingIdHiddenInputId);

            if (productAddWithExistingIdHiddenInput == null) return;

            productAddWithExistingIdHiddenInput.value = productId;

            var productIdHiddenTableIndexInput = document.getElementById(productIdHiddenTableIndexInputId);

            if (productIdHiddenTableIndexInput == null) return;

            productIdHiddenTableIndexInput.value = tableIndex;
        })
        .fail(function (jqXHR, textStatus)
        {
        });
}

function showProductXml(productId)
{
    fetch("/Index" + "?handler=Xml&productId=" + productId, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken':
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    })
        .then(async function (response)
        {
            if (response.status !== 200) return;

            var responseText = await response.text();

            open_product_xml(responseText);
        });
}

function open_product_xml(xml)
{
    const blob = new Blob([xml], { type: "text/xml" });

    const url = URL.createObjectURL(blob);

    window.open(url);

    setTimeout(() => {
        URL.revokeObjectURL(url);
    }, 1000);
}

async function saveProductWithoutSavingImagesInDAL(productId, productChangesModalAcceptButtonId, productDataInputsAndSelectsIds, changesPopupContentId)
{
    if (productChangesModalAcceptButtonId == null) return;

    await showChangesPopupData(productId, changesPopupContentId);

    var productChangesModalAcceptButton = document.getElementById(productChangesModalAcceptButtonId);

    if (productChangesModalAcceptButton == null) return;

    productChangesModalAcceptButton.addEventListener("click", (e) =>
    {
        e.preventDefault();

        var productDisplayData = fetchProductLatestData(productId, productDataInputsAndSelectsIds);

        fetch("/Index" + "?handler=SaveProductWithoutSavingImagesInDB&productId=" + productId, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken':
                    $('input:hidden[name="__RequestVerificationToken"]').val()
            },
            body: JSON.stringify(productDisplayData)
        })
            .then(async function (response)
            {
                if (response.status !== 200) return;

                var responseText = await response.text();

                var changesPopupContentElement = document.getElementById(changesPopupContentId);

                if (changesPopupContentElement == null) return;

                changesPopupContentElement.innerHTML = responseText;
            });
    });
}

async function saveProduct(productId, productChangesModalAcceptButtonId, productDataInputsAndSelectsIds, changesPopupContentId)
{
    if (productChangesModalAcceptButtonId == null) return;

    await showChangesPopupData(productId, changesPopupContentId);

    var productChangesModalAcceptButton = document.getElementById(productChangesModalAcceptButtonId);

    if (productChangesModalAcceptButton == null) return;

    productChangesModalAcceptButton.addEventListener("click", (e) =>
    {
        e.preventDefault();

        var productDisplayData = fetchProductLatestData(productId, productDataInputsAndSelectsIds);

        fetch("/Index/" + "?handler=SaveProduct&productId=" + productId, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken':
                    $('input:hidden[name="__RequestVerificationToken"]').val()
            },
            body: JSON.stringify(productDisplayData)
        })
            .then(async function (response)
            {
                if (response.status !== 200) return;

                var responseText = await response.text();

                var changesPopupContentElement = document.getElementById(changesPopupContentId);

                if (changesPopupContentElement == null) return;

                changesPopupContentElement.innerHTML = responseText;
            });
    });
}

function fetchProductLatestData(productId, productDataInputsAndSelectsIds)
{
    if (productDataInputsAndSelectsIds.constructor !== Array) return;

    var firstElement = document.getElementById(productDataInputsAndSelectsIds[0]);

    if (firstElement == null) return;

    var values =
    {
        Id: productId,
        Category: null,
        Manifacturer: null
    };

    for (var i = 0; i < productDataInputsAndSelectsIds.length; i++)
    {
        var productDataInputOrSelect = document.getElementById(productDataInputsAndSelectsIds[i]);

        var productDataInputProductPropertyName = productDataInputOrSelect.getAttribute("data-for-product-property");

        if (productDataInputProductPropertyName === "StandardWarrantyPrice")
        {
            values[productDataInputProductPropertyName] = productDataInputOrSelect.value;

            continue;
        }
        else if (productDataInputProductPropertyName === "RowGuid"
            && productDataInputOrSelect.value === "")
        {
            values[productDataInputProductPropertyName] = null;

            continue;
        }
        else if (productDataInputProductPropertyName === "ReadyForImageInsert")
        {
            values[productDataInputProductPropertyName] = (productDataInputOrSelect.value == '1');

            continue;
        }

        if (productDataInputOrSelect.value === ""
            && (productDataInputOrSelect.tagName !== "INPUT"
            || productDataInputOrSelect.getAttribute("type") !== "text"))
        {
            productDataInputOrSelect.value = null;
        }

        if ((productDataInputOrSelect.tagName === "INPUT"
            && productDataInputOrSelect.getAttribute("type") === "number")
            || (productDataInputOrSelect.tagName === "SELECT"
            && !isNaN(productDataInputOrSelect.value)))
        {
            var numberInputValue = parseFloat(productDataInputOrSelect.value);

            if (isNaN(numberInputValue))
            {
                numberInputValue = null;
            }

            values[productDataInputProductPropertyName] = numberInputValue;

            continue;
        }
        else if (productDataInputOrSelect.tagName === "INPUT"
            && productDataInputOrSelect.getAttribute("type") === "datetime")
        {
            values[productDataInputProductPropertyName] = productDataInputOrSelect.valueAsDate;

            continue;
        }

        values[productDataInputProductPropertyName] = productDataInputOrSelect.value;
    }

    return values;
}

function updateProductNewStatus(productId, productNewStatus, htmlElementId, tableIndex, productRowId)
{
    if (typeof productNewStatus !== 'number')
    {
        productNewStatus = parseInt(productNewStatus);

        if (isNaN(productNewStatus)) return;
    }

    fetch("/Index" + "?handler=UpdateProductNewStatus&productId=" + productId
        + "&productNewStatus=" + productNewStatus
        + "&htmlElementId=" + htmlElementId
        + "&tableIndex=" + tableIndex, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken':
                $('input:hidden[name="__RequestVerificationToken"]').val()
        }
    })
        .then(async function (response)
        {
            if (response.status !== 200) return;

            var data = await response.text();

            var productRow = document.getElementById(productRowId);

            if (productRow == null) return;

            productRow.insertAdjacentHTML("afterend", data);

            productRow.remove();

            toggleFirstRowsBasedOnCachedData();
        });
}

function updateProductXmlStatus(productId, productXmlStatus, htmlElementId, tableIndex, productRowId)
{
    if (typeof productXmlStatus !== 'number')
    {
        productXmlStatus = parseInt(productXmlStatus);

        if (isNaN(productXmlStatus)) return;
    }

    fetch("/Index" + "?handler=UpdateProductXmlStatus&productId=" + productId
        + "&productXmlStatus=" + productXmlStatus
        + "&htmlElementId=" + htmlElementId
        + "&tableIndex=" + tableIndex, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken':
                $('input:hidden[name="__RequestVerificationToken"]').val()
        }
    })
        .then(async function (response)
        {
            if (response.status !== 200) return;

            var data = await response.text();

            var productRow = document.getElementById(productRowId);

            if (productRow == null) return;

            productRow.insertAdjacentHTML("afterend", data);

            productRow.remove();

            toggleFirstRowsBasedOnCachedData();
        });
}

function toggleReadyForImageInsertStatus(productId, htmlElementId, tableIndex, productRowId)
{
    fetch("/Index" + "?handler=ToggleReadyForImageInsertStatus&productId=" + productId
        + "&htmlElementId=" + htmlElementId
        + "&tableIndex=" + tableIndex, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken':
                $('input:hidden[name="__RequestVerificationToken"]').val()
        }
    })
        .then(async function (response)
        {
            if (response.status !== 200) return;

            var data = await response.text();

            var productRow = document.getElementById(productRowId);

            if (productRow == null) return;

            productRow.insertAdjacentHTML("afterend", data);

            productRow.remove();

            toggleFirstRowsBasedOnCachedData();
        });
}

function deleteProduct(productId, productTableRowId)
{
    fetch("/Index/" + "?handler=DeleteProduct&productId=" + productId, {
        method: 'DELETE',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken':
                $('input:hidden[name="__RequestVerificationToken"]').val()
        }
    })
        .then(response => 
        {
            if (!response.ok) return;

            var productTableRow = document.getElementById(productTableRowId);

            if (productTableRow == null) return;

            productTableRow.remove();
        });
}

function showFirstImagePopupData(productId, firstImagePopupContentId)
{
    return new Promise((resolve, reject) =>
    {
        $("#" + firstImagePopupContentId).load("/Index?handler=GetProductFirstImagePopupPartialViewForProduct&productId=" + productId,
            function (response, status, xhr)
            {
                if (status == "error")
                {
                    reject(xhr.statusText);
                }
                else
                {
                    resolve(response);

                    open_ProductFirstImage_modal();
                }
            }
        );
    });
}

function showImagesPopupData(productId, imagePopupUsage, imagesPopupContentId)
{
    if (imagePopupUsage == null) return;

    return new Promise((resolve, reject) =>
    {
        $("#" + imagesPopupContentId).load("/Index?handler=GetPartialViewImagesForProduct&productId=" + productId
            + "&productImagePopupUsage=" + imagePopupUsage,
            function (response, status, xhr)
            {
                if (status == "error")
                {
                    reject(xhr.statusText);
                }
                else
                {
                    resolve(response);

                    open_ProductImages_modal();
                }
            }
        );
    });
}

function showProductFullWithXmlPopupData(productId, fullWithXmlpopupContentId)
{
    return new Promise((resolve, reject) =>
    {
        $("#" + fullWithXmlpopupContentId).load("/Index?handler=GetProductFullPopupPartialViewForProduct&productId=" + productId,
            function (response, status, xhr)
            {
                if (status == "error")
                {
                    reject(xhr.statusText);
                }
                else
                {
                    resolve(response);

                    $('#product_full_popup_carousel').carousel();

                    open_ProductFullWithXml_modal();
                }
            }
        );
    });
}

function showChangesPopupData(productId, changesPopupContentId, getEvenIfProductDoesntExist = false)
{
    return new Promise((resolve, reject) =>
    {
        $("#" + changesPopupContentId).load("/Index?handler=GetProductChangesPopupPartialViewForProduct&productId=" + productId
            + "&getEvenIfProductDoesntExist=" + getEvenIfProductDoesntExist,
            function (response, status, xhr)
            {
                if (status == "error")
                {
                    reject(xhr.statusText);
                }
                else
                {
                    resolve(response);

                    open_ProductChanges_modal();
                }
            }
        );
    });
}

function triggerChangesCheck(notificationBoxId)
{
    fetch("/Index/" + "?handler=TriggerChangeCheck", {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken':
                $('input:hidden[name="__RequestVerificationToken"]').val()
        }
    })
        .then(response =>
        {
            if (!response.ok)
            {
                showNotificationWithText(notificationBoxId, "A failure occured", "notificationBox-long-message");

                return;
            }

            showNotificationWithText(notificationBoxId, "Success", "notificationBox-short-message");
        });
}

function redirectToPropertiesEditor(productId)
{
    window.location.href = "ProductPropertiesEditor/" + productId;
}