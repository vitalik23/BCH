//Preload the image
var drgimg = document.createElement("img");
drgimg.src = 'data:image/gif;base64,R0lGODlhAQABAIAAAAUEBAAAACwAAAAAAQABAAACAkQBADs=';

// for Safari and Firefox
if (!("path" in MouseEvent.prototype)) {
    Object.defineProperty(MouseEvent.prototype, "path", {
        get: function () {

            var path = [];
            var currentElem = this.target;
            while (currentElem) {
                path.push(currentElem);
                currentElem = currentElem.parentElement;
            }
            if (path.indexOf(window) === -1 && path.indexOf(document) === -1)
                path.push(document);
            if (path.indexOf(window) === -1)
                path.push(window);
            return path;
        }
    });
}

function getPathCoordinates(event) {

    const pathCoordinates = event.path.map(element => {
        if (element.getBoundingClientRect) {
            const viewportOffset = element.getBoundingClientRect();

            return {
                x: event.pageX - viewportOffset.left,
                y: event.pageY - viewportOffset.top,
                scrollTop: element.scrollTop,
                classList: element.classList.value,
                id: element.id
            };
        }
    }).filter(x => x);

    return pathCoordinates;
}

const bchGhostElement = document.createElement('div');
bchGhostElement.style.opacity = 0;
var bchGhostElementAppended = false;

Blazor.registerCustomEventType('extscroll', {
    browserEventName: 'scroll',
    createEventArgs: event => {

        return {
            clientHeight: event.target.clientHeight,
            scrollHeight: event.target.scrollHeight,
            scrollTop: event.target.scrollTop,
            clientWidth: event.target.clientWidth
        };
    }
});

Blazor.registerCustomEventType('extdragstart', {
    browserEventName: 'dragstart',
    createEventArgs: event => {

        event.dataTransfer.effectAllowed = "copyMove";

        if (!bchGhostElementAppended) {
            document.body.appendChild(bchGhostElement);
            bchGhostElementAppended = true;
        }

        var isSafari = /constructor/i.test(window.HTMLElement) || (function (p) { return p.toString() === "[object SafariRemoteNotification]"; })(!window['safari'] || (typeof safari !== 'undefined' && safari.pushNotification));

        if (!isSafari) {
            event.dataTransfer.setDragImage(bchGhostElement, 0, 0);
        } else {
            event.dataTransfer.setDragImage(drgimg, 0, 0);
        }

        setTimeout(function () {
            event.target.setAttribute('dragging', '');
        }, 0);

        const pathCoordinates = getPathCoordinates(event);

        return {
            offsetX: event.offsetX,
            offsetY: event.offsetY,
            pageX: event.pageX,
            pageY: event.pageY,
            screenX: event.screenX,
            screenY: event.screenY,

            pathCoordinates: pathCoordinates
        };
    }
});

Blazor.registerCustomEventType('extdragend', {
    browserEventName: 'dragend',
    createEventArgs: event => {
        event.target.removeAttribute('dragging');
        return event;
    }
}); 

Blazor.registerCustomEventType('onextdragenter', {
    browserEventName: 'dragenter',
    createEventArgs: event => {
        event.dataTransfer.dropEffect = "copy";
        return event;
    }
});

Blazor.registerCustomEventType('extmousewheel', {
    browserEventName: 'mousewheel',
    createEventArgs: event => {

        const x = event.clientX - event.target.offsetLeft;
        const y = event.clientY - event.target.offsetTop;
        const pathCoordinates = getPathCoordinates(event);

        return {
            x: x,
            y: y,
            deltaX: event.deltaX,
            deltaY: event.deltaY,
            pathCoordinates: pathCoordinates
        };
    }
});

Blazor.registerCustomEventType('mouseleave', {
    browserEventName: 'mouseleave',
    createEventArgs: event => {

        const pathCoordinates = getPathCoordinates(event);

        return {
            offsetX: event.offsetX,
            offsetY: event.offsetY,
            pageX: event.pageX,
            pageY: event.pageY,
            screenX: event.screenX,
            screenY: event.screenY,

            pathCoordinates: pathCoordinates
        };
    }
});

function subscribeOnMouseMove(contextId, dotNetHelper) {
    const contextElement = document.getElementById(contextId);

    if(!contextElement) return;
    
    contextElement.addEventListener('mousemove', (event) => {
        const pathCoordinates = getPathCoordinates(event);

        dotNetHelper.invokeMethodAsync('OnMouseMove', {
            offsetX: event.offsetX,
            offsetY: event.offsetY,
            pageX: event.pageX,
            pageY: event.pageY,
            screenX: event.screenX,
            screenY: event.screenY,

            pathCoordinates: pathCoordinates
        });
    });
}