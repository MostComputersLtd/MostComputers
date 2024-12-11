const tableSortedColumnSessionStorageKey = "table_sorted_column";

const tableSortableSingleItemAttributeName = "data-sortable-single-item";
const tableSortableMultipleItemPartAttributeName = "data-sortable-multiple-item-part";

const selectableItemRelationshipIndexAttributeName = 'data-selectable-item-in-relationship-with-index';
const selectableItemIndexAttributeName = "data-selectable-item-in-relationship-index";
const selectableItemTypeAttributeName = 'data-selectable-item-in-relationship-type';

const selectedItemAttributeName = 'data-product-characteristics-compare-selected-item';
const selectedItemClassname = "product-characteristics-compare-selected-item";

const relationshipAllowsDropAttributeName = "data-relationship-allows-drop-with-index";

const relationshipDragEnterClassname = "product-characteristics-compare-relationship-allowing-drop-drag-over";

const selectedItemForMatchAttributeName = 'data-product-characteristics-compare-selected-for-match-item';
const matchedItemAttributeName = 'data-product-characteristics-compare-matched-item';

const matchedItemRandomColorGenerationSeed = 36;

function sortRelationshipTable(
    relationshipTableId,
    sortByCharacteristicsOrProperties,
    isDescending,
    innerColumnIndex)
{
    extraSortParams = [innerColumnIndex];

    sortTable(relationshipTableId, sortByCharacteristicsOrProperties, isDescending,
        (cell) => sortRelationshipsByCharacteristicColumn(cell, extraSortParams), 2);

    sessionStorage.setItem(tableSortedColumnSessionStorageKey, sortByCharacteristicsOrProperties + "," + innerColumnIndex);
}

function sortRelationshipTableWithSessionStorageData(relationshipTableId)
{
    var storedSortingData = sessionStorage.getItem(tableSortedColumnSessionStorageKey);

    if (storedSortingData == null) return;

    var indexOfCommaSeparator = storedSortingData.indexOf(',');

    var storedCharacteristicOrPropertySortIndexAsString = storedSortingData.substring(0, indexOfCommaSeparator);
    var columnToSortIndexAsString = storedSortingData.substring(indexOfCommaSeparator + 1);

    var storedCharacteristicOrPropertySortIndex = parseInt(storedCharacteristicOrPropertySortIndexAsString);
    var columnToSortIndex = parseInt(columnToSortIndexAsString);

    sortRelationshipTable(
        relationshipTableId,
        storedCharacteristicOrPropertySortIndex,
        false,
        columnToSortIndex)
}

function sortRelationshipsByCharacteristicColumn(cell, extraSortParams)
{
    var innerColumnIndex = extraSortParams[0];

    var singleCharacteristicItem = cell.querySelector(`[${tableSortableSingleItemAttributeName}]`);
    var multipleCharacteristicItem = cell.querySelector(`[${tableSortableMultipleItemPartAttributeName}]`);

    var cellValue = null;

    if (singleCharacteristicItem != null)
    {
        var firstRow = Array.from(singleCharacteristicItem.rows)[0];

        cellValue = firstRow.cells[innerColumnIndex].innerText;
    }
    else if (multipleCharacteristicItem != null)
    {
        cellValue = multipleCharacteristicItem.cells[innerColumnIndex].innerText;
    }

    const valueAsInt = parseFloat(cellValue);

    if (!isNaN(valueAsInt))
    {
        return valueAsInt
    }

    return (cellValue != null) ? cellValue.trim() : null;
}

