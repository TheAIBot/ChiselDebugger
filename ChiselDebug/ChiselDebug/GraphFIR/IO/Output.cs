using FIRRTL;
using System;

namespace ChiselDebug.GraphFIR.IO
{
    public class Output : ScalarIO
    {
        public Output(FIRRTLNode node, IFIRType type) : this(node, string.Empty, type)
        { }

        public Output(FIRRTLNode node, string name, IFIRType type) : base(node, name, type)
        {
            this.Con = new Connection(this);
        }

        public override void SetType(IFIRType type)
        {
            Type = type;
            Con.Value = new ValueType(type);
        }

        public override FIRIO GetOutput()
        {
            return this;
        }

        public override void ConnectToInput(FIRIO input, bool allowPartial = false, bool asPassive = false)
        {
            if (input is Input ioIn)
            {
                Con.ConnectToInput(ioIn);
            }
            else
            {
                throw new Exception("Output can only be connected to input.");
            }
        }

        public override FIRIO Flip()
        {
            return new Input(Node, Name, Type);
        }

        public override FIRIO Copy()
        {
            return new Output(Node, Name, Type);
        }

        public override bool IsPassiveOfType<T>()
        {
            return this is T;
        }

        public override bool SameIO(FIRIO other)
        {
            return other is Output otherOut &&
                   Type.Equals(otherOut.Type);
        }

        public void InferType()
        {
            if (Node != null && Type is UnknownType)
            {
                Node.InferType();
            }
        }


    }
}
