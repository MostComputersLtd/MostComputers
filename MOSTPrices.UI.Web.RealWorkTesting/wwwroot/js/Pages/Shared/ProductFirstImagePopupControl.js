function close_ProductFirstImage_modal()
{
    $("#ProductFirstImage_modal").modal("toggle");
}

function open_ProductFirstImage_modal()
{
    let dialog = document.getElementById("ProductFirstImage_modal_dialog");

    dialog.style.height = window.innerHeight;

    $("#ProductFirstImage_modal").modal("show");
}