async function compareExternalAndLocalData(
    externalXmlTextBoxId,
    localCharacteristicDataListContainerId,
    externalPropertyDataListContainerId,
    characteristicsRelationshipTableContainerId,
    characteristicsRelationshipTableId,
    compareDataInCategoryButtonId,
    loaderElementId = null)
{
    var externalXmlTextBox = document.getElementById(externalXmlTextBoxId);

    if (externalXmlTextBox == null
        || externalXmlTextBox.value == null)
    {
        return;
    }

    const url = "/ProductCharacteristicsComparer" + "?handler=CompareExternalAndLocalData";

    var promise = fetch(url, {
        method: 'POST',
        body: JSON.stringify(externalXmlTextBox.value),
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken':
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    });

    var response = await awaitWithCallbacks(promise,
        function () { toggleViews(compareDataInCategoryButtonId, loaderElementId); },
        function () { toggleViews(compareDataInCategoryButtonId, loaderElementId); });

    if (response.status !== 200) return;

    var responseData = await response.json();

    var localCharacteristicDataListContainer = document.getElementById(localCharacteristicDataListContainerId);
    var externalPropertyDataListContainer = document.getElementById(externalPropertyDataListContainerId);

    var characteristicsRelationshipTableContainer = document.getElementById(characteristicsRelationshipTableContainerId);

    localCharacteristicDataListContainer.innerHTML = responseData.localCharacteristicsPartialViewText;
    externalPropertyDataListContainer.innerHTML = responseData.externalPropertiesPartialViewText;

    characteristicsRelationshipTableContainer.innerHTML = responseData.characteristicsRelationshipTablePartialViewText;

    sortRelationshipTableWithSessionStorageData(characteristicsRelationshipTableId);
}

async function compareExternalAndLocalDataFromFile(
    localCharacteristicDataListContainerId,
    externalPropertyDataListContainerId,
    characteristicsRelationshipTableContainerId,
    characteristicsRelationshipTableId,
    compareDataInCategoryButtonId,
    loaderElementId)
{
    const url = "/ProductCharacteristicsComparer" + "?handler=CompareExternalAndLocalDataFromFile";

    var promise = fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken':
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    });

    var response = await awaitWithCallbacks(promise,
        function () { toggleViews(compareDataInCategoryButtonId, loaderElementId); },
        function () { toggleViews(compareDataInCategoryButtonId, loaderElementId); });

    if (response.status !== 200) return;

    var responseData = await response.json();

    var localCharacteristicDataListContainer = document.getElementById(localCharacteristicDataListContainerId);
    var externalPropertyDataListContainer = document.getElementById(externalPropertyDataListContainerId);

    var characteristicsRelationshipTableContainer = document.getElementById(characteristicsRelationshipTableContainerId);

    localCharacteristicDataListContainer.innerHTML = responseData.localCharacteristicsPartialViewText;
    externalPropertyDataListContainer.innerHTML = responseData.externalPropertiesPartialViewText;

    characteristicsRelationshipTableContainer.innerHTML = responseData.characteristicsRelationshipTablePartialViewText;

    sortRelationshipTableWithSessionStorageData(characteristicsRelationshipTableId);
}

async function compareExternalAndLocalDataForCategoryFromFile(
    categoryIdSelectId,
    localCharacteristicDataListContainerId,
    externalPropertyDataListContainerId,
    characteristicsRelationshipTablePartialViewContainerId,
    characteristicsRelationshipTableId,
    compareDataInCategoryButtonId,
    loaderElementId = null)
{
    var categoryIdSelect = document.getElementById(categoryIdSelectId);

    var categoryId = categoryIdSelect.value;

    if (categoryId == null || isNaN(parseInt(categoryId))) return;

    const url = "/ProductCharacteristicsComparer" + "?handler=CompareExternalAndLocalDataForCategoryFromFile"
        + "&categoryId=" + categoryId;

    var promise = fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken':
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    });

    var response = await awaitWithCallbacks(promise,
        function () { toggleViews(compareDataInCategoryButtonId, loaderElementId); },
        function () { toggleViews(compareDataInCategoryButtonId, loaderElementId); });

    if (response.status !== 200) return;

    var responseData = await response.json();

    var localCharacteristicDataListContainer = document.getElementById(localCharacteristicDataListContainerId);
    var externalPropertyDataListContainer = document.getElementById(externalPropertyDataListContainerId);

    var characteristicsRelationshipTablePartialViewContainer = document.getElementById(characteristicsRelationshipTablePartialViewContainerId);

    localCharacteristicDataListContainer.innerHTML = responseData.localCharacteristicsPartialViewText;
    externalPropertyDataListContainer.innerHTML = responseData.externalPropertiesPartialViewText;

    characteristicsRelationshipTablePartialViewContainer.innerHTML = responseData.characteristicsRelationshipTablePartialViewText;

    sortRelationshipTableWithSessionStorageData(characteristicsRelationshipTableId);
}

