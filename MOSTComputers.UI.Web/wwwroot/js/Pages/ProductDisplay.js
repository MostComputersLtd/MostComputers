function showXmlPopupData(productId)
{
    $("#ProductXml_popup_modal_content").load("/ProductDisplay?handler=PartialViewXmlForProduct&productId=" + productId);

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
    $("#ProductImages_popup_modal_content").load("/ProductDisplay?handler=PartialViewImagesForProduct&productId=" + productId);

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

        if (searchString === null
            || searchString === undefined
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

            searchStringLabel.innerHTML = localText
                .replaceAll(spanStartPlaceholder, spanStart)
                .replaceAll(spanEndPlaceholder, spanEnd);

            searchStringLabels.splice(j, 1);

            j--;
        }
    }
}

function DisplayMultipleInputColoringAndClearExtraOutputAfterTextSearchInParts(wholeSearchStringText, partElementsName, nameOfProductRows, length, customSpanName)
{
    var spanStart = "<span name='" + customSpanName + "' style='background-color:#FAF2B1'>";
    const spanEnd = "</span>";

    var productRows = Array.from(document.getElementsByName(nameOfProductRows));

    if (productRows.length > length)
    {
        var excessLength = productRows.length - length;

        $("#tableDataHolder").children().slice(0, excessLength).remove();

        [].slice.call(productRows, excessLength);
    }

    var searchStrings = splitSearchStringIntoTwoSearchStrings(wholeSearchStringText);

    for (let i = 0; i < searchStrings.length; i++)
    {
        var searchString = searchStrings[i];

        if (searchString === null
            || searchString === undefined
            || searchStrings === "") break;

        var substrings = searchString.trim().split(' ');

        for (let j = 0; j < productRows.length; j++)
        {
            var originatesFromThisSearchString = true;

            var searchStringPartElementsNameIndex = getIndexFromElementId(productRows[j].id);

            var currentSearchStringPartElementsName = partElementsName + "-" + searchStringPartElementsNameIndex;

            var searchStringPartElements = Array.from(document.getElementsByName(currentSearchStringPartElementsName));

            var searchStringPartElementsTexts = [];

            for (var z = 0; z < searchStringPartElements.length; z++)
            {
                searchStringPartElementsTexts.push(searchStringPartElements[z].innerText);
            }

            var searchStringText = searchStringPartElementsTexts.join(" ");

            var text = searchStringText;

            var localText = searchStringText;

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

            splitAllTextWithSpansIntoParts(localText, currentSearchStringPartElementsName, spanStart, spanEnd);

            productRows.splice(j, 1);

            j--;
        }
    }
}

function splitAllTextWithSpansIntoParts(textWithSpans, partElementsName, spanStart, spanEnd)
{
    var partElements = Array.from(document.getElementsByName(partElementsName));

    partElements.sort((a, b) => indexOfThatSkipsCharacters(textWithSpans.toUpperCase(), a.innerText.toUpperCase(), [spanStartPlaceholder, spanEndPlaceholder]) >
        indexOfThatSkipsCharacters(textWithSpans.toUpperCase(), b.innerText.toUpperCase(), [spanStartPlaceholder, spanEndPlaceholder]));

    for (var i = 0; i < partElements.length; i++)
    {
        var partElement = partElements[i];

        var partElementIndexesInText = indexOfThatSkipsCharacters(textWithSpans.toUpperCase(), partElement.innerText.toUpperCase(), [spanStartPlaceholder, spanEndPlaceholder])

        var partElementStartIndexInText = partElementIndexesInText[0];
        var partElementEndIndexInText = partElementIndexesInText[1];

        var partElementInText = textWithSpans.substring(partElementStartIndexInText, partElementEndIndexInText);

        var substringFromDirectlyAroundItem = textWithSpans.substring(
            partElementStartIndexInText - spanStartPlaceholder.length,
            partElementEndIndexInText + spanEndPlaceholder.length);

        if (substringFromDirectlyAroundItem.startsWith(spanStartPlaceholder))
        {
            partElementInText = spanStartPlaceholder + partElementInText;
        }

        if (substringFromDirectlyAroundItem.endsWith(spanEndPlaceholder))
        {
            partElementInText += spanEndPlaceholder;
        }

        if (partElementInText.startsWith(spanEndPlaceholder))
        {
            partElementInText = partElementInText
                .replace(spanEndPlaceholder, "");
        }

        if (partElementInText.endsWith(spanStartPlaceholder))
        {
            partElementInText = partElementInText
                .slice(0, -spanStartPlaceholder.length);
        }
        
        var lastSpanStartIndex = partElementInText.lastIndexOf(spanStartPlaceholder);

        if (lastSpanStartIndex != -1)
        {
            var nextSpanEndIndex = partElementInText.indexOf(spanEndPlaceholder, lastSpanStartIndex + 1);

            if (nextSpanEndIndex == -1)
            {
                partElementInText += spanEndPlaceholder;
            }
        }

        var firstSpanEndIndex = partElementInText.indexOf(spanEndPlaceholder);

        if (firstSpanEndIndex != -1)
        {
            var substringBeforeFirstSpanEnd = partElementInText.substring(0, firstSpanEndIndex);

            var spanStartBeforeFirstEndIndex = substringBeforeFirstSpanEnd.indexOf(spanStartPlaceholder);

            if (spanStartBeforeFirstEndIndex == -1)
            {
                partElementInText = spanStartPlaceholder + partElementInText
            }
        }

        partElement.innerHTML = partElementInText
            .replaceAll(spanStartPlaceholder, spanStart)
            .replaceAll(spanEndPlaceholder, spanEnd);
    }
}

