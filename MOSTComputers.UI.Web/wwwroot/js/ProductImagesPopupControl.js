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