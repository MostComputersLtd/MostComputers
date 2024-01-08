function showXmlPreviewData(xmlString)
{
    var data = { "XMLInput": xmlString };

    $.ajax({
        type: "GET",
        url: "/CreatePages/XmlProductCreate?handler=ShowOnly",
        contentType: "application/json; charset=utf-8",
        data: data,
        headers: {
            RequestVerificationToken:
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    })
        .done(function (result) {
            $("#previewProductsContainer").load("XmlProductCreate?handler=PartialView");
        })
        .fail(function (jqXHR, textStatus) {

            let boldErrorText = document.createElement("strong");

            boldErrorText.textContent = jqXHR.responseText;

            boldErrorText.className = "invalid-xml-error-text";

            document.getElementById("previewProductsContainer").innerHTML = "";

            document.getElementById("previewProductsContainer").appendChild(boldErrorText);
        });
}

function Failed_Characteristic_Name_Select_onSelect(requestIndex, propertyName, newCharacteristicId)
{
    var data = { "requestIndex": requestIndex, "propertyName": propertyName, "newCharacteristicId": newCharacteristicId };

    $.ajax({
        type: "GET",
        url: "/CreatePages/XmlProductCreate?handler=AlterFailedRequest",
        contentType: "application/json; charset=utf-8",
        data: data,
        headers: {
            RequestVerificationToken:
                $('input:hidden[name="__RequestVerificationToken"]').val()
        }
    }).done(function (response)
    {
        $("#previewProductsContainer").load("XmlProductCreate?handler=PartialView");
    });
}