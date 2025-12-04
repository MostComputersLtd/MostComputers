function getDefaultBarcodeReader()
{
    const hints = new Map();

    hints.set(ZXing.DecodeHintType.POSSIBLE_FORMATS,
        [
            ZXing.BarcodeFormat.CODE_128,
            ZXing.BarcodeFormat.CODE_39,
            ZXing.BarcodeFormat.EAN_13,
            ZXing.BarcodeFormat.EAN_8,
            ZXing.BarcodeFormat.UPC_A,
            ZXing.BarcodeFormat.UPC_E,
        ]
    );

    hints.set(ZXing.DecodeHintType.TRY_HARDER, false);

    return new ZXing.BrowserMultiFormatReader(hints);
}

function getEANBarcodeReader()
{
    const hints = new Map();

    hints.set(ZXing.DecodeHintType.POSSIBLE_FORMATS,
        [
            ZXing.BarcodeFormat.EAN_13,
            ZXing.BarcodeFormat.EAN_8,
        ]
    );

    hints.set(ZXing.DecodeHintType.TRY_HARDER, false);

    return new ZXing.BrowserMultiFormatReader(hints);
}

function getSerialNumberBarcodeReader()
{
    const hints = new Map();

    hints.set(ZXing.DecodeHintType.POSSIBLE_FORMATS,
        [
            ZXing.BarcodeFormat.CODE_128,
            ZXing.BarcodeFormat.CODE_39,
        ]
    );

    hints.set(ZXing.DecodeHintType.TRY_HARDER, true);

    return new ZXing.BrowserMultiFormatReader(hints);
}

async function decodeBarcodeFromCameraAndPutOnVideo(barcodeReader, videoInputDeviceId, videoElementId)
{
    const videoElement = document.getElementById(videoElementId);

    return new Promise((resolve, reject) =>
    {
        barcodeReader.decodeFromVideoDevice(videoInputDeviceId, videoElement, (result, err) =>
        {
            if (result)
            {
                barcodeReader.reset();

                resolve(result.getText());
            }
            else if (err && !(err instanceof ZXing.NotFoundException))
            {
                barcodeReader.reset();

                reject(err);
            }
        });
    });
}
async function decodeBarcodeFromRegion(barcodeReader, stream, regionWidth, regionHeight)
{
    const video = document.createElement("video");

    video.srcObject = stream;

    await video.play();

    return await decodeBarcodeFromVideoRegion(barcodeReader, video, regionWidth, regionHeight);
}

async function decodeBarcodeFromVideoRegion(barcodeReader, video, regionWidth, regionHeight)
{
    const canvas = document.createElement("canvas");

    const ctx = canvas.getContext("2d");

    canvas.width = regionWidth;
    canvas.height = regionHeight;

    return new Promise((resolve) =>
    {
        let scanning = true;

        async function scanFrame()
        {
            if (!scanning) return;

            const sx = (video.videoWidth - regionWidth) / 2;
            const sy = (video.videoHeight - regionHeight) / 2;

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

                        barcodeReader.reset();

                        resolve(result.getText());

                        return;
                    }
                }
                catch (err)
                {
                    if (!(err instanceof ZXing.NotFoundException))
                    {
                        barcodeReader.reset();

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

async function decodeBarcodeFromImageUrl(barcodeReader, imageFile)
{
    const imageUrl = URL.createObjectURL(imageFile);

    return await barcodeReader.decodeFromImageUrl(imageUrl);
}