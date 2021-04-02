using FIRRTL;
using System;
using System.Linq;

namespace ChiselDebug.GraphFIR.IO
{
    public class Input : ScalarIO
    {
        private Output SinkSource = null;

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

        public override FIRIO GetOutput()
        {
            if (SinkSource == null)
            {
                SinkSource = (Output)Flip();
            }

            return SinkSource;
        }

        public void MakeSinkOnly()
        {
            if (SinkSource == null)
            {
                return;
            }

            if (!SinkSource.Con.IsUsed())
            {
                throw new Exception("Probably an error when a source is created in a sink but it's not connected to anything.");
            }

            if (!IsConnected())
            {
                throw new Exception("Sink must be connected when it's also used as a source.");
            }

            IOHelper.BypassIO(SinkSource, Con.From);
            SinkSource = null;
        }

        public void Disconnect()
        {
            Con.DisconnectInput(this);
        }

        public override void ConnectToInput(FIRIO input, bool allowPartial = false, bool asPassive = false)
        {
            throw new Exception("Input can't be connected to output. Flow is reversed.");
        }

        public override FIRIO ToFlow(FlowChange flow, FIRRTLNode node = null)
        {
            return flow switch
            {
                FlowChange.Source => new Output(node ?? Node, Name, Type),
                FlowChange.Sink => new Input(node ?? Node, Name, Type),
                FlowChange.Flipped => new Output(node ?? Node, Name, Type),
                FlowChange.Preserve => new Input(node ?? Node, Name, Type),
                var error => throw new Exception($"Unknown flow. Flow: {flow}")
            };
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
