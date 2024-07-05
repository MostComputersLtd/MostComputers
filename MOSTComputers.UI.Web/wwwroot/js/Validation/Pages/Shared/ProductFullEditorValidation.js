document.addEventListener("DOMContentLoaded", function ()
{
    $('#productCompareEditor_LocalProductEditorContainer_validationForm').on('submit', function (e)
    {
        e.preventDefault();
    })
});

function addValidationForPropertyValue(propertyContentEditableDivId, validationFormId, propertyContentEditableDivValidationSummaryId)
{
    if (validationFormId == null) return;

    //var validator = $('#' + validationFormId).validate({
    //    errorPlacement: function (error, element)
    //    {
    //        error.appendTo('#' + element.attr('id') + '-validation');
    //    }
    //});

    var propertyContentEditableDiv = document.getElementById(propertyContentEditableDivId);

    var propertyContentEditableDivValidationSummary = document.getElementById(propertyContentEditableDivValidationSummaryId);

    var value = $(propertyContentEditableDiv).text().trim();
    var isRequiredText = $(propertyContentEditableDiv).data('val-required');
    var maxLength = $(propertyContentEditableDiv).data('val-maxlength-max');
    var maxLengthText = $(propertyContentEditableDiv).data('val-maxlength');

    if (isRequiredText != null && value === '')
    {
        propertyContentEditableDivValidationSummary.innerText = isRequiredText;
    }
    else if (maxLength != null && value.length > maxLength)
    {
        propertyContentEditableDivValidationSummary.innerText = maxLengthText;
    }
    else
    {
        propertyContentEditableDivValidationSummary.innerText = "";
    }
}