function addNewEmptyRelationship(
    characteristicsRelationshipTableContainerId,
    characteristicsRelationshipTableId)
{
    const url = "/ProductCharacteristicsComparer" + "?handler=AddNewEmptyRelationship";

    fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken':
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    })
        .then(async function (response)
        {
            if (response.status !== 200) return;

            var responseText = await response.text();

            var characteristicsRelationshipTableContainer = document.getElementById(characteristicsRelationshipTableContainerId);

            characteristicsRelationshipTableContainer.outerHTML = responseText;

            sortRelationshipTableWithSessionStorageData(characteristicsRelationshipTableId);
        });
}

function isElementSelectable(element)
{
    return element.hasAttribute(selectableItemRelationshipIndexAttributeName);
}

function isElementSelected(element)
{
    var selectedAttribute = element.getAttribute(selectedItemAttributeName);

    if (selectedAttribute == null) return false;

    return selectedAttribute;
}

function doesRelationshipAllowDrop(element)
{
    return element.hasAttribute(relationshipAllowsDropAttributeName);
}

function getRelationshipToDropInIndex(element)
{
    return element.getAttribute(relationshipAllowsDropAttributeName);
}

function getRelationshipIndex(element)
{
    if (!isElementSelectable(element)) return null;

    return element.getAttribute(selectableItemRelationshipIndexAttributeName);
}

function getSelectableElementIndex(element)
{
    if (!isElementSelectable(element)) return null;

    return element.getAttribute(selectableItemIndexAttributeName);
}

function getSelectableElementType(element)
{
    return element.getAttribute(selectableItemTypeAttributeName);
}

function unselectElementsFromOtherRelationships(currentElement)
{
    const currentIndex = getRelationshipIndex(currentElement);
    const currentElementType = getSelectableElementType(currentElement);

    document.querySelectorAll('[' + selectedItemAttributeName + '="true"]')
        .forEach(element =>
        {
            if (getRelationshipIndex(element) !== currentIndex
                || getSelectableElementType(element) !== currentElementType)
            {
                element.removeAttribute(selectedItemAttributeName);

                element.classList.remove(selectedItemClassname);
            }
        });
}

function toggleElementSelectionState(element)
{
    if (element.getAttribute(selectedItemAttributeName) === 'true')
    {
        element.removeAttribute(selectedItemAttributeName);

        element.classList.remove(selectedItemClassname);
    }
    else
    {
        element.setAttribute(selectedItemAttributeName, 'true');

        element.classList.add(selectedItemClassname);
    }
}

function toggleElementSelect(elementId)
{
    var element = document.getElementById(elementId);

    if (element == null
        || !isElementSelectable(element)) return;

    if (!isElementSelected(element))
    {
        unselectElementsFromOtherRelationships(element);
    }

    toggleElementSelectionState(element);
}

async function onClickToggleElementSelectOrRelationshipMatch(event, elementId)
{
    if (event.ctrlKey)
    {
        clearSelectedElements();

        await matchSelectedPropertyAndCharacteristic(elementId);

        return;
    }

    clearSelectedElementsForMatching();

    toggleElementSelect(elementId);
}


function clearSelectedElements()
{
    var elementsSelectedForMoving = Array.from(document.querySelectorAll('[' + selectedItemAttributeName + '="true"]'));

    elementsSelectedForMoving.forEach(item =>
    {
        item.removeAttribute(selectedItemAttributeName);

        item.classList.remove(selectedItemClassname);
    });
}

