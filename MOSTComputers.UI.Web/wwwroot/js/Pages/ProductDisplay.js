function showXmlPopupData(productId)
{
    $("#ProductXml_popup_modal-content").load("/ProductDisplay?handler=PartialViewXmlForProduct&productId=" + productId);

    open_ProductXml_modal();
}

function copyXmlTextToClipboard()
{
    var xmlTextAreaValue = getXmlValue();

    if (xmlTextAreaValue === null) return;

    navigator.clipboard.writeText(xmlTextAreaValue);

    showNotificationWithText("copiedXmlNotificationBox", "Copied!", "copy-to-xml-success-message");
}

document.onkeydown = (e) =>
{
    if (e.ctrlKey && e.key === 's')
    {
        let xmlTextArea = document.getElementById("Xml_textarea");

        if (xmlTextArea != null)
        {
            if (xmlTextArea.value != null)
            {
                e.preventDefault();

                copyXmlTextToClipboard();

                return;
            }
        }

        let productImagesPopup = document.getElementById("ProductImages_popup");

        if (productImagesPopup === null) return;

        e.preventDefault();

        copyImageDataToClipboard();
    }
    else if (e.ctrlKey && e.key === 'a')
    {
        e.preventDefault();

        var productId = getProductId();

        getImageFileData(productId);
    }
}

function showImagePopupData(productId)
{
    $("#ProductImages_popup_modal-content").load("/ProductDisplay?handler=PartialViewImagesForProduct&productId=" + productId);

    open_ProductImages_modal();
}

function copyImageDataToClipboard()
{
    let selectedImageData = getSelectedImageData();

    navigator.clipboard.writeText(selectedImageData);

    showNotificationWithText("copiedXmlNotificationBox", "Copied!", "copy-to-xml-success-message");
}

function getImageFileData(productId)
{
    var imageIndex = getSelectedImageIndex();

    var urlFileResult = "/ProductDisplay?handler=CurrentImageFileResultSingle" + "&productId=" + productId + "&imageIndex=" + imageIndex;

    window.location.href = urlFileResult;

    showNotificationWithText("copiedXmlNotificationBox", "Saved!", "copy-to-xml-success-message");
}

let lastFunc;
let lastRan;

function throttleCust(func, limit)
{
    return function ()
    {
        let context = this;
        let args = arguments;

        if (!lastRan)
        {
            func.apply(context, args);
            lastRan = Date.now();
        }
        else
        {
            clearTimeout(lastFunc);
            lastFunc = setTimeout(function ()
            {
                if ((Date.now() - lastRan) >= limit)
                {
                    func.apply(context, args);
                    lastRan = Date.now();
                }
            }, limit - (Date.now() - lastRan));
        }
    };
}


