function open_ProductChanges_modal()
{
    let dialog = document.getElementById("ProductChanges_modal_dialog");

    dialog.style.height = window.innerHeight;

    $("#ProductChanges_modal").modal("show");
}

function close_ProductChanges_modal()
{
    $("#ProductChanges_modal").modal("toggle");
}