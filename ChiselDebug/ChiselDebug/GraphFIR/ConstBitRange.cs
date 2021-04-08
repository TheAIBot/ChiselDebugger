using ChiselDebug.GraphFIR.IO;
using FIRRTL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiselDebug.GraphFIR
{
    public abstract class ConstBitRange : FIRRTLPrimOP
    {
        public readonly string OpName;
        public readonly Input In;

        public ConstBitRange(string name, Output arg1, IFIRType outType, FirrtlNode defNode) : base(outType, defNode)
        {
            this.OpName = name;
            this.In = new Input(this, arg1.Type);
            arg1.ConnectToInput(In);
        }

        public override ScalarIO[] GetInputs()
        {
            return new ScalarIO[] { In };
        }

        public override FIRIO[] GetIO()
        {
            return new FIRIO[]
            {
                In,
                Result
            };
        }
    }

    public class Head : ConstBitRange
    {
        public readonly int FromMSB;
        public Head(Output arg1, IFIRType outType, int fromMSB, FirrtlNode defNode) : base("head", arg1, outType, defNode)
        {
            this.FromMSB = fromMSB;
        }

        public override void InferType()
        {
            In.InferType();

            IFIRType type = In.Type switch
            {
                UIntType a => new UIntType(FromMSB),
                SIntType a => new UIntType(FromMSB),
                _ => throw new Exception("Failed to infer type.")
            };
            Result.SetType(type);
        }
    }

    public class Tail : ConstBitRange
    {
        public readonly int FromLSB;
        public Tail(Output arg1, IFIRType outType, int fromLSB, FirrtlNode defNode) : base("tail", arg1, outType, defNode)
        {
            this.FromLSB = fromLSB;
        }

        public override void InferType()
        {
            In.InferType();

            IFIRType type = In.Type switch
            {
                UIntType a => new UIntType(a.Width - FromLSB),
                SIntType a => new UIntType(a.Width - FromLSB),
                _ => throw new Exception("Failed to infer type.")
            };
            Result.SetType(type);
        }
    }

    public class BitExtract : ConstBitRange
    {
        public readonly int StartInclusive;
        public readonly int EndInclusive;
        public BitExtract(Output arg1, IFIRType outType, int startInclusive, int endInclusive, FirrtlNode defNode) : base("bits", arg1, outType, defNode)
        {
            this.StartInclusive = startInclusive;
            this.EndInclusive = endInclusive;
        }

        public override void InferType()
        {
            In.InferType();

            IFIRType type = In.Type switch
            {
                UIntType a => new UIntType(EndInclusive - StartInclusive + 1),
                SIntType a => new UIntType(EndInclusive - StartInclusive + 1),
                _ => throw new Exception("Failed to infer type.")
            };
            Result.SetType(type);
        }
    }

    public class Pad : ConstBitRange
    {
        public readonly int WidthAfterPad;
        public Pad(Output arg1, IFIRType outType, int newWidth, FirrtlNode defNode) : base("pad", arg1, outType, defNode)
        {
            this.WidthAfterPad = newWidth;
        }

        public override void InferType()
        {
            In.InferType();

            IFIRType type = In.Type switch
            {
                UIntType a => new UIntType(Math.Max(a.Width, WidthAfterPad)),
                SIntType a => new SIntType(Math.Max(a.Width, WidthAfterPad)),
                _ => throw new Exception("Failed to infer type.")
            };
            Result.SetType(type);
        }
    }
}
