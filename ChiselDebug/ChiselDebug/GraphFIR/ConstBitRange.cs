using ChiselDebug.GraphFIR.IO;
using FIRRTL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCDReader;

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

        public override Input[] GetInputs()
        {
            return new Input[] { In };
        }

        public override IEnumerable<FIRIO> GetIO()
        {
            yield return In;
            yield return Result;
        }

        public override void Compute()
        {
            In.UpdateValueFromSource();

            BinaryVarValue aVal = In.Value.GetValue();
            BinaryVarValue resultVal = Result.Value.GetValue();

            if (!aVal.IsValidBinary())
            {
                Array.Fill(resultVal.Bits, BitState.X);
            }

            ConstBitRangeCompute(aVal, resultVal);
        }
        protected abstract void ConstBitRangeCompute(BinaryVarValue a, BinaryVarValue result);
    }

    public class Head : ConstBitRange
    {
        public readonly int FromMSB;
        public Head(Output arg1, IFIRType outType, int fromMSB, FirrtlNode defNode) : base("head", arg1, outType, defNode)
        {
            this.FromMSB = fromMSB;
        }

        protected override void ConstBitRangeCompute(BinaryVarValue a, BinaryVarValue result)
        {
            Array.Copy(a.Bits, a.Bits.Length - FromMSB, result.Bits, 0, FromMSB);
        }

        internal override void InferType()
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

        protected override void ConstBitRangeCompute(BinaryVarValue a, BinaryVarValue result)
        {
            Array.Copy(a.Bits, 0, result.Bits, 0, a.Bits.Length - FromLSB);
        }

        internal override void InferType()
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

        protected override void ConstBitRangeCompute(BinaryVarValue a, BinaryVarValue result)
        {
            Array.Copy(a.Bits, StartInclusive, result.Bits, 0, EndInclusive - StartInclusive + 1);
        }

        internal override void InferType()
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

        protected override void ConstBitRangeCompute(BinaryVarValue a, BinaryVarValue result)
        {
            Array.Copy(a.Bits, result.Bits, a.Bits.Length);

            BitState signExt = In.Type is SIntType ? a.Bits[^1] : BitState.Zero;
            Array.Fill(result.Bits, signExt, a.Bits.Length, WidthAfterPad - a.Bits.Length);
        }

        internal override void InferType()
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
