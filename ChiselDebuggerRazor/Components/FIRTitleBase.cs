﻿using ChiselDebug.GraphFIR.Components;
using ChiselDebug.Utilities;
using ChiselDebuggerRazor.Code;
using System.Threading.Tasks;

namespace ChiselDebuggerRazor.Components
{
    public abstract class FIRTitleBase<T> : FIRBase<T> where T : FIRRTLNode
    {
        protected readonly string TitleID = UniqueID.UniqueHTMLID();
        private bool IsTitleHeightKnown = false;
        private Point TitleSize = Point.Zero;

        protected override Task OnResizeAsync(int width, int height)
        {
            if (IsTitleHeightKnown)
            {
                SinkOffsets = OnMakeSinks(width, height);
                SourceOffsets = OnMakeSources(width, height);

                return ParentLayoutCtrl?.UpdateComponentInfoAsync(new FIRComponentUpdate(Operation, GetCurrentSize(), SinkOffsets, SourceOffsets)) ?? Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                AddSizeWatcher(TitleID, UpdateTitleSizeAsync);
            }

            return base.OnAfterRenderAsync(firstRender);
        }

        private Task UpdateTitleSizeAsync(ElemWH size)
        {
            Point newSize = size.ToPoint();
            if (TitleSize != newSize)
            {
                TitleSize = newSize;
                IsTitleHeightKnown = true;

                Point firSize = GetCurrentSize();
                return OnResizeAsync(firSize.X, firSize.Y);
            }

            return Task.CompletedTask;
        }

        protected int GetPaddedTitleHeight()
        {
            return (int)(TitleSize.Y * 1.2f);
        }
    }
}
