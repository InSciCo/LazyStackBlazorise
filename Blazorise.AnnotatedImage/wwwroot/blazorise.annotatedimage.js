
export function initialize() {
    
}

export function setPointerCapture(element, pointerid) {
    element.setPointerCapture(pointerid);
}

export function getImageAnnotationDataURL(elm) {
    var img = elm.firstChild;
    return getBase64Image(img);
}

export function getImageAnnotationDataURLById(id) {
    var img = document.getElementById(id).firstChild;
    return getBase64Image(img);
}

export function canvasElementToDataURL(elm) {
    return elm.toDataURL('image/png');
}

export function getBoundingClientRect(elm) {
    return elm.getBoundingClientRect();
}

export function getBase64Image(img) {
    // Create an empty canvas element
    var canvas = document.createElement("canvas");
    canvas.width = img.naturalWidth;
    canvas.height = img.naturalHeight;

    // Copy the image contents to the canvas
    var ctx = canvas.getContext("2d");
    ctx.drawImage(img, 0, 0);

    // Using default image/png becuase Safari doesn't support the type argument'
    var dataURL = canvas.toDataURL();
    return dataURL.replace(/^data:image\/(png|jpg);base64,/, "");
}




