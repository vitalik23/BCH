window.addEventListener("resize", () => {
    DotNet.invokeMethodAsync("BlazorComponentHeap.Core", 'OnBrowserResizeAsync').then(data => data);
});

function bchGetBoundingClientRectById(id, param) {

    const element = document.getElementById(id);
    if (!element) return null;

    const rect = element.getBoundingClientRect();

    return {
        width: rect.width,
        height: rect.height,
        bottom: rect.bottom,
        left: rect.left,
        right: rect.right,
        top: rect.top,
        x: rect.x,
        y: rect.y,
        offsetTop: element.offsetTop,
        offsetLeft: element.offsetLeft,
        clientWidth: element.clientWidth,
        clientHeight: element.clientHeight,
        offsetWidth: element.offsetWidth,
        offsetHeight: element.offsetHeight
    };
}

function bchScrollElementTo(id, x, y, behavior) {
    const element = document.getElementById(id);

    if (!element) {
        return;
    }

    element.scrollTo({
        left: x,
        top: y,
        behavior: behavior // only 'auto' or 'smooth'
    });
}

const listeners = {};

function addDocumentListener(key, eventName, dotnetReference, methodName) {
    listeners[key + eventName] = function (event) {

        let response = {};

        switch (eventName) {
            case "touchmove":

                const touches = Object.entries(event.touches).map((value, key) => {

                    const touch = value[1];

                    return {
                        clientX: touch.clientX,
                        clientY: touch.clientY,
                        pageX: touch.pageX,
                        pageY: touch.pageY
                    }
                });

                response = {
                    touches: touches
                };
                break;
            default:
                const pathCoordinates = getPathCoordinates(event);
                
                response = {
                    offsetX: event.offsetX,
                    offsetY: event.offsetY,
                    pageX: event.pageX,
                    pageY: event.pageY,
                    screenX: event.screenX,
                    screenY: event.screenY,
                    clientX: event.clientX,
                    clientY: event.clientY,
                    pathCoordinates: pathCoordinates
                };
                break;
        }

        dotnetReference.invokeMethodAsync(methodName, response);
    };

    document.addEventListener(eventName, listeners[key + eventName]);
}

function removeDocumentListener(key, eventName) {
    if (listeners[key + eventName]) {
        document.removeEventListener(eventName, listeners[key + eventName]);
        delete listeners[key + eventName];
    }
}