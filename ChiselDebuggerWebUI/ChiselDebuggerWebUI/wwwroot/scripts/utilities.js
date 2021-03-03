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

JSUtils.addScrollListener = function (elementID) {
    var isScrollQueued = false;
    var scrollDelta = 0;

    document.getElementById(elementID).addEventListener("wheel", function (e) {
        if (e.deltaY > 0) {
            scrollDelta++;
        }
        else {
            scrollDelta--;
        }

        if (!isScrollQueued) {
            isScrollQueued = true;
            window.requestAnimationFrame(function () {
                isScrollQueued = false;

                let delta = scrollDelta;
                scrollDelta = 0;

                DotNet.invokeMethodAsync("ChiselDebuggerWebUI", "ScrollEventAsync", elementID, delta);
            });
        }
    });
}