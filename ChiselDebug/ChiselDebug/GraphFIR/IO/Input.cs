using FIRRTL;
using System;

namespace ChiselDebug.GraphFIR.IO
{
    public class Input : ScalarIO
    {
        public Input(FIRRTLNode node, IFIRType type) : this(node, string.Empty, type)
        { }

        public Input(FIRRTLNode node, string name, IFIRType type) : base(node, name, type)
        { }

        public override void SetType(IFIRType type)
        {
            Type = type;
        }

        public override FIRIO GetInput()
        {
            return this;
        }

        public override void ConnectToInput(FIRIO input, bool allowPartial = false, bool asPassive = false)
        {
            throw new Exception("Input can't be connected to output. Flow is reversed.");
        }

        public override FIRIO Flip(FIRRTLNode node = null)
        {
            return new Output(node ?? Node, Name, Type);
        }

        public override FIRIO Copy(FIRRTLNode node = null)
        {
            return new Input(node ?? Node, Name, Type);
        }

        public override bool IsPassiveOfType<T>()
        {
            return this is T;
        }

        public override bool SameIO(FIRIO other)
        {
            return other is Input otherIn && 
                   Type.Equals(otherIn.Type);
        }

        public override void InferType()
        {
            if (Con != null && Type is UnknownType)
            {
                Con.From.InferType();
                Type = Con.From.Type;
            }
        }
    }
}
