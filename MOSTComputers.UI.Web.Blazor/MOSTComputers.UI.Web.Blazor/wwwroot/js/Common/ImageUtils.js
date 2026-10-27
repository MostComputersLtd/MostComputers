export function getImageFileDimensions(blob) {

    return new Promise((resolve, reject) => {

        const image = new Image();

        image.onload = () => {

            URL.revokeObjectURL(image.src);

            resolve({
                width: image.naturalWidth,
                height: image.naturalHeight
            });
        };

        image.onerror = () => {

            URL.revokeObjectURL(image.src);

            reject(new Error("Invalid image file."));
        };

        image.src = URL.createObjectURL(blob);
    });
}
