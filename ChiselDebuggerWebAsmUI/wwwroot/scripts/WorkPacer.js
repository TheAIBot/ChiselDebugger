var WorkPacing = {};
WorkPacing.WaitForAnimationFrame = function () {
    window.requestAnimationFrame(async function () {
        await DotNet.invokeMethodAsync("ChiselDebuggerWebAsmUI", "FrameHasBeenAnimatedAsync");
    });
};