function IDSearch_input_oninput_search()
{
    var productId = document.getElementById("IDSearch_input").value;

    if ((isNaN(productId) || isNaN(parseInt(productId)))
    && (productId.length !== 0)) return;

    var url = "/ProductDisplay?handler=SearchProductById&productId=" + productId;

    $.ajax({
        type: "GET",
        url: url,
        contentType: "application/json; charset=utf-8",
        data: null,
        headers: {
            RequestVerificationToken:
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    })
        .done(function (result)
        {
            $("#tableDataHolder").load("/ProductDisplay?handler=GetTablePartialView");
        })
        .fail(function (jqXHR, textStatus)
        {
        });
}

function LoadDisplayData(func)
{
    $("#tableDataHolder").load("/ProductDisplay?handler=GetTablePartialView", func);
}

function DisplayInputColoringAndClearExtraOutputAfterTextSearch(substring, nameOfTextDisplayinputs, length, customSpanName)
{
    var spanStart = "<span name='" + customSpanName + "' style='background-color:#FAF2B1'>";

    var searchStringLabels = document.getElementsByName(nameOfTextDisplayinputs);

    if (searchStringLabels.length > length)
    {
        var excessLength = searchStringLabels.length - length;

        $("#tableDataHolder").children().slice(0, excessLength).remove();

        [].slice.call(searchStringLabels, excessLength);
    }

    for (let i = 0; i < searchStringLabels.length; i++)
    {
        var searchStringLabel = searchStringLabels[i];

        var text = searchStringLabel.textContent;

        var textToUpperCase = searchStringLabel.textContent.toUpperCase();

        var substringIndex = textToUpperCase.indexOf(substring.toUpperCase());

        if (substringIndex != -1) 
        {
            var dataFromSubstringInActualString = text.substring(substringIndex, substringIndex + substring.length);

            text = text.replace(dataFromSubstringInActualString, spanStart + dataFromSubstringInActualString + "</span>")

            searchStringLabel.innerHTML = text;
        }
    }
}

function SearchStringSearch_input_oninput_search()
{
    var substring = document.getElementById("SearchStringSearch_input").value;

    var url = "/ProductDisplay?handler=SearchProductWhereSearchStringMatches&subString=" + substring;

    $.ajax({
        type: "GET",
        url: url,
        contentType: "application/json; charset=utf-8",
        data: null,
        headers: {
            RequestVerificationToken:
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    })
        .done(function (result)
        {
            LoadDisplayData(function()
            {
                DisplayInputColoringAndClearExtraOutputAfterTextSearch(substring, "SearchString_label", result.length, "customSpanForSearchString");
            });
        })
        .fail(function (jqXHR, textStatus)
        {
        });
}

function NameSearch_input_oninput_search()
{
    var substring = document.getElementById("NameSearch_input").value;

    var url = "/ProductDisplay?handler=SearchProductWhereNameMatches&subString=" + substring;

    $.ajax({
        type: "GET",
        url: url,
        contentType: "application/json; charset=utf-8",
        data: null,
        headers: {
            RequestVerificationToken:
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    })
        .done(function (result)
        {
            LoadDisplayData(function()
            {
                DisplayInputColoringAndClearExtraOutputAfterTextSearch(substring, "Name_label", result.length, "customSpanForName");
            });
        })
        .fail(function (jqXHR, textStatus)
        {
        });
}

function MultiSearch()
{
    var categoryId = document.getElementById("CategorySearch_select").value;
    var searchStringSubstring = document.getElementById("SearchStringSearch_input").value;
    var nameSubstring = document.getElementById("NameSearch_input").value;
    var status = document.getElementById("StatusSearch_select").value;
    var needsToBeUpdated = document.getElementById("NeedsToBeUpdatedSearch_select").value;
    var isProcessed = document.getElementById("IsProcessedSearch_select").value;

    if (categoryId === "null")
    {
        categoryId = null;
    }

    if (nameSubstring === "")
    {
        nameSubstring = null;
    }

    if (searchStringSubstring === "")
    {
        searchStringSubstring = null;
    }

    if (status === "null")
    {
        status = null;
    }

    if (needsToBeUpdated === "null")
    {
        needsToBeUpdated = null;
    }

    if (isProcessed === "null")
    {
        isProcessed = null;
    }

    var data =
    {
        CategoryId: categoryId,
        SearchStringSubstring: searchStringSubstring,
        NameSubstring: nameSubstring,
        StatusInt: status,
        NeedsToBeUpdated: needsToBeUpdated,
        IsProcessed: isProcessed
    };

    var url = "/ProductDisplay?handler=SearchProductWhereAllConditionsMatch";

    $.ajax({
        type: "POST",
        url: url,
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(data),
        headers: {
            RequestVerificationToken:
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    })
        .done(function (result)
        {
            LoadDisplayData(
            function ()
            {
                if (nameSubstring != null)
                {
                    DisplayInputColoringAndClearExtraOutputAfterTextSearch(nameSubstring, "Name_label", result.length, "customSpanForName");
                }

                if (searchStringSubstring != null)
                {
                    DisplayInputColoringAndClearExtraOutputAfterTextSearch(searchStringSubstring, "SearchString_label", result.length, "customSpanForSearchString");
                }
            });
        })
        .fail(function (jqXHR, textStatus)
        {
        });
}