function clearSelectedElementsForMatching()
{
    var elementsSelectedForMatching = Array.from(
        document.querySelectorAll('[' + selectedItemForMatchAttributeName + '="true"]:not([' + matchedItemAttributeName + '])'));

    clearElementsForMatching(elementsSelectedForMatching);
}

function clearElementsForMatching(elementsSelectedForMatching)
{
    if (elementsSelectedForMatching == null) return;

    elementsSelectedForMatching.forEach(item =>
    {
        clearElementForMatching(item);
    });
}

function clearElementForMatching(elementSelectedForMatching)
{
    if (elementSelectedForMatching == null) return;

    elementSelectedForMatching.removeAttribute(selectedItemForMatchAttributeName);

    if (!elementSelectedForMatching.hasAttribute(matchedItemAttributeName))
    {
        elementSelectedForMatching.style.backgroundColor = "";
    }
}

async function matchSelectedPropertyAndCharacteristic(elementId)
{
    var element = document.getElementById(elementId);

    var elementRelationshipIndex = getRelationshipIndex(element);
    var elementType = getSelectableElementType(element);
    var elementIndex = getSelectableElementIndex(element);

    var characteristicSelectedForMatching = document.querySelector('[' + selectedItemForMatchAttributeName + '="true"][' + selectableItemTypeAttributeName + '="0"]');
    var propertiesSelectedForMatching = Array.from(document.querySelectorAll('[' + selectedItemForMatchAttributeName + '="true"][' + selectableItemTypeAttributeName + '="1"]'));

    var currentMatchRelationshipIndex = getCharacteristicOrPropertyRelationshipIndex(characteristicSelectedForMatching, propertiesSelectedForMatching)

    var backgroundColorToUse = getOrCreateBackgroundColor(
        characteristicSelectedForMatching, propertiesSelectedForMatching);

    if (elementRelationshipIndex != currentMatchRelationshipIndex)
    {
        clearElementForMatching(characteristicSelectedForMatching);
        clearElementsForMatching(propertiesSelectedForMatching);

        element.setAttribute(selectedItemForMatchAttributeName, 'true');

        var numberToIncreaseColorSeedBy = getNumberToIncreaseColorSeedBy(elementRelationshipIndex);

        element.style.backgroundColor = generateColorDeterministically(matchedItemRandomColorGenerationSeed + numberToIncreaseColorSeedBy);

        return;
    }

    if (elementType === '0')
    {
        if (characteristicSelectedForMatching != null
            && characteristicSelectedForMatching.hasAttribute(matchedItemAttributeName))
        {
            characteristicSelectedForMatching.removeAttribute(selectedItemForMatchAttributeName);

            propertiesSelectedForMatching.forEach(x => 
            {
                if (x.hasAttribute(selectedItemForMatchAttributeName)
                    && x.hasAttribute(matchedItemAttributeName)
                    && x.getAttribute(matchedItemAttributeName) != elementIndex)
                {
                    x.removeAttribute(selectedItemForMatchAttributeName);
                }
            });

            var numberToIncreaseColorSeedBy = getNumberToIncreaseColorSeedBy(elementRelationshipIndex);

            propertiesSelectedForMatching = propertiesSelectedForMatching.filter(x => !x.hasAttribute(matchedItemAttributeName));

            backgroundColorToUse = generateColorDeterministically(matchedItemRandomColorGenerationSeed + numberToIncreaseColorSeedBy);
        }
        else
        {
            clearElementForMatching(characteristicSelectedForMatching);

            element.setAttribute(selectedItemForMatchAttributeName, 'true');

            element.style.backgroundColor = backgroundColorToUse;

            characteristicSelectedForMatching = element;
        }
    }
    else
    {
        element.setAttribute(selectedItemForMatchAttributeName, 'true');

        element.style.backgroundColor = backgroundColorToUse;

        propertiesSelectedForMatching.push(element);
    }

    if (characteristicSelectedForMatching == null
        || propertiesSelectedForMatching == null
        || propertiesSelectedForMatching.length <= 0)
    {
        element.setAttribute(selectedItemForMatchAttributeName, 'true');

        element.style.backgroundColor = backgroundColorToUse;

        return;
    }

    var selectedCharacteristicIndex = getSelectableElementIndex(characteristicSelectedForMatching);

    var matchedExternalPropertyIndexes = "";

    for (var i = 0; i < propertiesSelectedForMatching.length; i++)
    {
        const externalPropertyToMatch = propertiesSelectedForMatching[i];

        var otherItemToMatchIndex = getSelectableElementIndex(externalPropertyToMatch);

        var response = await matchCorrespondingPropertyAndCharacteristic(elementRelationshipIndex, selectedCharacteristicIndex, otherItemToMatchIndex, backgroundColorToUse);

        if (response.status !== 200)
        {
            clearSelectedElementsForMatching();

            return;
        }

        externalPropertyToMatch.setAttribute(matchedItemAttributeName, elementIndex);

        var externalPropertyIndexData = (i == 0) ? otherItemToMatchIndex.toString() : "," + otherItemToMatchIndex;

        matchedExternalPropertyIndexes += externalPropertyIndexData;
    }

    characteristicSelectedForMatching.setAttribute(matchedItemAttributeName, matchedExternalPropertyIndexes);
}

