// For reference, See:
// https://developer.mozilla.org/en-US/docs/Web/API/Media_Capture_and_Streams_API/Taking_still_photos
// https://developer.mozilla.org/en-US/docs/Web/API/MediaDevices/getUserMedia

// |streaming| indicates whether or not we're currently streaming
// video from the camera. Obviously, we start at false.
var streaming = false;

// The various HTML elements we need to configure or control. These
// will be set by the initialize() function.
var video = null;
var moduleInstance = null;
export function initialize(videoRef, mirrorImage, facingMode, moduleInstanceRef) {
    let isAvailable = true;
    video = videoRef;
    moduleInstance = moduleInstanceRef;
    facingMode = (facingMode == null || facingMode == "") ? "environment" : facingMode;
    //mirror = mirrorImage;

    navigator.mediaDevices.getUserMedia( // request media input and returns a media stream
        { // Pass in constraints for requested media input
            video:
            { 
                facingMode: facingMode // one of "user" or "environment"
            },
            audio: false
        })
        .then(function (stream) {
            video.srcObject = stream;
            video.play();
            //mirror image
            if (mirrorImage) {
                video.style.webkitTransform = "scaleX(-1)";
                video.style.transform = "scaleX(-1)";
            }
        })
        .catch(function (err) {
            console.log("An error occurred: " + err);
            isAvailable = false;
            moduleInstance.invokeMethodAsync("OnCameraUnavailable");
        });

    video.addEventListener('canplay', function (ev) {
        if (!streaming) {
            streaming = true;
        }
        moduleInstance.invokeMethodAsync("OnCameraInitialized");
    }, false);
    return isAvailable;
}
export function releaseCamera() {
    const stream = video.srcObject;
    const tracks = stream.getTracks();
    tracks.forEach(track => track.stop());
    video.srcObject = null;
}
export async function getWidthAndHeight() {
    if (video == null || isNaN(video.videoWidth))
        return [0, 0];
    const width = video.videoWidth;
    const height = isNaN(video.videoHeight) ? width / (4.0 / 3.0) : video.videoHeight;
    return [ parseInt(width), parseInt(height) ];
}
export function takepicture() {
    // Firefox currently has a bug where the height can't be read from
    // the video, so we will make assumptions if this happens.
    const width = video.videoWidth;
    const height = isNaN(video.videoHeight) ? width / (4.0 / 3.0) : video.videoHeight;
    const canvas = document.createElement('canvas');
    var context = canvas.getContext('2d');
    canvas.width = video.videoWidth;
    canvas.height = height;
    context.drawImage(video, 0, 0, canvas.width,  canvas.height);
    var data = canvas.toDataURL('image/png');
    canvas.remove();
    return data;
}

