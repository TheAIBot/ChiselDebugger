class ElemWH {
    constructor(width, height) {
        this.Width = width;
        this.Height = height;
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