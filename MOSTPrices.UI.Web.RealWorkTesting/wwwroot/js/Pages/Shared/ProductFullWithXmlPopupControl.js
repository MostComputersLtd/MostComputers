function open_ProductFullWithXml_modal()
{
    let dialog = document.getElementById("ProductFullWithXml_modal_dialog");

    dialog.style.height = window.innerHeight;

    $("#ProductFullWithXml_modal").modal("show");
}

function close_ProductFullWithXml_modal()
{
    $("#ProductFullWithXml_modal").modal("toggle");
}