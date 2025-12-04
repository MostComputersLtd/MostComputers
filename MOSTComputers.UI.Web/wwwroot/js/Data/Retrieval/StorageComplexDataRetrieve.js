function getComplexObjectFromStorage(storage, key)
{
    if (!(storage instanceof Storage)) return null;

    const dataAsString = storage.getItem(key);

    if (!dataAsString) return null;

    return JSON.parse(dataAsString);
}

function setComplexObjectToStorage(storage, key, data)
{
    if (!(storage instanceof Storage)) return false;

    const dataAsString = JSON.stringify(data);

    storage.setItem(key, dataAsString);

    return true;
}