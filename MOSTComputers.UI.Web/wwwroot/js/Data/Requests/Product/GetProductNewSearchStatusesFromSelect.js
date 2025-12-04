function getProductNewStatusesFromSelectElementValue(productNewStatusesAsString)
{
    if (productNewStatusesAsString == null
        || productNewStatusesAsString.trim().length === 0)
    {
        return null;
    }

    const productNewStatusesAsList = productNewStatusesAsString.split(',');

    const productNewStatusesInOptions = [];

    for (var i = 0; i < productNewStatusesAsList.length; i++)
    {
        const productNewStatusAsString = productNewStatusesAsList[i];

        const productNewStatusInOptions = getIntOrNullFromString(productNewStatusAsString);

        if (productNewStatusInOptions == null) continue;

        productNewStatusesInOptions.push(productNewStatusInOptions);
    }

    return productNewStatusesInOptions;
}