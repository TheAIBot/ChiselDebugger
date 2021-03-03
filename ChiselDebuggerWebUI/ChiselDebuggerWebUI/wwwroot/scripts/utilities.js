class ElemWH {
    constructor(width, height) {
        this.Width = width;
        this.Height = height;
    }
}


var JSUtils = JSUtils || {};
JSUtils.getElementSize = function(element) {
    const elemRect = element.getBoundingClientRect();
    return new ElemWH(elemRect.width, elemRect.height);
};


let isScrollQueued = false;
let scrollDelta = 0;

function FireScrollEvent() {
    isScrollQueued = false;

    let delta = scrollDelta;
    scrollDelta = 0;
    DotNet.invokeMethodAsync("ChiselDebuggerWebUI", "ScrollEventAsync", delta);
}

document.addEventListener("wheel", function (e) {
    if (e.deltaY > 0) {
        scrollDelta++;
    }
    else {
        scrollDelta--;
    }

    if (!isScrollQueued) {
        isScrollQueued = true;
        window.requestAnimationFrame(FireScrollEvent);
    }
}, true);