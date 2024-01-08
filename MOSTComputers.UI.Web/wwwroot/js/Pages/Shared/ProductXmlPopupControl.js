function open_ProductXml_modal()
{
    let dialog = document.getElementById("ProductXml-modal-dialog");

    dialog.style.height = window.innerHeight;

    $("#ProductXml_modal").modal("show");
}

function close_ProductXml_modal()
{
    $("#ProductXml_modal").modal("toggle");
}

function getXmlValue()
{
    var xmlTextArea = document.getElementById("Xml_textarea");

    if (xmlTextArea === null) return null;

    return xmlTextArea.value;
}