function getAllTextWithSpansWithData(textWithSpans, substring)
{
    var indexesOfSubstringInTextWithSpansWithSkips = indexOfThatSkipsCharacters(textWithSpans.toUpperCase(), substring.toUpperCase(), [spanStartPlaceholder, spanEndPlaceholder]);

    var startRealIndex = indexesOfSubstringInTextWithSpansWithSkips[0];
    var endRealIndex = indexesOfSubstringInTextWithSpansWithSkips[1];

    var originalString = textWithSpans.substring(startRealIndex, endRealIndex);

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
        originalString = originalString.replace(spanStartPlaceholder, "");

        if (indexOfEndOfThisSpan === -1)
        {
            originalString = originalString.replace(spanEndPlaceholder, "");

            textWithSpans = textWithSpans.substring(0, startRealIndex) + spanStartPlaceholder + originalString + spanEndPlaceholder + textWithSpans.substring(endRealIndex);
        }
        else
        {
            textWithSpans = textWithSpans.substring(0, startRealIndex) + spanStartPlaceholder + originalString + textWithSpans.substring(endRealIndex);
        }
    }
    else if (indexOfEndOfThisSpan === -1)
    {
        originalString = originalString.replace(spanEndPlaceholder, "");

        textWithSpans = textWithSpans.substring(0, startRealIndex) + originalString + spanEndPlaceholder + textWithSpans.substring(endRealIndex);
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

            var substringLengthExtension = 0;

            for (let j = 1; j < substring.length + substringLengthExtension; j++)
            {
                if (string[i + j] !== substring[j - substringLengthExtension])
                {
                    let skipped = false;

                    for (let k = 0; k < stringsToSkip.length; k++)
                    {
                        var stringToSkip = stringsToSkip[k];

                        var subStringForStrToSkip = string.substring(i + j, i + j + stringToSkip.length);

                        if (subStringForStrToSkip === stringToSkip)
                        {
                            skipped = true;

                            j += stringToSkip.length - 1;

                            substringLengthExtension += stringToSkip.length;

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

function getIndexFromElementId(elementId)
{
    if (elementId === null
        || elementId === undefined
        || elementId === "") return null;

    var indexOfTag = elementId.lastIndexOf("-");

    if (indexOfTag == -1) return null;

    return elementId.substring(indexOfTag + 1);
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
                    DisplayMultipleInputColoringAndClearExtraOutputAfterTextSearchInParts(searchStringSubstring,
                        "searchStringPartDisplayText", "productDisplayRow", result.length, "customSpanForSearchString");
                }
            });
        })
        .fail(function (jqXHR, textStatus)
        {
        });
}

function showSearchStringOriginDataSmallPopup(productIndex, index, searchStringPart)
{
    if (productIndex === null
        || productIndex === undefined
        || (isNaN(productIndex) && isNaN(parseInt(productIndex)))
        || index === null
        || index === undefined
        || (isNaN(index) && isNaN(parseInt(index)))) return;

    showSearchStringOriginDataSmallPopupCommon(
        "searchStringPartOrigin_li-" + productIndex + "-" + index,
        "searchStringPartOrigin_multipleOriginsDisplayList-" + productIndex + "-" + index,
        searchStringPart,
        "searchStringPartOrigin_multipleOriginsDisplayList_nameLabel-" + productIndex + "-" + index,
        "searchStringPartOrigin_multipleOriginsDisplayList_meaningLabel-" + productIndex + "-" + index);
}

function removeSearchStringOriginDataSmallPopup(productIndex, index, searchStringPart)
{
    if (productIndex === null
        || productIndex === undefined
        || (isNaN(productIndex) && isNaN(parseInt(productIndex)))
        || index === null
        || index === undefined
        || (isNaN(index) && isNaN(parseInt(index)))) return;

    searchStringPart = decodeHtmlString(searchStringPart);

    removeSearchStringOriginDataSmallPopupCommon(
        "searchStringPartOrigin_li#" + productIndex + "-" + index,
        "searchStringPartOrigin_multipleOriginsDisplayList-" + productIndex + "-" + index,
        searchStringPart,
        "searchStringPartOrigin_multipleOriginsDisplayList_nameLabel-" + productIndex + "-" + index,
        "searchStringPartOrigin_multipleOriginsDisplayList_meaningLabel-" + productIndex + "-" + index);
}

function displayPopupAndTransportToSearchStringOriginDisplay(productId, index)
{
    if (index === null
        || index === undefined
        || (isNaN(index) && isNaN(parseInt(index)))) return;

    $("#ProductSearchString_modal").one("shown.bs.modal", function()
    {
        transportToSearchStringOriginDisplay(index);
    });

    showSearchStringPopupData(productId);
}

function showSearchStringPopupData(productId)
{
    $("#ProductSearchString_popup_modal_content").load("/ProductPropertiesEditor/" + productId + "?handler=GetSearchStringPartialView", function()
    {
        open_ProductSearchString_modal();
    });
}

function copySearchStringDataToClipboard()
{
    let searchStringData = getSearchStringData();

    if (searchStringData === null
    || searchStringData === undefined) return;

    navigator.clipboard.writeText(searchStringData);

    showNotificationWithText("copiedXmlNotificationBox", "Copied!", "copy-to-xml-success-message");
}