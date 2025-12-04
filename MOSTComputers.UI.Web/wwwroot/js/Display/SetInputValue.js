function setInputValueIfNotNull(inputElementId, value)
{
    if (!value) return;

    const inputElement = document.getElementById(inputElementId);

    if (!inputElement) return;

    inputElement.value = value;
}