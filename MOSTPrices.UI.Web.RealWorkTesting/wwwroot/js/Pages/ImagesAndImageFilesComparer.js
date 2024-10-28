async function onChange_DataSourceSelect_GetImageDataFromSource(
    imageComparisonDataPlacementOption,
    categorySelectElementId,
    productIdInputElementId,
    dataSourceSelectId,
    partialViewContainerId)
{
    const categorySelectElement = document.getElementById(categorySelectElementId);
    const productIdInputElement = document.getElementById(productIdInputElementId);

    const dataSourceSelect = document.getElementById(dataSourceSelectId);

    const categoryId = parseInt(categorySelectElement.value);
    const productId = parseInt(productIdInputElement.value);
    const dataSource = parseInt(dataSourceSelect.value);

    if (isNaN(dataSource))
    {
        return false;
    }

    if (!isNaN(productId))
    {
        const getImageDataForProductPromise = getImageDataForProductAsync(productId, imageComparisonDataPlacementOption, dataSource);

        if (getImageDataForProductPromise == null) return;

        const response = await getImageDataForProductPromise;

        if (response.status !== 200) return;

        const partialViewContainer = document.getElementById(partialViewContainerId);

        const responseText = await response.text();

        if (responseText == null
            || responseText == "")
        {
            return;
        }

        partialViewContainer.innerHTML = responseText;
    }
    else if (!isNaN(categoryId))
    {
        const getImageDataForCategoryPromise = getImageDataForCategoryAsync(categoryId, imageComparisonDataPlacementOption, dataSource);

        if (getImageDataForCategoryPromise == null) return;

        const response = await getImageDataForCategoryPromise;

        if (response.status !== 200) return;

        const partialViewContainer = document.getElementById(partialViewContainerId);

        const responseText = await response.text();

        if (responseText == null
            || responseText == "")
        {
            return;
        }

        partialViewContainer.innerHTML = responseText;
    }
}

function getImageDataForCategoryAsync(categoryId, imageComparisonDataPlacementOption, dataSourceOption)
{
    if (typeof categoryId !== "number"
        || typeof imageComparisonDataPlacementOption !== "number"
        || typeof dataSourceOption !== "number")
    {
        return null;
    }

    const url = "/ImagesAndImageFilesComparer" + "?handler=GetImageDataForCategory"
        + "&categoryId=" + categoryId
        + "&imageComparisonDataPlacementLocation=" + imageComparisonDataPlacementOption
        + "&dataSource=" + dataSourceOption;

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

async function onClick_ProductIdSelectButton_GetImageDataFromSourceForProduct(
    imageComparisonDataPlacementOption,
    productInputId,
    dataSourceSelectId)
{
    const productInput = document.getElementById(productInputId);
    const dataSourceSelect = document.getElementById(dataSourceSelectId);

    const productId = parseInt(productInput.value);
    const dataSource = parseInt(dataSourceSelect.value);

    if (isNaN(productId)
        || isNaN(dataSource))
    {
        return false;
    }

    const getImageDataForProductPromise = getImageDataForProductAsync(productId, imageComparisonDataPlacementOption, dataSource);

    if (getImageDataForProductPromise == null) return;

    const response = await getImageDataForProductPromise;

    if (response.status !== 200) return;

    location.reload();
}

function getImageDataForProductAsync(productId, imageComparisonDataPlacementOption, dataSourceOption)
{
    if (typeof productId !== "number"
        || typeof imageComparisonDataPlacementOption !== "number"
        || typeof dataSourceOption !== "number")
    {
        return null;
    }

    const url = "/ImagesAndImageFilesComparer" + "?handler=GetImageDataForProduct"
        + "&productId=" + productId
        + "&imageComparisonDataPlacementLocation=" + imageComparisonDataPlacementOption
        + "&dataSource=" + dataSourceOption;

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