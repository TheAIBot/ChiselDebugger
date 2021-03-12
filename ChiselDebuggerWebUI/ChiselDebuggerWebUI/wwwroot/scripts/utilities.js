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


var JSUtils = JSUtils || {};
JSUtils.getElementSize = function(element) {
    return new ElemWH(element.clientWidth, element.clientHeight);
};
JSUtils.getElementPosition = function (element) {
    const elemRect = element.getBoundingClientRect();
    return new ElemWH(elemRect.left, elemRect.top);
}

const oldSizes = {};
const reObserver = new ResizeObserver(entries => {
    for (let entry of entries) {
        onElemResize(entry.target);
    }
});

function onElemResize(element) {
    const elementID = element.id;

    const oldSize = oldSizes[elementID];
    const newSize = JSUtils.getElementSize(element);

    if (!oldSize.approxEquals(newSize)) {
        oldSizes[elementID] = newSize;
        DotNet.invokeMethodAsync("ChiselDebuggerWebUI", "ResizeEventAsync", elementID, newSize);
    }
}

JSUtils.addResizeListener = function (elementID) {
    oldSizes[elementID] = new ElemWH(0, 0);
    reObserver.observe(document.getElementById(elementID));
}

JSUtils.addDragListener = function (elementID) {
    var isMouseDown = false;
    var oldMouseX = 0;
    var oldMouseY = 0;
    var draggedX = 0;
    var draggedY = 0;
    var isDragQueued = false;

    let element = document.getElementById(elementID);
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

                window.requestAnimationFrame(function () {
                    isDragQueued = false;

                    let movedX = draggedX;
                    let movedY = draggedY;

                    draggedX = 0;
                    draggedY = 0;

                    DotNet.invokeMethodAsync("ChiselDebuggerWebUI", "DragEventAsync", elementID, movedX, movedY);
                });
            }
        }
    }, true);
}