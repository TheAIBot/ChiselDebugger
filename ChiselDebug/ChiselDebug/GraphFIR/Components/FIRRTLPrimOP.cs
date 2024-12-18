﻿using ChiselDebug.GraphFIR.IO;
using FIRRTL;
#nullable enable

namespace ChiselDebug.GraphFIR.Components
{
    public abstract class FIRRTLPrimOP : FIRRTLNode
    {
        public readonly Source Result;

        public FIRRTLPrimOP(IFIRType type, IFirrtlNode defNode) : base(defNode)
        {
            Result = new Source(this, null, type);
        }

        public override Source[] GetSources()
        {
            return new Source[] { Result };
        }
    }
}
