const jsonDataLocalStorageKey = "single-product-editor-data";

let localData = null;

let localFileData = {
    productImages: [],
    productDocuments: [],
};

export function loadData() {

    if (localData == null) {

        const storedData = window.localStorage.getItem(jsonDataLocalStorageKey);

        if (storedData) {

            localData = JSON.parse(storedData);
        }
    }

    return localData;
}

export function saveData(jsonDataObject) {

    localData = jsonDataObject;

    const jsonDataParsed = JSON.stringify(jsonDataObject);

    window.localStorage.setItem(jsonDataLocalStorageKey, jsonDataParsed);
}

export function loadImageData(editorId) {

    const data = loadData();

    const productImage = data.productImages.find(x => x.editorId === editorId);

    return productImage ? productImage : null;
}

export function addProductImageAndSave(editorId, productImageData, file) {

    const data = loadData();

    const productImage = data.productImages.find(x => x.editorId === editorId); 

    if (productImage) return false;

    data.productImages.push(productImageData);
    localFileData.productImages.push(file);

    saveData(data);

    return true;
}

export function updateProductImageAndSave(editorId, mutator) {

    const data = loadData();

    const productImage = data.productImages.find(x => x.editorId === editorId);

    mutator(productImage);

    saveData(data);
}

export function removeImageAndSave(editorId) {

    const data = loadData();

    const productImageIndex = data.productImages.findIndex(x => x.editorId === editorId);

    if (productImageIndex === -1) return false;

    data.productImages.splice(productImageIndex, 1);

    saveData(data);

    return true;
}

export function updatePropertyAndSave(editorId, isLink, mutator) {

    const data = loadData();

    let property;

    if (isLink) {

        property = data.productLinks.find(x => x.editorId === editorId);
    }
    else {

        property = data.productProperties.find(x => x.editorId === editorId);
    }

    mutator(property);

    saveData(data);
}

export function updateProductDocumentAndSave(editorId, mutator) {

    const data = loadData();

    const productDocument = data.productDocuments.find(x => x.editorId === editorId);

    mutator(productDocument);

    saveData(data);
}

export function removeProductDocumentAndSave(editorId) {

    const data = loadData();

    const productDocumentIndex = data.productDocuments.findIndex(x => x.editorId === editorId);

    if (productDocumentIndex == -1) return false;

    data.productDocuments.splice(productDocumentIndex, 1);

    saveData(data);

    return true;
}

function storageAvailable(type) {

  let storage;
  try {
    storage = window[type];
    const x = "__storage_test__";
    storage.setItem(x, x);
    storage.removeItem(x);
    return true;
  } catch (e) {
    return (
      e instanceof DOMException &&
      e.name === "QuotaExceededError" &&
      // acknowledge QuotaExceededError only if there's something already stored
      storage &&
      storage.length !== 0
    );
  }
}
