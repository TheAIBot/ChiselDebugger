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