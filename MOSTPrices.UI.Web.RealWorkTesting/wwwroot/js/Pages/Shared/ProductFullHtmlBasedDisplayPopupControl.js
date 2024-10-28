function open_ProductFullHtmlBased_modal()
{
    let dialog = document.getElementById("ProductFullHtmlBased_modal_dialog");

    dialog.style.height = window.innerHeight;

    $("#ProductFullHtmlBased_modal").modal("show");
}

function close_ProductFullHtmlBased_modal()
{
    $("#ProductFullHtmlBased_modal").modal("toggle");
}