function getNumberToIncreaseColorSeedBy(elementRelationshipIndex)
{
    var queryStringForAllOtherSelectedCharacteristics = '[' + matchedItemAttributeName + '][' + selectableItemRelationshipIndexAttributeName + '="' + elementRelationshipIndex + '"][' + selectableItemTypeAttributeName + '="0"]';

    var characteristicsThatAreAlreadyMatchedInSameRelation = document.querySelectorAll(queryStringForAllOtherSelectedCharacteristics);

    return characteristicsThatAreAlreadyMatchedInSameRelation.length;
}

function getCharacteristicOrPropertyRelationshipIndex(characteristicSelectedForMatching, propertiesSelectedForMatching)
{
    if (characteristicSelectedForMatching != null)
    {
        return getRelationshipIndex(characteristicSelectedForMatching);
    }

    if (propertiesSelectedForMatching == null
        || propertiesSelectedForMatching.length <= 0)
    {
        return null;
    }

    return getRelationshipIndex(propertiesSelectedForMatching[0]);
}

function changeBackgroundColorOfAllElementsToUsedOrNewOne(elements)
{
    if (elements == null
        || !Array.isArray(elements)) return;

    var backgroundColor = getOrCreateBackgroundColor(elements);

    changeBackgroundColorOfAllElements(elements, backgroundColor)
}

function getOrCreateBackgroundColor(characteristicSelectedForMatching, propertiesSelectedForMatching, numberToIncrementColorBy = 0)
{
    var backgroundColorToUse = null;

    if (characteristicSelectedForMatching != null
        && characteristicSelectedForMatching.style.backgroundColor !== "")
    {
        backgroundColorToUse = characteristicSelectedForMatching.style.backgroundColor;
    }
    
    if (backgroundColorToUse == null)
    {
        backgroundColorToUse = getUsedBackgroundColor(propertiesSelectedForMatching);
    }

    if (backgroundColorToUse == null)
    {
        backgroundColorToUse = generateColorDeterministically(matchedItemRandomColorGenerationSeed + numberToIncrementColorBy);
    }

    return backgroundColorToUse;
}

function getUsedBackgroundColor(elements)
{
    var backgroundColor = null;

    for (var i = 0; i < elements.length; i++)
    {
        var elementBackgroundColor = elements[i].style.backgroundColor;

        if (elementBackgroundColor != null
            && elementBackgroundColor !== "")
        {
            backgroundColor = elementBackgroundColor;

            break;
        }
    }

    return backgroundColor;
}

function changeBackgroundColorOfAllElements(elements, backgroundColor)
{
    if (elements == null
        || !Array.isArray(elements)) return;

    for (var i = 0; i < elements.length; i++)
    {
        elements[i].style.backgroundColor = backgroundColor;
    }
}

