﻿using ChiselDebug.GraphFIR.IO;
using FIRRTL;
using System;
using System.Collections.Generic;
using VCDReader;
#nullable enable

namespace ChiselDebug.GraphFIR.Components
{
    public sealed class ConstValue : FIRRTLNode, INoPlaceAndRoute
    {
        public readonly ILiteral Value;
        public readonly Source Result;
        private bool FirstCompute = true;

        public ConstValue(ILiteral value) : base(value)
        {
            Value = value;
            Result = new Source(this, null, value.GetFIRType());
        }

        public override Sink[] GetSinks()
        {
            return Array.Empty<Sink>();
        }

        public override Source[] GetSources()
        {
            return new Source[] { Result };
        }

        public override IEnumerable<FIRIO> GetIO()
        {
            yield return Result;
        }

        public override void Compute()
        {
            if (Result.Value == null)
            {
                throw new InvalidOperationException($"{nameof(Result)} value was not initialized.");
            }

            if (FirstCompute)
            {
                FirstCompute = false;

                BinaryVarValue binValue = new BinaryVarValue(Value.Width, true);
                binValue.SetBitsAndExtend(Value.Value);
                Result.Value.UpdateValue(ref binValue);
            }
        }

        internal override void InferType()
        { }
    }
}
