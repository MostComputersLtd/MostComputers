const videoAndOverlayContainerElementId = "productBarcodeReaderVideoAndOverlayContainer";

const videoElementContainerId = "productBarcodeReaderVideoContainer";
const videoElementId = "productBarcodeReaderVideo";

const decodeImageFileInputElementId = "productBarcodeReaderDecodeImageFileInput";

const scanOverlayElementId = "productBarcodeReaderScanOverlay";
const scanOverlayVisiblePartElementId = "productBarcodeReaderScanOverlayVisiblePart";

export async function decodeProductGTINCodeAndDisplayChanges()
{
    const device = await getVideoInputDevice();

    const videoElement = document.getElementById(videoElementId);

    const videoAndOverlayContainerElement = document.getElementById(videoAndOverlayContainerElementId);
    const scanOverlayElement = document.getElementById(scanOverlayElementId);
    const scanOverlayVisiblePartElement = document.getElementById(scanOverlayVisiblePartElementId);

    videoElement.srcObject = device;

    await waitForVideoMetadataLoad(videoElement);

    videoElement.play();

    const videoScanRegionWidth = videoElement.videoWidth * 0.4;
    const videoScanRegionHeight = videoElement.videoHeight * 0.4;

    var videoScanRegionSize = Math.min(videoScanRegionWidth, videoScanRegionHeight);

    videoScanRegionSize = Math.max(videoScanRegionWidth, 300);

    scanOverlayVisiblePartElement.style.width = `${videoScanRegionSize}px`;
    scanOverlayVisiblePartElement.style.height = `${videoScanRegionSize}px`;

    videoAndOverlayContainerElement.classList.add("started-scan");
    scanOverlayElement.classList.add("started-scan");

    toggleElementDisplay(videoElementContainerId);

    const barcodeReader = getDefaultBarcodeReader();

    const productGTINCode = await decodeBarcodeFromVideoRegion(barcodeReader, videoElement, videoScanRegionSize, videoScanRegionSize);

    ZXingBrowser.BrowserCodeReader.disposeMediaStream(device);

    toggleElementDisplay(videoElementContainerId);

    videoAndOverlayContainerElement.classList.remove("started-scan");
    scanOverlayElement.classList.remove("started-scan");

    videoElement.srcObject = null;

    return productGTINCode;
}

export async function decodeProductGTINCodeFromFileAndDisplayChanges()
{
    const fileInputElement = document.getElementById(decodeImageFileInputElementId);

    const firstImageFile = Array.from(fileInputElement.files).find(file => file.type.startsWith('image/'));

    const barcodeReader = getDefaultBarcodeReader();

    const productGTINCode = await decodeBarcodeFromImageUrl(barcodeReader, firstImageFile);

    return productGTINCode;
}
export function forceFileInputClick(fileInputElementId)
{
    const fileInputElement = document.getElementById(fileInputElementId);

    fileInputElement.click();
}

async function getVideoInputDevice()
{
    return await navigator.mediaDevices.getUserMedia({ video: true });
}

async function decodeBarcodeFromVideoRegion(barcodeReader, video, regionWidth, regionHeight)
{
    const canvas = document.createElement("canvas");

    const ctx = canvas.getContext("2d");

    canvas.width = regionWidth;
    canvas.height = regionHeight;

    const sx = (video.videoWidth - regionWidth) / 2;
    const sy = (video.videoHeight - regionHeight) / 2;

    return new Promise((resolve, reject) =>
    {
        let scanning = true;

        async function scanFrame()
        {
            if (!scanning) return;

            ctx.clearRect(0, 0, regionWidth, regionHeight);
            ctx.drawImage(video, sx, sy, regionWidth, regionHeight, 0, 0, regionWidth, regionHeight);

            const dataUrl = canvas.toDataURL();
            
            const img = new Image();

            img.onload = async () =>
            {
                try
                {
                    const result = await barcodeReader.decodeFromImageElement(img);

                    if (result)
                    {
                        scanning = false;

                        resolve(result.getText());

                        return;
                    }
                }
                catch (err)
                {
                    if (!(err instanceof ZXingLib.NotFoundException))
                    {
                        reject(err);
                    }
                }

                requestAnimationFrame(scanFrame);
            };

            img.src = dataUrl;
        }

        scanFrame();
    });
}

async function waitForVideoMetadataLoad(videoElement)
{
    await new Promise((resolve) =>
    {
        if (videoElement.readyState >= 1)
        {
            resolve();

            return;
        }

        videoElement.addEventListener('loadedmetadata', () => resolve(), { once: true });
    });
}

function getDefaultBarcodeReader()
{
    const hints = new Map();

    hints.set(ZXingLib.DecodeHintType.POSSIBLE_FORMATS,
        [
            ZXingLib.BarcodeFormat.CODE_128,
            ZXingLib.BarcodeFormat.CODE_39,
            ZXingLib.BarcodeFormat.EAN_13,
            ZXingLib.BarcodeFormat.EAN_8,
            ZXingLib.BarcodeFormat.UPC_A,
            ZXingLib.BarcodeFormat.UPC_E,
        ]
    );

    hints.set(ZXingLib.DecodeHintType.TRY_HARDER, true);
    //hints.set(ZXingLib.DecodeHintType.TRY_INVERTED, true);

    return new ZXingLib.BrowserMultiFormatReader(hints);
}

async function decodeBarcodeFromImageUrl(barcodeReader, imageFile)
{
    const imageUrl = URL.createObjectURL(imageFile);

    return await barcodeReader.decodeFromImageUrl(imageUrl);
}

function toggleElementDisplay(elementId)
{
    const element = document.getElementById(elementId);

    if (element.style.display === "none")
    {
        element.style.display = "";
    }
    else
    {
        element.style.display = "none";
    }
}