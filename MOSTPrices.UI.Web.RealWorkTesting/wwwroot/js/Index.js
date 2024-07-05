function toggleFirstRows(shouldHide)
{
    var elementsToHide = document.getElementsByClassName("hide-on-button-press");

    for (var i = 0; i < elementsToHide.length; i++)
    {
        elementsToHide[i].style.display = shouldHide ? "none" : "";
    }
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

            var productHighestIdPlusOneLabel = document.getElementById(productHighestIdPlusOneLabelId);

            if (productHighestIdPlusOneLabel == null
                || productHighestIdPlusOneLabel.value == null
                || isNaN(parseInt(productHighestIdPlusOneLabel))) return;

            productHighestIdPlusOneLabel.value = parseInt(productHighestIdPlusOneLabel) + 1;
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
            var result = await response.text();

            if (productTableBodyId == null || result == null) return;

            productHighestIdLabel.value = highestId + 2;

            var productTableBody = document.getElementById(productTableBodyId);

            if (productTableBody == null) return;

            productTableBody.innerHTML = result + productTableBody.innerHTML;
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
async function saveProductWithoutSavingImagesInDAL(productId, productChangesModalAcceptButtonId, productDataInputsAndSelectsIds)
{
    if (productChangesModalAcceptButtonId == null) return;

    await showChangesPopupData(productId);

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
        });
    });
}

async function saveProduct(productId, productChangesModalAcceptButtonId, productDataInputsAndSelectsIds)
{
    if (productChangesModalAcceptButtonId == null) return;

    await showChangesPopupData(productId);

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
        });
    })
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

function updateProductNewStatus(productId, productNewStatus)
{
    if (typeof productNewStatus !== 'number')
    {
        productNewStatus = parseInt(productNewStatus);

        if (isNaN(productNewStatus)) return;
    }

    fetch("/Index" + "?handler=UpdateProductNewStatus&productId=" + productId + "&productNewStatus=" + productNewStatus, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken':
                $('input:hidden[name="__RequestVerificationToken"]').val()
        }
    });
}

function updateProductXmlStatus(productId, productXmlStatus)
{
    if (typeof productXmlStatus !== 'number')
    {
        productXmlStatus = parseInt(productXmlStatus);

        if (isNaN(productXmlStatus)) return;
    }

    fetch("/Index" + "?handler=UpdateProductXmlStatus&productId=" + productId + "&productXmlStatus=" + productXmlStatus, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken':
                $('input:hidden[name="__RequestVerificationToken"]').val()
        }
    });
}

function toggleReadyForImageInsertStatus(productId, readyForImageInsertValueInputId)
{
    fetch("/Index" + "?handler=ToggleReadyForImageInsertStatus&productId=" + productId, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken':
                $('input:hidden[name="__RequestVerificationToken"]').val()
        }
    })
        .then(async response =>
        {
            var readyForImageInsertValueInput = document.getElementById(readyForImageInsertValueInputId);

            if (readyForImageInsertValueInput == null) return;

            var json = await response.json();

            if (json == null
                || json.updatedReadyForImageInsertValue == null) return;

            readyForImageInsertValueInput.value = json.updatedReadyForImageInsertValue ? 1 : 0;
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

function showFirstImagePopupData(productId)
{
    return new Promise((resolve, reject) =>
    {
        $("#ProductFirstImage_popup_modal_content").load("/Index?handler=GetProductFirstImagePopupPartialViewForProduct&productId=" + productId,
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

function showImagesPopupData(productId)
{
    $("#ProductImages_popup_modal_content").load("/Index?handler=GetPartialViewImagesForProduct&productId=" + productId);

    open_ProductImages_modal();
}

function showChangesPopupData(productId, getEvenIfProductDoesntExist = false)
{
    return new Promise((resolve, reject) =>
    {
        $("#ProductChanges_popup_modal_content").load("/Index?handler=GetProductChangesPopupPartialViewForProduct&productId=" + productId
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

function triggerChangesCheck()
{
    fetch("/Index/" + "?handler=TriggerChangeCheck", {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken':
                $('input:hidden[name="__RequestVerificationToken"]').val()
        }
    })
        .then(response => {
            if (!response.ok) return;

            var productTableRow = document.getElementById(productTableRowId);

            if (productTableRow == null) return;

            productTableRow.remove();
        });
}

function redirectToPropertiesEditor(productId)
{
    window.location.href = "ProductPropertiesEditor/" + productId;
}