function onStartDraggingRelationshipElements(event, elementId)
{
    var element = document.getElementById(elementId);

    if (!isElementSelected(element))
    {
        clearSelectedElementsForMatching();

        toggleElementSelect(elementId);
    }

    const selectedItems = document.querySelectorAll('[' + selectedItemAttributeName + '="true"]');

    var data =
    {
        relationshipIndex: getRelationshipIndex(element),
        elementsType: getSelectableElementType(element),
        elementData: []
    };

    selectedItems.forEach(item =>
    {
        const elementIndex = getSelectableElementIndex(item);

        data.elementData.push(
        {
            elementId: item.id,
            elementIndex: elementIndex
        });
    });

    event.dataTransfer.setData("application/json", JSON.stringify(data));
}

function onDragEnterRelationshipElements(event, relationshipRowElementId)
{
    event.preventDefault();

    var relationshipRowElement = document.getElementById(relationshipRowElementId);

    relationshipRowElement.classList.add(relationshipDragEnterClassname);
}

function onDragOverRelationshipElements(event)
{
    event.preventDefault();
}

function onDragLeaveRelationshipElements(event, relationshipRowElementId)
{
    event.preventDefault();

    var relationshipRowElement = document.getElementById(relationshipRowElementId);

    if (!relationshipRowElement.contains(event.relatedTarget))
    {
        relationshipRowElement.classList.remove(relationshipDragEnterClassname);
    }
}

async function onDropRelationshipElements(
    event,
    relationshipToDropInId,
    productCharacteristicsCompareTableContainerId,
    characteristicsRelationshipTableId)
{
   event.preventDefault();

    var relationshipToDropIn = document.getElementById(relationshipToDropInId);

    if (!doesRelationshipAllowDrop(relationshipToDropIn)) return;

    relationshipToDropIn.classList.remove(relationshipDragEnterClassname);

    const relationshipToMoveToIndex = getRelationshipToDropInIndex(relationshipToDropIn);

    const dataAsString = event.dataTransfer.getData("application/json");

    if (dataAsString == null || dataAsString === "") return;

    const data = JSON.parse(dataAsString);

    if (data.relationshipIndex === relationshipToMoveToIndex) return;

    const indexesOfElements = data.elementData.map(item => item.elementIndex);

    var response = await moveItemsFromOneRelationshipToAnother(data.relationshipIndex, relationshipToMoveToIndex, data.elementsType, indexesOfElements);

    if (response.status != 200) return;

    var responseText = await response.text();

    if (responseText == null
        || responseText.length <= 0) return;

    var productCharacteristicsCompareTableContainer = document.getElementById(productCharacteristicsCompareTableContainerId);

    productCharacteristicsCompareTableContainer.outerHTML = responseText;

    sortRelationshipTableWithSessionStorageData(characteristicsRelationshipTableId);
}

