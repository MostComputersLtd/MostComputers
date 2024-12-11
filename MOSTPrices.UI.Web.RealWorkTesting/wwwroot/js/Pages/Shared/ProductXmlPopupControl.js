function getXmlValue()
{
    var xmlTextArea = document.getElementById("Xml_textarea");

    if (xmlTextArea === null) return null;

    return xmlTextArea.value;
}