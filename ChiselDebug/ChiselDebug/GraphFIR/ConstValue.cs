﻿using System;

namespace ChiselDebug.GraphFIR
{
    public class ConstValue : FIRRTLNode, INoPlaceAndRoute
    {
        public readonly FIRRTL.Literal Value;
        public readonly Output Result;

        public ConstValue(FIRRTL.Literal value)
        {
            this.Value = value;
            this.Result = new Output("lol", new FIRRTL.UnknownType());
        }

        public override Input[] GetInputs()
        {
            return Array.Empty<Input>();
        }

        public override Output[] GetOutputs()
        {
            return new Output[] { Result };
        }
    }
}