async function showXmlViewForCategory(categoryId, relationshipTableViewContainerId, xmlViewContainerId)
{
    const url = "/ProductCharacteristicsComparer" + "?handler=GetLocalAndExternalXmlDataForCategory" + "&categoryId=" + categoryId;

    var response = await fetch(url,
    {
        method: 'POST',
        headers:
        {
            'Content-Type': 'application/json',
            'RequestVerificationToken':
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    });

    if (response.status !== 200) return;

    var responsePartialViewText = await response.text();

    var xmlViewContainer = document.getElementById(xmlViewContainerId);

    xmlViewContainer.innerHTML = responsePartialViewText;

    if (xmlViewContainer.style.display === "none")
    {
        var relationshipTableViewContainer = document.getElementById(relationshipTableViewContainerId);

        relationshipTableViewContainer.style.display = "none";

        xmlViewContainer.style.display = "";
    }
}

function moveItemsFromOneRelationshipToAnother(
    indexOfRelationshipToMoveFrom,
    indexOfRelationshipToMoveTo,
    moveCharacteristicsOrProperties,
    indexesOfItemsToMove)
{
    const url = "/ProductCharacteristicsComparer" + "?handler=MoveItemsFromOneRelationshipToAnother"
        + "&indexOfRelationshipToMoveFrom=" + indexOfRelationshipToMoveFrom
        + "&indexOfRelationshipToMoveTo=" + indexOfRelationshipToMoveTo
        + "&moveCharacteristicsOrProperties=" + moveCharacteristicsOrProperties;

    return fetch(url,
    {
        method: 'PUT',
        body: JSON.stringify(indexesOfItemsToMove),
        headers:
        {
            'Content-Type': 'application/json',
            'RequestVerificationToken':
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    });
}

function matchCorrespondingPropertyAndCharacteristic(relationshipIndex, characteristicIndex, propertyIndex, newBackgroundColorOfElements)
{
    const url = "/ProductCharacteristicsComparer" + "?handler=MatchCorrespondingPropertyAndCharacteristic"
        + "&relationshipIndex=" + relationshipIndex
        + "&characteristicIndex=" + characteristicIndex
        + "&externalXmlPropertyIndex=" + propertyIndex;

    var backgroundColorData = null;

    if (newBackgroundColorOfElements != null
        && newBackgroundColorOfElements != "")
    {
        var rgbValues = extractRGB(newBackgroundColorOfElements);

        backgroundColorData =
        {
            Red: rgbValues.r,
            Green: rgbValues.g,
            Blue: rgbValues.b
        };
    }

    return fetch(url,
    {
        method: 'PUT',
        headers:
        {
            'Content-Type': 'application/json',
            'RequestVerificationToken':
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
        body: (backgroundColorData != null) ? JSON.stringify(backgroundColorData) : null
    });
}

function extractRGB(rgbColorFromHtmlElement)
{
    const startOfValuesIndex = rgbColorFromHtmlElement.indexOf('(') + 1;

    const endOfValuesIndex = rgbColorFromHtmlElement.indexOf(')');

    const rgbValues = rgbColorFromHtmlElement.substring(startOfValuesIndex, endOfValuesIndex)
        .split(',')
        .map(Number);

    const [r, g, b] = rgbValues;

    return { r, g, b };
}

function saveAllRelationships()
{
    const url = "/ProductCharacteristicsComparer" + "?handler=SaveAllRelationships";

    return fetch(url,
    {
        method: 'POST',
        headers:
        {
            'Content-Type': 'application/json',
            'RequestVerificationToken':
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    });
}

function saveAllPropertiesForCategory(categoryId)
{
    const url = "/ProductCharacteristicsComparer" + "?handler=SaveAllPropertiesForCategory" + "&categoryId=" + categoryId;

    return fetch(url,
    {
        method: 'POST',
        headers:
        {
            'Content-Type': 'application/json',
            'RequestVerificationToken':
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    });
}

function saveAllImageFileNamesForCategory(categoryId)
{
    const url = "/ProductCharacteristicsComparer" + "?handler=SaveAllImageFileNamesForCategory" + "&categoryId=" + categoryId;

    return fetch(url,
    {
        method: 'POST',
        headers:
        {
            'Content-Type': 'application/json',
            'RequestVerificationToken':
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    });
}

function saveAllImagesForCategory(categoryId)
{
    const url = "/ProductCharacteristicsComparer" + "?handler=SaveAllImagesForTestingForCategory" + "&categoryId=" + categoryId;

    return fetch(url,
    {
        method: 'POST',
        headers:
        {
            'Content-Type': 'application/json',
            'RequestVerificationToken':
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    });
}

function deleteSavedRelationsForCategory(categoryId)
{
    const url = "/ProductCharacteristicsComparer" + "?handler=DeleteSavedRelationsForCategory" + "&categoryId=" + categoryId;

    return fetch(url,
    {
        method: 'DELETE',
        headers:
        {
            'Content-Type': 'application/json',
            'RequestVerificationToken':
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    });
}