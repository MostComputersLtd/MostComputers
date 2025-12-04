function getFilesFromDataTransfer(dataTransfer)
{
    const files = [];

    if (dataTransfer.items)
    {
        [...dataTransfer.items].forEach((item, i) =>
        {
            if (item.kind === "file")
            {
                const file = item.getAsFile();

                files.push(file);
            }
        });

        return files;
    }
    else
    {
        [...dataTransfer.files].forEach((file, i) =>
        {
            files.push(file);
        });

        return files;
    }

    return files;
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