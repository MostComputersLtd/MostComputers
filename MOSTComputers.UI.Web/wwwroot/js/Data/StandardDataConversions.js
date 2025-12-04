function getIntOrNullFromString(value)
{
    const valueAsInt = parseInt(value);

    return isNaN(valueAsInt) ? null : valueAsInt;
}

function getDateOrNullFromString(value)
{
    const valueAsDate = Date.parse(value);

    return isNaN(valueAsDate) ? null : new Date(valueAsDate);
}

function getFormDataFromObject(obj, formData = new FormData(), parentKey = "")
{
    for (const key in obj)
    {
        if (!obj.hasOwnProperty(key)) continue;

        const value = obj[key];

        let nestedKey = parentKey ? `${parentKey}.${key}` : key;

        if (Array.isArray(value))
        {
            value.forEach((item, index) =>
            {
                let arrayKey = `${nestedKey}[${index}]`;

                if (item == null
                    || typeof item !== "object"
                    || item instanceof File
                    || item instanceof Blob)
                {
                    formData.append(arrayKey, item);
                }
                else
                {
                    getFormDataFromObject(item, formData, arrayKey);
                }
            });
        }
        else if (value == null
            || typeof value !== "object"
            || value instanceof File
            || value instanceof Blob)
        {
            formData.append(nestedKey, value);
        }
        else
        {
            getFormDataFromObject(value, formData, nestedKey);
        }
    }

    return formData;
}