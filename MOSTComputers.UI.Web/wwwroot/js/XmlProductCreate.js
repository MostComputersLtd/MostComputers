function Failed_Characteristic_Name_Select_onSelect(requestIndex, propertyName, newCharacteristicId) {
    var data = { "requestIndex": requestIndex, "propertyName": propertyName, "newCharacteristicId": newCharacteristicId }
    $.ajax({
        type: "GET",
        url: "/CreatePages/XmlProductCreate?handler=AlterFailedRequest",
        contentType: "application/json; charset=utf-8",
        data: data,
        headers: {
            RequestVerificationToken:
                $('input:hidden[name="__RequestVerificationToken"]').val()
        }
    }).done(function (response) {

        $("#previewProductsContainer").load("XmlProductCreate?handler=PartialView");
    });

}