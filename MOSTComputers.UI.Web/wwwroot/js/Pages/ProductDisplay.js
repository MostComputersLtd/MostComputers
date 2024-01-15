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

const spanStartPlaceholder = "|?";
const spanEndPlaceholder = "|/";

function DisplayMultipleInputColoringAndClearExtraOutputAfterTextSearch(wholeSearchStringText, nameOfTextDisplayinputs, length, customSpanName)
{
    var spanStart = "<span name='" + customSpanName + "' style='background-color:#FAF2B1'>";
    const spanEnd = "</span>";

    var searchStringLabels = Array.from(document.getElementsByName(nameOfTextDisplayinputs));

    if (searchStringLabels.length > length)
    {
        var excessLength = searchStringLabels.length - length;

        $("#tableDataHolder").children().slice(0, excessLength).remove();

        [].slice.call(searchStringLabels, excessLength);
    }

    var searchStrings = splitSearchStringIntoTwoSearchStrings(wholeSearchStringText);

    for (let i = 0; i < searchStrings.length; i++)
    {
        var searchString = searchStrings[i];

        if (searchString === null || searchString === undefined
            || searchStrings === "") break;

        var substrings = searchString.trim().split(' ');

        for (let j = 0; j < searchStringLabels.length; j++)
        {
            var originatesFromThisSearchString = true;

            var searchStringLabel = searchStringLabels[j];

            var text = searchStringLabel.textContent;

            var localText = searchStringLabel.textContent;

            for (let k1 = 0; k1 < substrings.length; k1++)
            {
                var substring = substrings[k1];

                var textToUpperCase = text.toUpperCase();

                var substringIndex = textToUpperCase.indexOf(substring.toUpperCase());

                if (substringIndex === -1)
                {
                    originatesFromThisSearchString = false;

                    break;
                }
            }

            if (!originatesFromThisSearchString) continue;

            for (let k = 0; k < substrings.length; k++)
            {
                var substring = substrings[k];

                var localTextWithNewSpans = getAllTextWithSpansWithData(localText, substring);

                localText = localTextWithNewSpans;
            }

            searchStringLabel.innerHTML = localText.replaceAll(spanStartPlaceholder, spanStart)
                .replaceAll(spanEndPlaceholder, spanEnd);

            searchStringLabels.splice(j, 1);

            j--;
        }
    }
}

function getAllTextWithSpansWithData(textWithSpans, substring)
{
    var indexesOfSubstringInTextWithSpansWithSkips = indexOfThatSkipsCharacters(textWithSpans, substring.toUpperCase(), [spanStartPlaceholder, spanEndPlaceholder]);

    var startRealIndex = indexesOfSubstringInTextWithSpansWithSkips[0];
    var endRealIndex = indexesOfSubstringInTextWithSpansWithSkips[1];

    var indexOfEndOfPreviousSpan = textWithSpans.lastIndexOf(spanEndPlaceholder, startRealIndex);
    var indexOfStartOfNextSpan = textWithSpans.indexOf(spanStartPlaceholder, endRealIndex);

    if (indexOfEndOfPreviousSpan === -1)
    {
        indexOfEndOfPreviousSpan = 0;
    }
    else
    {
        indexOfEndOfPreviousSpan = indexOfEndOfPreviousSpan + 2
    }

    if (indexOfStartOfNextSpan === -1)
    {
        indexOfStartOfNextSpan = textWithSpans.length;
    }

    var substringBetweenOutsideSpans = textWithSpans.substring(indexOfEndOfPreviousSpan, indexOfStartOfNextSpan);

    var indexOfStartOfThisSpan = substringBetweenOutsideSpans.lastIndexOf(spanStartPlaceholder, startRealIndex - indexOfEndOfPreviousSpan);
    var indexOfEndOfThisSpan = substringBetweenOutsideSpans.indexOf(spanEndPlaceholder, endRealIndex - indexOfEndOfPreviousSpan);

    if (indexOfStartOfThisSpan === -1)
    {
        if (indexOfEndOfThisSpan === -1)
        {
            textWithSpans = textWithSpans.substring(0, startRealIndex) + spanStartPlaceholder + substring + spanEndPlaceholder + textWithSpans.substring(endRealIndex);
        }
        else
        {
            textWithSpans = textWithSpans.substring(0, startRealIndex) + spanStartPlaceholder + substring + textWithSpans.substring(endRealIndex);
        }
    }
    else if (indexOfEndOfThisSpan === -1)
    {
        textWithSpans = textWithSpans.substring(0, startRealIndex) + substring + spanEndPlaceholder + textWithSpans.substring(endRealIndex);
    }

    return textWithSpans;
}

