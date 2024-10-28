const htmlDataDisplayCurrentImageIndexAttributeName = "data-html-data-display-current-image-index";

function open_ProductImages_modal()
{
    let dialog = document.getElementById("ProductImages_modal_dialog");

    dialog.style.height = window.innerHeight;

    $("#ProductImages_modal").modal("show");
}

function close_ProductImages_modal()
{
    $("#ProductImages_modal").modal("toggle");
}

function scrollHorizontally(e)
{
    var delta = Math.max(-1, Math.min(1, (e.wheelDelta || -e.detail)));
    document.getElementById("productImageDisplay_ul").scrollLeft -= (delta * 30);
    e.preventDefault();
}

function getSelectedImageIndex()
{
    let currentSelectedImage = document.getElementById("productCurrentImageDisplay");

    if (currentSelectedImage === null) return null;

    var nameOfActiveImage = currentSelectedImage.getAttribute("name");

    var indexOfTagInName = nameOfActiveImage.indexOf('-');

    var imageIndex = nameOfActiveImage.substring(indexOfTagInName + 1);

    return imageIndex;
}

function getSelectedImageData()
{
    let selectedImage = document.getElementById("productCurrentImageDisplay");

    if (selectedImage === null) return null;

    return selectedImage.src;
}

function getProductId()
{
    let productIdInput = document.getElementById("modal_title_productIdShow");

    if (productIdInput === null) return;

    var productIdText = productIdInput.innerText;

    if (productIdText === null) return;

    var startIndexOfProductId = productIdText.indexOf("ID: ") + 4;

    var productId = productIdText.substring(startIndexOfProductId);

    return parseInt(productId);
}

var indexOfDraggedItem;

function productImageDisplay_ul_li_ondragstart(e)
{
    indexOfDraggedItem = getImageDisplayOrder(e.target);

    setTimeout(() =>
    {
        e.target.classList.add("draggedImageLi");
    }, 0);

}

function productImageDisplay_ul_ondragover(e)
{
    const productImageDisplayList = document.getElementById("productImageDisplay_ul");

    const childrenOfListThatArentDragged = [...productImageDisplayList.querySelectorAll("li:not(.draggedImageLi)")];

    const itemThatIsDragged = productImageDisplayList.querySelector(".draggedImageLi");

    var nextSibling = childrenOfListThatArentDragged.find(child =>
    {
        return e.clientX <= child.getBoundingClientRect().left + (child.clientWidth / 2);
    });

    if (nextSibling != undefined)
    {
        productImageDisplayList.insertBefore(itemThatIsDragged, nextSibling);
    }
}

