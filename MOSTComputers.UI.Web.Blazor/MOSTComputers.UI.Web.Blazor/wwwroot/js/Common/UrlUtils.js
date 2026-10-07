export function openDataUrlInNewWindow(url)
{
    window.open(url, '_blank');
}

export function openFileDataInNewWindow(fileData, contentType, newWindowTitle)
{
    const blob = new Blob([fileData], { type: contentType });

    const url = URL.createObjectURL(blob);

    const newWindow = window.open(url, '_blank');

    newWindow.onload = () => URL.revokeObjectURL(url);

    if (newWindow)
    {
        newWindow.document.title = newWindowTitle;
    }
}