function indexOfThatSkipsCharacters(string, substring, stringsToSkip, fromIndex = 0)
{
    if (typeof string !== "string" || typeof substring !== "string")
    {
        throw new TypeError("The first and second arguments must be strings");
    }

    if (Object.prototype.toString.call(stringsToSkip) !== '[object Array]')
    {
        throw new TypeError("The third argument must be string array");
    }

    if (typeof fromIndex !== "number" || isNaN(fromIndex))
    {
        throw new TypeError("The fourth argument must be a number");
    }

    if (substring === "")
    {
        return [fromIndex, fromIndex];
    }

    let i = fromIndex;

    var endIndex = fromIndex;

    while (i < string.length)
    {
        if (string[i] === substring[0])
        {
            let found = true;

            for (let j = 1; j < substring.length; j++)
            {
                if (string[i + j] !== substring[j])
                {
                    let skipped = false;

                    for (let k = 0; k < stringsToSkip.length; k++)
                    {
                        var stringToSkip = stringsToSkip[k];

                        var subStringForStrToSkip = string.substring(i + j, i + j + stringToSkip.length);

                        if (subStringForStrToSkip === stringToSkip)
                        {
                            skipped = true;

                            j += stringToSkip.length;

                            endIndex += stringToSkip.length;

                            break;
                        }
                    }

                    if (!skipped)
                    {
                        found = false;

                        endIndex = i;

                        break;
                    }
                }
            }
            if (found)
            {
                return [i, endIndex + substring.length];
            }
        }

        i++;
        endIndex++;
    }

    return [-1, -1];
}

function splitSearchStringIntoTwoSearchStrings(searchStringData)
{
    var indexOfParenthesesInSearchStringSubstr = searchStringData.indexOf('[');

    if (indexOfParenthesesInSearchStringSubstr == -1) return [searchStringData, null];

    var endIndexOfParenthesesInSearchStringSubstr = searchStringData.indexOf(']');

    if (endIndexOfParenthesesInSearchStringSubstr == -1) return [searchStringData, null];

    var firstSearchData = searchStringData.substring(0, indexOfParenthesesInSearchStringSubstr - 1)
        .trim();

    var secondSearchStringDataLength = endIndexOfParenthesesInSearchStringSubstr - indexOfParenthesesInSearchStringSubstr - 1;

    var secondSearchData = searchStringData.substring(indexOfParenthesesInSearchStringSubstr + 1, indexOfParenthesesInSearchStringSubstr + 1 + secondSearchStringDataLength)
        .trim();

    var endString = searchStringData.substring(endIndexOfParenthesesInSearchStringSubstr + 1)
        .trim();

    var firstSearchString = firstSearchData + " " + endString;
    var secondSearchString = secondSearchData + " " + endString;

    return [firstSearchString, secondSearchString];
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
                DisplayMultipleInputColoringAndClearExtraOutputAfterTextSearch(substring, "SearchString_label", result.length, "customSpanForSearchString");
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
                    DisplayMultipleInputColoringAndClearExtraOutputAfterTextSearch(searchStringSubstring, "SearchString_label", result.length, "customSpanForSearchString");
                }
            });
        })
        .fail(function (jqXHR, textStatus)
        {
        });
}