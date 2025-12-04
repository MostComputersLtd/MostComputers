export async function fetchAndDownloadFile(url, fileName, acceptHeader)
{
    const response = await fetch(url,
    {
        method: 'GET',
        headers:
        {
            'Accept': acceptHeader
        }
    });

    if (!response.ok) return;

    await downloadFile(response, fileName);
}

export async function fetchAndDownloadFileWithBody(url, fileName, acceptHeader, requestMethod = "POST", requestBodyAsJson = null)
{
    const response = await fetch(url,
    {
        method: requestMethod,
        headers:
        {
            'Content-Type': 'application/json',
            'Accept': acceptHeader
        },
        body: (requestBodyAsJson == null) ? null : requestBodyAsJson
    });

    if (!response.ok) return;

    await downloadFile(response, fileName);
}

async function downloadFile(response, fileName)
{
    const blob = await response.blob();

    const link = document.createElement('a');

    link.href = URL.createObjectURL(blob);

    link.download = fileName || 'download';

    link.target = "_blank";
    link.rel = "noopener";

    document.body.appendChild(link);

    link.click();
    link.remove();

    URL.revokeObjectURL(link.href);
}