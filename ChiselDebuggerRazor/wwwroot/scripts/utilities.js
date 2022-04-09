class ElemWH {
    constructor(width, height) {
        this.Width = width;
        this.Height = height;
    }

    approxEquals(other) {
        const allowedDelta = 1;
        return this.Width  - allowedDelta <= other.Width  && other.Width  <= this.Width  + allowedDelta &&
               this.Height - allowedDelta <= other.Height && other.Height <= this.Height + allowedDelta;
    }
}

class ElemResizes {
    constructor(ids, sizes) {
        this.IDs = ids;
        this.Sizes = sizes;
    }
}


var JSUtils = JSUtils || {};
JSUtils.getElementSize = function(element) {
    return new ElemWH(element.clientWidth, element.clientHeight);
};
JSUtils.getViewPortSize = function () {
    return new ElemWH(window.innerWidth, window.innerHeight);
};
JSUtils.getElementPosition = function (element) {
    const elemRect = element.getBoundingClientRect();
    return new ElemWH(elemRect.left, elemRect.top);
}

const oldSizes = {};
const reObserver = new ResizeObserver(async entries => {
    var elemID = []
    var sizes = []
    for (let entry of entries) {
        const elementID = entry.target.id;

        const oldSize = oldSizes[elementID];
        const newSize = JSUtils.getElementSize(entry.target);

        if (newSize.Width == 0 && newSize.Height == 0) {
            continue;
        }

        if (!oldSize.approxEquals(newSize)) {
            oldSizes[elementID] = newSize;
            elemID.push(elementID);
            sizes.push(newSize);
        }
    }

    var toWait = []
    while (elemID.length > 0) {
        var elemIDsPart = []
        var sizesPart = []
        for (var i = 0; i < Math.min(elemID.length, 500); i++) {
            elemIDsPart.push(elemID.pop())
            sizesPart.push(sizes.pop())
        }

        toWait.push(DotNet.invokeMethodAsync("ChiselDebuggerRazor", "ResizeEventsAsync", new ElemResizes(elemIDsPart, sizesPart)));
    }

    await Promise.all(toWait);
});

JSUtils.addResizeListeners = function (elementIDs) {
    for (let elementID of elementIDs) {
        let element = document.getElementById(elementID);
        if (element == null) {
            continue;
        }

        oldSizes[elementID] = new ElemWH(0, 0);
        reObserver.observe(element);
    }
}

JSUtils.addDragListener = function (elementID) {
    var isMouseDown = false;
    var oldMouseX = 0;
    var oldMouseY = 0;
    var draggedX = 0;
    var draggedY = 0;
    var isDragQueued = false;

    let element = document.getElementById(elementID);
    if (element == null) {
        return;
    }

    element.addEventListener("mousedown", function (e) {
        isMouseDown = true;
        oldMouseX = e.clientX;
        oldMouseY = e.clientY;
        draggedX = 0;
        draggedY = 0;
    }, true);
    document.addEventListener("mouseup", function (e) {
        isMouseDown = false;
    }, true);
    element.addEventListener("mousemove", function (e) {
        if (isMouseDown) {
            draggedX += e.clientX - oldMouseX;
            draggedY += e.clientY - oldMouseY;

            oldMouseX = e.clientX;
            oldMouseY = e.clientY;

            if (!isDragQueued) {
                isDragQueued = true;

                window.requestAnimationFrame(async function () {
                    isDragQueued = false;

                    let movedX = draggedX;
                    let movedY = draggedY;

                    draggedX = 0;
                    draggedY = 0;

                    await DotNet.invokeMethodAsync("ChiselDebuggerRazor", "DragEventAsync", elementID, movedX, movedY);
                });
            }
        }
    }, true);
}