function productImageDisplay_ul_li_ondragend(e, productImagePopupUsageEnum)
{
    setTimeout(() =>
    {
        e.target.classList.remove("draggedImageLi");
    }, 0);

    const productId = getProductId();

    var newIndexOfDraggedItem = getImageDisplayOrder(e.target);

    if (indexOfDraggedItem === newIndexOfDraggedItem) return;

    const url = "/Index" + "?handler=UpdateImageDisplayOrder"
        + "&productId=" + productId
        + "&oldDisplayOrder=" + indexOfDraggedItem
        + "&newDisplayOrder=" + newIndexOfDraggedItem
        + "&productImagePopupUsage=" + productImagePopupUsageEnum;

    $.ajax({
        type: "PUT",
        url: url,
        contentType: "application/json",
        data: null,
        headers: {
            RequestVerificationToken:
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    })
        .done(function (result) {
            
            //var imagesPopupModalContent = document.getElementById("ProductImages_popup_modal_content");

            //imagesPopupModalContent.innerHTML = result;
        })
        .fail(function (jqXHR, textStatus) {
        });
}

function getImageDisplayOrder(target)
{
    const productImageDisplayList = document.getElementById("productImageDisplay_ul");

    const childrenArray = Array.from(productImageDisplayList.children);

    var newIndexOfDraggedItem = childrenArray.indexOf(target);

    return newIndexOfDraggedItem + 1;
}

function displayImageHtmlData(htmlDataViewId, htmlDataInputId, rawHtmlDataDisplayId)
{
    if (htmlDataViewId == null
        || htmlDataInputId == null) return;

    var htmlDataView = document.getElementById(htmlDataViewId);
    var htmlDataInput = document.getElementById(htmlDataInputId);
    var rawHtmlDataDisplay = document.getElementById(rawHtmlDataDisplayId);

    if (htmlDataView == null
        || htmlDataInput == null) return;

    rawHtmlDataDisplay.style.display = "none";

    htmlDataView.innerHTML = htmlDataInput.value;

    var elementImageIndexAsString = getLastPartInSeparatedStringInElementId(htmlDataInputId, "-");

    htmlDataView.setAttribute(htmlDataDisplayCurrentImageIndexAttributeName, elementImageIndexAsString);

    htmlDataView.style.display = "";
}

function toggleHtmlDataAndRawHtmlData(
    htmlDataInputPrefix, htmlDataDisplayId, rawHtmlDataInputPrefix, rawHtmlDataDisplayTextAreaId, rawHtmlDataDisplayId)
{
    var htmlDataDisplay = document.getElementById(htmlDataDisplayId);
    var rawHtmlDataDisplayInput = document.getElementById(rawHtmlDataDisplayTextAreaId);
    var rawHtmlDataDisplay = document.getElementById(rawHtmlDataDisplayId);

    var currentImageIndex = htmlDataDisplay.getAttribute(htmlDataDisplayCurrentImageIndexAttributeName);

    if (currentImageIndex == null
        || currentImageIndex === "")
    {
        currentImageIndex = rawHtmlDataDisplayInput.getAttribute(htmlDataDisplayCurrentImageIndexAttributeName);
    }

    var htmlDataInputId = htmlDataInputPrefix + currentImageIndex;
    var rawHtmlDataInputId = rawHtmlDataInputPrefix + currentImageIndex;

    var rawHtmlDataInput = document.getElementById(rawHtmlDataInputId);

    if (htmlDataDisplay.style.display === "none")
    {
        displayImageHtmlData(htmlDataDisplayId, htmlDataInputId, rawHtmlDataDisplayId);

        return;
    }

    htmlDataDisplay.style.display = "none";

    rawHtmlDataDisplayInput.textContent = rawHtmlDataInput.value;

    var elementImageIndexAsString = getLastPartInSeparatedStringInElementId(rawHtmlDataInput, "-");

    rawHtmlDataDisplayInput.setAttribute(htmlDataDisplayCurrentImageIndexAttributeName, elementImageIndexAsString);

    rawHtmlDataDisplay.style.display = "";
}

function addNewImage(fileInputElement, imagePopupUsage)
{
    var productId = getProductId();

    if (productId == null
        || imagePopupUsage == null) return;

    var formBody = new FormData();

    formBody.append("fileInfo", fileInputElement.files[0])

    var url = "/Index" + "?handler=AddNewImageToProduct" + "&productId=" + productId + "&productImagePopupUsage=" + imagePopupUsage;

    $.ajax({
        type: "POST",
        url: url,
        contentType: false,
        processData: false,
        data: formBody,
        headers: {
            RequestVerificationToken:
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    })
        .done(function (result)
        {

            var imagesPopupModalContent = document.getElementById("ProductImages_popup_modal_content");

            imagesPopupModalContent.innerHTML = result;
        })
        .fail(function (jqXHR, textStatus)
        {
        });
}

function update_productImageDisplay_ul_li_activeState(elementId, elementCheckBoxId, productId, displayOrder, fileName)
{
    if (displayOrder < 0) return;

    var elementCheckbox = document.getElementById(elementCheckBoxId);

    var checkboxIsChecked = elementCheckbox.checked;

    const url = "/Index" + "?handler=UpdateImageActiveStatus" + "&productId=" + productId + "&displayOrder=" + displayOrder + "&active=" + checkboxIsChecked;

    $.ajax({
        type: "PUT",
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
            var opacityOfLi = checkboxIsChecked ? 1 : 0.35;

            document.getElementById(elementId).style.opacity = opacityOfLi;
        })
        .fail(function (jqXHR, textStatus)
        {
        });
}

function delete_productImageDisplay_ul_li(productId, imageIndex, imagePopupUsage, elementId)
{
    if (productId == null
        || imageIndex == null
        || imagePopupUsage == null) return;

    const url = "/Index" + "?handler=DeleteImageFromProduct" + "&productId=" + productId + "&imageIndex=" + imageIndex + "&productImagePopupUsage=" + imagePopupUsage;

    fetch(url, {
        method: "DELETE",
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken':
                $('input:hidden[name="__RequestVerificationToken"]').val()
        }
    })
        .then(function (response)
        {
            if (response.status !== 200) return;

            const productImageDisplayList = document.getElementById("productImageDisplay_ul");

            const productImageListItemToDelete = document.getElementById(elementId);

            productImageDisplayList.removeChild(productImageListItemToDelete)
        });
}

function getLastPartInSeparatedStringInElementId(elementId, separator)
{
    if (typeof elementId !== "string") return null;

    var stringParts = elementId.split(separator);

    return stringParts[stringParts.length - 1];
}