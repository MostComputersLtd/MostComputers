export function createObjectUrlForFileInInputElement(fileInputElementId, fileIndex)
{
    const input = document.getElementById(fileInputElementId);

    if (!input || !input.files || input.files.length <= fileIndex)
    {
        return null;
    }

    return URL.createObjectURL(input.files[fileIndex]);
}

export function revokeObjectUrl(url)
{
    if (url)
    {
        URL.revokeObjectURL(url);
    }
}

export function forceFileInputClick(fileInputElementId)
{
    const fileInputElement = document.getElementById(fileInputElementId);

    fileInputElement.click();
}

export function registerPromotionFileDropZone(dropzoneElementId, fileInputElementId)
{
    const dropzoneElement = document.getElementById(dropzoneElementId);

    dropzoneElement.addEventListener("drop", function (event)
    {
        onDropChangeInputFilesAndTriggerChangeEvent(event, fileInputElementId);
    },
        { once: true });
}

function onDropChangeInputFilesAndTriggerChangeEvent(dropEvent, fileInputElementId)
{
    const files = getFilesFromDataTransferAsFileList(dropEvent.dataTransfer);

    if (files.length <= 0) return;

    const fileInputElement = document.getElementById(fileInputElementId);

    fileInputElement.files = files;

    fileInputElement.dispatchEvent(new Event("change"));
}

function getFilesFromDataTransferAsFileList(dataTransfer)
{
    const files = getFilesFromDataTransfer(dataTransfer);

    const outputDataTransfer = new DataTransfer();

    files.forEach(function (file)
    {
        outputDataTransfer.items.add(file);
    })

    return outputDataTransfer.files;
}