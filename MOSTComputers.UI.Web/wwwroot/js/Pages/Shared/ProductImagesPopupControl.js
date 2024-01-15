function open_ProductImages_modal()
{
    let dialog = document.getElementById("ProductImages-modal-dialog");

    dialog.style.height = window.innerHeight;

    $("#ProductImages_modal").modal("show");
}

function close_ProductImages_modal()
{
    $("#ProductImages_modal").modal("toggle");
}

function displayImage(data, name)
{
    document.getElementById("productCurrentImageDisplay").src = data;
    document.getElementById("productCurrentImageDisplay").setAttribute("name", name);
}

function scrollHorizontally(e)
{
    e = window.event || e;
    var delta = Math.max(-1, Math.min(1, (e.wheelDelta || -e.detail)));
    document.getElementById("productImageDisplay_ul").scrollLeft -= (delta * 30);
    e.preventDefault();
}

function getSelectedImageIndex()
{
    let currentSelectedImage = document.getElementById("productCurrentImageDisplay");

    if (currentSelectedImage === null) return null;

    var nameOfActiveImage = currentSelectedImage.getAttribute("name");

    var indexOfTagInName = nameOfActiveImage.indexOf('#');

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
    let productIdInput = document.getElementById("modal-title_productIdShow");

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

function productImageDisplay_ul_li_ondragend(e)
{
    setTimeout(() =>
    {
        e.target.classList.remove("draggedImageLi");
    }, 0);

    const productId = getProductId();

    var newIndexOfDraggedItem = getImageDisplayOrder(e.target);

    if (indexOfDraggedItem === newIndexOfDraggedItem) return;

    const url = "/ProductDisplay/" + "?handler=UpdateImageOrder" + "&productId=" + productId + "&oldDisplayOrder=" + indexOfDraggedItem + "&newDisplayOrder=" + newIndexOfDraggedItem;

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

            //var imagesPopupModalContent = document.getElementById("ProductImages_popup_modal-content");

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

function addNewImage(fileInputElement)
{
    var productId = getProductId();

    if (productId === null) return;

    var formBody = new FormData();

    formBody.append("fileInfo", fileInputElement.files[0])

    var url = "/ProductDisplay/" + "?handler=AddNewImageToProduct" + "&productId=" + productId;

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
        .done(function (result) {

            var imagesPopupModalContent = document.getElementById("ProductImages_popup_modal-content");

            imagesPopupModalContent.innerHTML = result;
        })
        .fail(function (jqXHR, textStatus) {
        });
}

function delete_productImageDisplay_ul_li(elementId, imageId)
{
    const url = "/ProductDisplay/" + "?handler=DeleteImageFromProduct" + "&imageId=" + imageId;

    $.ajax({
        type: "DELETE",
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
            const productImageDisplayList = document.getElementById("productImageDisplay_ul");

            const productImageListItemToDelete = document.getElementById(elementId);

            productImageDisplayList.removeChild(productImageListItemToDelete)
        })
        .fail(function (jqXHR, textStatus)
        {
        });
}